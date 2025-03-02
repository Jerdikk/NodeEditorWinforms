namespace MathSample
{
    partial class FormMathSample
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlNodeEditor = new SampleCommon.ControlNodeEditor();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rk86ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lOADMEMORYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sAVEMEMORYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlNodeEditor
            // 
            this.controlNodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlNodeEditor.Location = new System.Drawing.Point(0, 24);
            this.controlNodeEditor.MaximumSize = new System.Drawing.Size(1800, 1600);
            this.controlNodeEditor.Name = "controlNodeEditor";
            this.controlNodeEditor.Size = new System.Drawing.Size(957, 486);
            this.controlNodeEditor.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.runToolStripMenuItem,
            this.rk86ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(957, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // rk86ToolStripMenuItem
            // 
            this.rk86ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lOADMEMORYToolStripMenuItem,
            this.sAVEMEMORYToolStripMenuItem,
            this.testToolStripMenuItem});
            this.rk86ToolStripMenuItem.Name = "rk86ToolStripMenuItem";
            this.rk86ToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.rk86ToolStripMenuItem.Text = "rk86";
            // 
            // lOADMEMORYToolStripMenuItem
            // 
            this.lOADMEMORYToolStripMenuItem.Name = "lOADMEMORYToolStripMenuItem";
            this.lOADMEMORYToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.lOADMEMORYToolStripMenuItem.Text = "LOAD MEMORY";
            this.lOADMEMORYToolStripMenuItem.Click += new System.EventHandler(this.lOADMEMORYToolStripMenuItem_Click);
            // 
            // sAVEMEMORYToolStripMenuItem
            // 
            this.sAVEMEMORYToolStripMenuItem.Name = "sAVEMEMORYToolStripMenuItem";
            this.sAVEMEMORYToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.sAVEMEMORYToolStripMenuItem.Text = "SAVE MEMORY";
            this.sAVEMEMORYToolStripMenuItem.Click += new System.EventHandler(this.sAVEMEMORYToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.testToolStripMenuItem.Text = "test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // FormMathSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 510);
            this.Controls.Add(this.controlNodeEditor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMathSample";
            this.Text = "NodeEditor WinForms - Math Sample";
            this.Load += new System.EventHandler(this.FormMathSample_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SampleCommon.ControlNodeEditor controlNodeEditor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rk86ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lOADMEMORYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sAVEMEMORYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
    }
}

