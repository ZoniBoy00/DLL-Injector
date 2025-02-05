using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLLInjectorApp
{
    public partial class Form1 : Form
    {
        // Windows API constants for process manipulation
        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        // Importing necessary Windows API functions
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        // Global icon property for the form
        public static Icon MainFormIcon { get; set; }

        public Form1()
        {
            InitializeComponent();
            InitializeDarkTheme();
            LoadProcesses();

            // Set the form's icon if it exists
            if (MainFormIcon != null)
            {
                this.Icon = MainFormIcon;
            }
        }

        /// <summary>
        /// Initializes the dark theme for the application.
        /// </summary>
        private void InitializeDarkTheme()
        {
            this.BackColor = Color.FromArgb(30, 30, 30); // Main background color

            lstProcesses.BackColor = Color.FromArgb(40, 40, 40); // ListView background color
            lstProcesses.ForeColor = Color.White; // ListView text color
            lstProcesses.GridLines = true; // Enable grid lines
            lstProcesses.FullRowSelect = true; // Allow selecting entire rows
            lstProcesses.Font = new Font("Segoe UI", 9F); // Improve font readability

            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.BackColor = Color.FromArgb(40, 40, 40);
                    textBox.ForeColor = Color.White;
                    textBox.Font = new Font("Segoe UI", 9F);
                }
                else if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
                    button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 50, 50);
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
                    button.BackColor = Color.FromArgb(50, 50, 50);
                    button.ForeColor = Color.White;
                    button.Font = new Font("Segoe UI", 9F);
                }
                else if (control is Label label)
                {
                    label.ForeColor = Color.White;
                    label.Font = new Font("Segoe UI", 9F);
                }
            }
        }

        /// <summary>
        /// Loads all running processes into the ListView.
        /// </summary>
        private void LoadProcesses()
        {
            Task.Run(() =>
            {
                var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToList();
                lstProcesses.Invoke((MethodInvoker)delegate
                {
                    lstProcesses.Items.Clear();
                    foreach (var process in processes)
                    {
                        var item = new ListViewItem(new[] { process.ProcessName, process.Id.ToString() }) { Tag = process };
                        item.Font = new Font(lstProcesses.Font, FontStyle.Regular);
                        lstProcesses.Items.Add(item);
                    }
                });
            });
        }

        /// <summary>
        /// Filters the processes based on the search query.
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearch.Text.ToLower();
            lstProcesses.Items.Clear();
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.ToLower().Contains(filter))
                {
                    lstProcesses.Items.Add(new ListViewItem(new[] { process.ProcessName, process.Id.ToString() }) { Tag = process });
                }
            }
        }

        /// <summary>
        /// Handles the "Browse" button click to select a DLL file.
        /// </summary>
        private void btnSelectDLL_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DLL Files (*.dll)|*.dll";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDLLPath.Text = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Handles the "Inject DLL" button click.
        /// </summary>
        private void btnInject_Click(object sender, EventArgs e)
        {
            string dllPath = txtDLLPath.Text.Trim();

            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                MessageBox.Show("Please select a valid DLL file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lstProcesses.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a process from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var selectedItem = lstProcesses.SelectedItems[0];
                InjectDLL(selectedItem.SubItems[0].Text, dllPath);
                MessageBox.Show("DLL injected successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Injection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Injects the specified DLL into the target process.
        /// </summary>
        private void InjectDLL(string processName, string dllPath)
        {
            Process targetProcess = null;

            foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)))
            {
                targetProcess = process;
                break;
            }

            if (targetProcess == null)
            {
                throw new Exception("Target process not found.");
            }

            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, targetProcess.Id);
            if (hProcess == IntPtr.Zero)
            {
                throw new Exception("Failed to open process.");
            }

            IntPtr allocatedMem = IntPtr.Zero;
            IntPtr remoteThread = IntPtr.Zero;

            try
            {
                byte[] dllPathBytes = Encoding.ASCII.GetBytes(dllPath + "\0");
                allocatedMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllPathBytes.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

                if (allocatedMem == IntPtr.Zero)
                {
                    throw new Exception("Failed to allocate memory in the target process.");
                }

                if (!WriteProcessMemory(hProcess, allocatedMem, dllPathBytes, (uint)dllPathBytes.Length, out _))
                {
                    throw new Exception("Failed to write DLL path to the target process.");
                }

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    throw new Exception("Failed to get LoadLibrary address.");
                }

                remoteThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocatedMem, 0, IntPtr.Zero);
                if (remoteThread == IntPtr.Zero)
                {
                    throw new Exception("Failed to create remote thread.");
                }

                WaitForSingleObject(remoteThread, 0xFFFFFFFF);
            }
            finally
            {
                if (allocatedMem != IntPtr.Zero)
                {
                    VirtualFreeEx(hProcess, allocatedMem, 0, MEM_COMMIT);
                }

                if (remoteThread != IntPtr.Zero)
                {
                    CloseHandle(remoteThread);
                }

                if (hProcess != IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                }
            }
        }
    }
}