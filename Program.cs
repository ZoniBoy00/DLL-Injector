using System;
using System.Drawing;
using System.Windows.Forms;

namespace DLLInjectorApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set the application icon globally
            try
            {
                Form1.MainFormIcon = new Icon("Resources/icon2.ico");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load global icon: {ex.Message}");
            }

            Application.Run(new Form1());
        }
    }
}