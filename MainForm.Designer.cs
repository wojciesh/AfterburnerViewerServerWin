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
            log = new TextBox();
            toolStripContainer1 = new ToolStripContainer();
            splitContainer1 = new SplitContainer();
            label1 = new Label();
            txtMeasurementsPreview = new TextBox();
            toolStrip2 = new ToolStrip();
            toolStripLabel2 = new ToolStripLabel();
            btOpenDir = new ToolStripButton();
            txtDir = new ToolStripTextBox();
            toolStripSeparator3 = new ToolStripSeparator();
            btSetABConfig = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            txtABStatus = new ToolStripTextBox();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            openToolStripButton = new ToolStripButton();
            txtFile = new ToolStripTextBox();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStrip3 = new ToolStrip();
            btRestartIpc = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            btCopyMeasurement = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            btHelp = new ToolStripButton();
            logTimer = new System.Windows.Forms.Timer(components);
            dlgOpen = new OpenFileDialog();
            dlgDir = new FolderBrowserDialog();
            timerABSettings = new System.Windows.Forms.Timer(components);
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip2.SuspendLayout();
            toolStrip1.SuspendLayout();
            toolStrip3.SuspendLayout();
            SuspendLayout();
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
            log.Size = new Size(785, 226);
            log.TabIndex = 1;
            log.WordWrap = false;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.AutoScroll = true;
            toolStripContainer1.ContentPanel.Controls.Add(splitContainer1);
            toolStripContainer1.ContentPanel.Size = new Size(785, 275);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(785, 350);
            toolStripContainer1.TabIndex = 2;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip2);
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip3);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(txtMeasurementsPreview);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(log);
            splitContainer1.Size = new Size(785, 275);
            splitContainer1.SplitterDistance = 45;
            splitContainer1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 5);
            label1.Name = "label1";
            label1.Size = new Size(132, 15);
            label1.TabIndex = 3;
            label1.Text = "Measurements preview:";
            // 
            // txtMeasurementsPreview
            // 
            txtMeasurementsPreview.Dock = DockStyle.Bottom;
            txtMeasurementsPreview.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMeasurementsPreview.Location = new Point(0, 24);
            txtMeasurementsPreview.Name = "txtMeasurementsPreview";
            txtMeasurementsPreview.ReadOnly = true;
            txtMeasurementsPreview.Size = new Size(785, 21);
            txtMeasurementsPreview.TabIndex = 2;
            // 
            // toolStrip2
            // 
            toolStrip2.Dock = DockStyle.None;
            toolStrip2.Items.AddRange(new ToolStripItem[] { toolStripLabel2, btOpenDir, txtDir, toolStripSeparator3, btSetABConfig, toolStripSeparator5, txtABStatus });
            toolStrip2.Location = new Point(3, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(723, 25);
            toolStrip2.TabIndex = 1;
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(88, 22);
            toolStripLabel2.Text = "Afterburner dir:";
            // 
            // btOpenDir
            // 
            btOpenDir.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btOpenDir.Image = (Image)resources.GetObject("btOpenDir.Image");
            btOpenDir.ImageTransparentColor = Color.Magenta;
            btOpenDir.Name = "btOpenDir";
            btOpenDir.Size = new Size(23, 22);
            btOpenDir.Text = "&Open";
            btOpenDir.Click += btOpenDir_Click;
            // 
            // txtDir
            // 
            txtDir.Name = "txtDir";
            txtDir.ReadOnly = true;
            txtDir.Size = new Size(250, 25);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // btSetABConfig
            // 
            btSetABConfig.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btSetABConfig.Image = (Image)resources.GetObject("btSetABConfig.Image");
            btSetABConfig.ImageTransparentColor = Color.Magenta;
            btSetABConfig.Name = "btSetABConfig";
            btSetABConfig.Size = new Size(128, 22);
            btSetABConfig.Text = "Enable AB History Log";
            btSetABConfig.Click += btSetABConfig_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // txtABStatus
            // 
            txtABStatus.Name = "txtABStatus";
            txtABStatus.ReadOnly = true;
            txtABStatus.Size = new Size(175, 25);
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, openToolStripButton, txtFile, toolStripSeparator2 });
            toolStrip1.Location = new Point(3, 25);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(506, 25);
            toolStrip1.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(133, 22);
            toolStripLabel1.Text = "Afterburner History File:";
            // 
            // openToolStripButton
            // 
            openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            openToolStripButton.Image = (Image)resources.GetObject("openToolStripButton.Image");
            openToolStripButton.ImageTransparentColor = Color.Magenta;
            openToolStripButton.Name = "openToolStripButton";
            openToolStripButton.Size = new Size(23, 22);
            openToolStripButton.Text = "&Open";
            openToolStripButton.Click += btSelectSourceFile_Click;
            // 
            // txtFile
            // 
            txtFile.Name = "txtFile";
            txtFile.ReadOnly = true;
            txtFile.Size = new Size(330, 25);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStrip3
            // 
            toolStrip3.Dock = DockStyle.None;
            toolStrip3.Items.AddRange(new ToolStripItem[] { btRestartIpc, toolStripSeparator4, btCopyMeasurement, toolStripSeparator1, btHelp });
            toolStrip3.Location = new Point(3, 50);
            toolStrip3.Name = "toolStrip3";
            toolStrip3.Size = new Size(284, 25);
            toolStrip3.TabIndex = 2;
            // 
            // btRestartIpc
            // 
            btRestartIpc.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btRestartIpc.Image = (Image)resources.GetObject("btRestartIpc.Image");
            btRestartIpc.ImageTransparentColor = Color.Magenta;
            btRestartIpc.Name = "btRestartIpc";
            btRestartIpc.Size = new Size(81, 22);
            btRestartIpc.Text = "Restart server";
            btRestartIpc.Click += btRestartIpc_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // btCopyMeasurement
            // 
            btCopyMeasurement.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btCopyMeasurement.Image = (Image)resources.GetObject("btCopyMeasurement.Image");
            btCopyMeasurement.ImageTransparentColor = Color.Magenta;
            btCopyMeasurement.Name = "btCopyMeasurement";
            btCopyMeasurement.Size = new Size(156, 22);
            btCopyMeasurement.Text = "Copy current measurement";
            btCopyMeasurement.Click += btCopyMeasurement_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // btHelp
            // 
            btHelp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btHelp.Image = (Image)resources.GetObject("btHelp.Image");
            btHelp.ImageTransparentColor = Color.Magenta;
            btHelp.Name = "btHelp";
            btHelp.Size = new Size(23, 22);
            btHelp.Text = "He&lp";
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
            // dlgDir
            // 
            dlgDir.RootFolder = Environment.SpecialFolder.CommonProgramFilesX86;
            // 
            // timerABSettings
            // 
            timerABSettings.Enabled = true;
            timerABSettings.Interval = 1000;
            timerABSettings.Tick += timerABSettings_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(785, 350);
            Controls.Add(toolStripContainer1);
            Name = "MainForm";
            Text = "AfterburnerToStreamDeck-Server";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            toolStrip3.ResumeLayout(false);
            toolStrip3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TextBox log;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private System.Windows.Forms.Timer logTimer;
        private ToolStripTextBox txtFile;
        private ToolStripLabel toolStripLabel1;
        private OpenFileDialog dlgOpen;
        private TextBox txtMeasurementsPreview;
        private SplitContainer splitContainer1;
        private Label label1;
        private ToolStripButton openToolStripButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton helpToolStripButton;
        private ToolStrip toolStrip2;
        private ToolStripLabel toolStripLabel2;
        private ToolStripButton btOpenDir;
        private ToolStripTextBox txtDir;
        private FolderBrowserDialog dlgDir;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStrip toolStrip3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripLabel toolStripLabel3;
        private ToolStripButton toolStripButton2;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton btHelp;
        private ToolStripButton btRestartIpc;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton btCopyMeasurement;
        private ToolStripButton btSetABConfig;
        private ToolStripTextBox txtABStatus;
        private System.Windows.Forms.Timer timerABSettings;
    }
}
