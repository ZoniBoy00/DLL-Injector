namespace DLLInjectorApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtDLLPath;
        private System.Windows.Forms.Button btnSelectDLL;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.ListView lstProcesses;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txtSearch;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtDLLPath = new System.Windows.Forms.TextBox();
            this.btnSelectDLL = new System.Windows.Forms.Button();
            this.btnInject = new System.Windows.Forms.Button();
            this.lstProcesses = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtSearch = new System.Windows.Forms.TextBox();

            // Form properties
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 360);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DLL Injector";

            // txtDLLPath
            this.txtDLLPath.Location = new System.Drawing.Point(15, 12);
            this.txtDLLPath.Name = "txtDLLPath";
            this.txtDLLPath.Size = new System.Drawing.Size(300, 22);
            this.txtDLLPath.TabIndex = 0;

            // btnSelectDLL
            this.btnSelectDLL.Location = new System.Drawing.Point(321, 10);
            this.btnSelectDLL.Name = "btnSelectDLL";
            this.btnSelectDLL.Size = new System.Drawing.Size(114, 25);
            this.btnSelectDLL.TabIndex = 1;
            this.btnSelectDLL.Text = "Browse DLL";
            this.btnSelectDLL.UseVisualStyleBackColor = true;
            this.btnSelectDLL.Click += new System.EventHandler(this.btnSelectDLL_Click);

            // btnInject
            this.btnInject.Location = new System.Drawing.Point(15, 40);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(420, 35);
            this.btnInject.TabIndex = 2;
            this.btnInject.Text = "Inject DLL";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);

            // lstProcesses
            this.lstProcesses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstProcesses.FullRowSelect = true;
            this.lstProcesses.GridLines = true;
            this.lstProcesses.Location = new System.Drawing.Point(15, 81);
            this.lstProcesses.Name = "lstProcesses";
            this.lstProcesses.Size = new System.Drawing.Size(420, 200);
            this.lstProcesses.TabIndex = 3;
            this.lstProcesses.UseCompatibleStateImageBehavior = false;
            this.lstProcesses.View = System.Windows.Forms.View.Details;

            // columnHeader1
            this.columnHeader1.Text = "Process Name";
            this.columnHeader1.Width = 250;

            // columnHeader2
            this.columnHeader2.Text = "PID";
            this.columnHeader2.Width = 100;

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(15, 300);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(300, 22);
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // Add controls to form
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lstProcesses);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.btnSelectDLL);
            this.Controls.Add(this.txtDLLPath);
        }
    }
}