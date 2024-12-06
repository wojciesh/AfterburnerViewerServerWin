namespace AfterburnerViewerServerWin
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            measurementTimer = new System.Windows.Forms.Timer(components);
            log = new TextBox();
            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();
            btRestartIpc = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            txtFile = new ToolStripTextBox();
            btSelectABFile = new ToolStripButton();
            logTimer = new System.Windows.Forms.Timer(components);
            dlgOpen = new OpenFileDialog();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // measurementTimer
            // 
            measurementTimer.Enabled = true;
            measurementTimer.Interval = 1000;
            measurementTimer.Tick += sendMeasurementTimer_Tick;
            // 
            // log
            // 
            log.Dock = DockStyle.Fill;
            log.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            log.Location = new Point(0, 0);
            log.Multiline = true;
            log.Name = "log";
            log.ReadOnly = true;
            log.ScrollBars = ScrollBars.Both;
            log.Size = new Size(800, 425);
            log.TabIndex = 1;
            log.WordWrap = false;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.AutoScroll = true;
            toolStripContainer1.ContentPanel.Controls.Add(log);
            toolStripContainer1.ContentPanel.Size = new Size(800, 425);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(800, 450);
            toolStripContainer1.TabIndex = 2;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] { btRestartIpc, toolStripSeparator1, toolStripLabel1, txtFile, btSelectABFile });
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(635, 25);
            toolStrip1.TabIndex = 0;
            // 
            // btRestartIpc
            // 
            btRestartIpc.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btRestartIpc.Image = (Image)resources.GetObject("btRestartIpc.Image");
            btRestartIpc.ImageTransparentColor = Color.Magenta;
            btRestartIpc.Name = "btRestartIpc";
            btRestartIpc.Size = new Size(81, 22);
            btRestartIpc.Text = "Restart server";
            btRestartIpc.Click += toolStripButton1_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(133, 22);
            toolStripLabel1.Text = "Afterburner History File:";
            // 
            // txtFile
            // 
            txtFile.Name = "txtFile";
            txtFile.Size = new Size(330, 25);
            txtFile.Text = "C:\\Users\\Wojtek\\Desktop\\MSI-AB\\HardwareMonitoring.hml";
            // 
            // btSelectABFile
            // 
            btSelectABFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btSelectABFile.Image = (Image)resources.GetObject("btSelectABFile.Image");
            btSelectABFile.ImageTransparentColor = Color.Magenta;
            btSelectABFile.Name = "btSelectABFile";
            btSelectABFile.Size = new Size(40, 22);
            btSelectABFile.Text = "Open";
            btSelectABFile.Click += btSelectABFile_Click;
            // 
            // logTimer
            // 
            logTimer.Enabled = true;
            logTimer.Interval = 500;
            logTimer.Tick += logTimer_Tick;
            // 
            // dlgOpen
            // 
            dlgOpen.FileName = "HardwareMonitoring.hml";
            dlgOpen.Filter = "HML Files (*.hml)|*.hml|All files (*.*)|*.*";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStripContainer1);
            Name = "MainForm";
            Text = "AfterburnerToStreamDeck-Server";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.ContentPanel.PerformLayout();
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer measurementTimer;
        private TextBox log;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripButton btRestartIpc;
        private System.Windows.Forms.Timer logTimer;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripTextBox txtFile;
        private ToolStripButton btSelectABFile;
        private ToolStripLabel toolStripLabel1;
        private OpenFileDialog dlgOpen;
    }
}
