namespace UltimaTileEditor
{
    partial class FormUltimaEditor
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
            gbGame = new GroupBox();
            rbUltima2 = new RadioButton();
            rbUltima3 = new RadioButton();
            rbUltima4 = new RadioButton();
            rbUltima5 = new RadioButton();
            rbUltima1 = new RadioButton();
            gbGameLocation = new GroupBox();
            btnGameDataBrowse = new Button();
            lblGameData = new Label();
            tbGameDataDir = new TextBox();
            gbOutImages = new GroupBox();
            btnImageBrowse = new Button();
            tbImagesDir = new TextBox();
            lblImagesDirectory = new Label();
            gbFilesLocated = new GroupBox();
            lbFiles = new ListBox();
            gbImages = new GroupBox();
            lbImages = new ListBox();
            btnExtract = new Button();
            btnCompress = new Button();
            pnlGame = new Panel();
            gbPalette = new GroupBox();
            cbPalette = new ComboBox();
            gbFileType = new GroupBox();
            cbFileType = new ComboBox();
            pnlLoc = new Panel();
            splitContainer1 = new SplitContainer();
            pnlControl = new Panel();
            gbGame.SuspendLayout();
            gbGameLocation.SuspendLayout();
            gbOutImages.SuspendLayout();
            gbFilesLocated.SuspendLayout();
            gbImages.SuspendLayout();
            pnlGame.SuspendLayout();
            gbPalette.SuspendLayout();
            gbFileType.SuspendLayout();
            pnlLoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            pnlControl.SuspendLayout();
            SuspendLayout();
            // 
            // gbGame
            // 
            gbGame.Controls.Add(rbUltima2);
            gbGame.Controls.Add(rbUltima3);
            gbGame.Controls.Add(rbUltima4);
            gbGame.Controls.Add(rbUltima5);
            gbGame.Controls.Add(rbUltima1);
            gbGame.Dock = DockStyle.Top;
            gbGame.Location = new Point(0, 0);
            gbGame.Name = "gbGame";
            gbGame.Size = new Size(128, 167);
            gbGame.TabIndex = 0;
            gbGame.TabStop = false;
            gbGame.Text = "Game";
            // 
            // rbUltima2
            // 
            rbUltima2.AutoSize = true;
            rbUltima2.Location = new Point(21, 47);
            rbUltima2.Name = "rbUltima2";
            rbUltima2.Size = new Size(69, 19);
            rbUltima2.TabIndex = 5;
            rbUltima2.Text = "Ultima 2";
            rbUltima2.UseVisualStyleBackColor = true;
            rbUltima2.CheckedChanged += rbUltima2_CheckedChanged;
            // 
            // rbUltima3
            // 
            rbUltima3.AutoSize = true;
            rbUltima3.Location = new Point(21, 72);
            rbUltima3.Name = "rbUltima3";
            rbUltima3.Size = new Size(69, 19);
            rbUltima3.TabIndex = 4;
            rbUltima3.Text = "Ultima 3";
            rbUltima3.UseVisualStyleBackColor = true;
            rbUltima3.CheckedChanged += rbUltima3_CheckedChanged;
            // 
            // rbUltima4
            // 
            rbUltima4.AutoSize = true;
            rbUltima4.Checked = true;
            rbUltima4.Location = new Point(21, 97);
            rbUltima4.Name = "rbUltima4";
            rbUltima4.Size = new Size(69, 19);
            rbUltima4.TabIndex = 3;
            rbUltima4.TabStop = true;
            rbUltima4.Text = "Ultima 4";
            rbUltima4.UseVisualStyleBackColor = true;
            rbUltima4.CheckedChanged += rbUltima4_CheckedChanged;
            // 
            // rbUltima5
            // 
            rbUltima5.AutoSize = true;
            rbUltima5.Location = new Point(21, 122);
            rbUltima5.Name = "rbUltima5";
            rbUltima5.Size = new Size(69, 19);
            rbUltima5.TabIndex = 2;
            rbUltima5.Text = "Ultima 5";
            rbUltima5.UseVisualStyleBackColor = true;
            rbUltima5.CheckedChanged += rbUltima5_CheckedChanged;
            // 
            // rbUltima1
            // 
            rbUltima1.AutoSize = true;
            rbUltima1.Location = new Point(21, 22);
            rbUltima1.Name = "rbUltima1";
            rbUltima1.Size = new Size(69, 19);
            rbUltima1.TabIndex = 0;
            rbUltima1.Text = "Ultima 1";
            rbUltima1.UseVisualStyleBackColor = true;
            rbUltima1.CheckedChanged += rbUltima1_CheckedChanged;
            // 
            // gbGameLocation
            // 
            gbGameLocation.Controls.Add(btnGameDataBrowse);
            gbGameLocation.Controls.Add(lblGameData);
            gbGameLocation.Controls.Add(tbGameDataDir);
            gbGameLocation.Dock = DockStyle.Top;
            gbGameLocation.Location = new Point(0, 0);
            gbGameLocation.Name = "gbGameLocation";
            gbGameLocation.Size = new Size(572, 64);
            gbGameLocation.TabIndex = 1;
            gbGameLocation.TabStop = false;
            gbGameLocation.Text = "Game Files Location";
            // 
            // btnGameDataBrowse
            // 
            btnGameDataBrowse.Location = new Point(488, 19);
            btnGameDataBrowse.Name = "btnGameDataBrowse";
            btnGameDataBrowse.Size = new Size(75, 23);
            btnGameDataBrowse.TabIndex = 2;
            btnGameDataBrowse.Text = "Browse";
            btnGameDataBrowse.UseVisualStyleBackColor = true;
            btnGameDataBrowse.Click += btnGameDataBrowse_Click;
            // 
            // lblGameData
            // 
            lblGameData.AutoSize = true;
            lblGameData.Location = new Point(6, 24);
            lblGameData.Name = "lblGameData";
            lblGameData.Size = new Size(119, 15);
            lblGameData.TabIndex = 0;
            lblGameData.Text = "Game Data Directory:";
            // 
            // tbGameDataDir
            // 
            tbGameDataDir.Location = new Point(131, 19);
            tbGameDataDir.Name = "tbGameDataDir";
            tbGameDataDir.ReadOnly = true;
            tbGameDataDir.Size = new Size(351, 23);
            tbGameDataDir.TabIndex = 1;
            // 
            // gbOutImages
            // 
            gbOutImages.Controls.Add(btnImageBrowse);
            gbOutImages.Controls.Add(tbImagesDir);
            gbOutImages.Controls.Add(lblImagesDirectory);
            gbOutImages.Dock = DockStyle.Top;
            gbOutImages.Location = new Point(0, 64);
            gbOutImages.Name = "gbOutImages";
            gbOutImages.Size = new Size(572, 59);
            gbOutImages.TabIndex = 2;
            gbOutImages.TabStop = false;
            gbOutImages.Text = "Image Files Location";
            // 
            // btnImageBrowse
            // 
            btnImageBrowse.Location = new Point(488, 12);
            btnImageBrowse.Name = "btnImageBrowse";
            btnImageBrowse.Size = new Size(75, 23);
            btnImageBrowse.TabIndex = 3;
            btnImageBrowse.Text = "Browse";
            btnImageBrowse.UseVisualStyleBackColor = true;
            btnImageBrowse.Click += btnImageBrowse_Click;
            // 
            // tbImagesDir
            // 
            tbImagesDir.Location = new Point(131, 17);
            tbImagesDir.Name = "tbImagesDir";
            tbImagesDir.ReadOnly = true;
            tbImagesDir.Size = new Size(351, 23);
            tbImagesDir.TabIndex = 3;
            // 
            // lblImagesDirectory
            // 
            lblImagesDirectory.AutoSize = true;
            lblImagesDirectory.Location = new Point(6, 20);
            lblImagesDirectory.Name = "lblImagesDirectory";
            lblImagesDirectory.Size = new Size(99, 15);
            lblImagesDirectory.TabIndex = 1;
            lblImagesDirectory.Text = "Images Directory:";
            // 
            // gbFilesLocated
            // 
            gbFilesLocated.Controls.Add(lbFiles);
            gbFilesLocated.Dock = DockStyle.Fill;
            gbFilesLocated.Location = new Point(0, 0);
            gbFilesLocated.Name = "gbFilesLocated";
            gbFilesLocated.Size = new Size(242, 288);
            gbFilesLocated.TabIndex = 3;
            gbFilesLocated.TabStop = false;
            gbFilesLocated.Text = "Files";
            // 
            // lbFiles
            // 
            lbFiles.Dock = DockStyle.Fill;
            lbFiles.FormattingEnabled = true;
            lbFiles.Location = new Point(3, 19);
            lbFiles.Name = "lbFiles";
            lbFiles.Size = new Size(236, 266);
            lbFiles.TabIndex = 0;
            // 
            // gbImages
            // 
            gbImages.Controls.Add(lbImages);
            gbImages.Dock = DockStyle.Fill;
            gbImages.Location = new Point(0, 0);
            gbImages.Name = "gbImages";
            gbImages.Size = new Size(236, 288);
            gbImages.TabIndex = 4;
            gbImages.TabStop = false;
            gbImages.Text = "Images";
            // 
            // lbImages
            // 
            lbImages.Dock = DockStyle.Fill;
            lbImages.FormattingEnabled = true;
            lbImages.Location = new Point(3, 19);
            lbImages.Name = "lbImages";
            lbImages.Size = new Size(230, 266);
            lbImages.TabIndex = 0;
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(0, 19);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(80, 23);
            btnExtract.TabIndex = 5;
            btnExtract.Text = "Extract";
            btnExtract.UseVisualStyleBackColor = true;
            btnExtract.Click += btnExtract_Click;
            // 
            // btnCompress
            // 
            btnCompress.Location = new Point(3, 253);
            btnCompress.Name = "btnCompress";
            btnCompress.Size = new Size(80, 23);
            btnCompress.TabIndex = 6;
            btnCompress.Text = "Compress";
            btnCompress.UseVisualStyleBackColor = true;
            btnCompress.Click += btnCompress_Click;
            // 
            // pnlGame
            // 
            pnlGame.Controls.Add(gbPalette);
            pnlGame.Controls.Add(gbFileType);
            pnlGame.Controls.Add(gbGame);
            pnlGame.Dock = DockStyle.Left;
            pnlGame.Location = new Point(0, 0);
            pnlGame.Name = "pnlGame";
            pnlGame.Size = new Size(128, 411);
            pnlGame.TabIndex = 1;
            // 
            // gbPalette
            // 
            gbPalette.Controls.Add(cbPalette);
            gbPalette.Dock = DockStyle.Fill;
            gbPalette.Location = new Point(0, 231);
            gbPalette.Name = "gbPalette";
            gbPalette.Size = new Size(128, 180);
            gbPalette.TabIndex = 2;
            gbPalette.TabStop = false;
            gbPalette.Text = "Palette";
            // 
            // cbPalette
            // 
            cbPalette.Dock = DockStyle.Top;
            cbPalette.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPalette.FormattingEnabled = true;
            cbPalette.Location = new Point(3, 19);
            cbPalette.Name = "cbPalette";
            cbPalette.Size = new Size(122, 23);
            cbPalette.TabIndex = 1;
            cbPalette.SelectedIndexChanged += cbPalette_SelectedIndexChanged;
            // 
            // gbFileType
            // 
            gbFileType.Controls.Add(cbFileType);
            gbFileType.Dock = DockStyle.Top;
            gbFileType.Location = new Point(0, 167);
            gbFileType.Name = "gbFileType";
            gbFileType.Size = new Size(128, 64);
            gbFileType.TabIndex = 1;
            gbFileType.TabStop = false;
            gbFileType.Text = "File Type";
            // 
            // cbFileType
            // 
            cbFileType.Dock = DockStyle.Top;
            cbFileType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFileType.FormattingEnabled = true;
            cbFileType.Location = new Point(3, 19);
            cbFileType.Name = "cbFileType";
            cbFileType.Size = new Size(122, 23);
            cbFileType.TabIndex = 0;
            cbFileType.SelectedIndexChanged += cbFileType_SelectedIndexChanged;
            // 
            // pnlLoc
            // 
            pnlLoc.Controls.Add(gbOutImages);
            pnlLoc.Controls.Add(gbGameLocation);
            pnlLoc.Dock = DockStyle.Top;
            pnlLoc.Location = new Point(128, 0);
            pnlLoc.Name = "pnlLoc";
            pnlLoc.Size = new Size(572, 123);
            pnlLoc.TabIndex = 7;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(128, 123);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(gbFilesLocated);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(gbImages);
            splitContainer1.Size = new Size(482, 288);
            splitContainer1.SplitterDistance = 242;
            splitContainer1.TabIndex = 9;
            // 
            // pnlControl
            // 
            pnlControl.Controls.Add(btnExtract);
            pnlControl.Controls.Add(btnCompress);
            pnlControl.Dock = DockStyle.Right;
            pnlControl.Location = new Point(610, 123);
            pnlControl.Name = "pnlControl";
            pnlControl.Size = new Size(90, 288);
            pnlControl.TabIndex = 10;
            // 
            // FormUltimaEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 411);
            Controls.Add(splitContainer1);
            Controls.Add(pnlControl);
            Controls.Add(pnlLoc);
            Controls.Add(pnlGame);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormUltimaEditor";
            Text = "Ultima PC Graphics Extractor";
            Load += FormUltimaEditor_Load;
            gbGame.ResumeLayout(false);
            gbGame.PerformLayout();
            gbGameLocation.ResumeLayout(false);
            gbGameLocation.PerformLayout();
            gbOutImages.ResumeLayout(false);
            gbOutImages.PerformLayout();
            gbFilesLocated.ResumeLayout(false);
            gbImages.ResumeLayout(false);
            pnlGame.ResumeLayout(false);
            gbPalette.ResumeLayout(false);
            gbFileType.ResumeLayout(false);
            pnlLoc.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pnlControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gbGame;
        private RadioButton rbUltima2;
        private RadioButton rbUltima3;
        private RadioButton rbUltima4;
        private RadioButton rbUltima5;
        private RadioButton rbUltima1;
        private GroupBox gbGameLocation;
        private Button btnGameDataBrowse;
        private TextBox tbGameDataDir;
        private Label lblGameData;
        private GroupBox gbOutImages;
        private Button btnImageBrowse;
        private TextBox tbImagesDir;
        private Label lblImagesDirectory;
        private GroupBox gbFilesLocated;
        private ListBox lbFiles;
        private GroupBox gbImages;
        private ListBox lbImages;
        private Button btnExtract;
        private Button btnCompress;
        private Panel pnlGame;
        private Panel pnlLoc;
        private SplitContainer splitContainer1;
        private Panel pnlControl;
        private GroupBox gbFileType;
        private ComboBox cbFileType;
        private GroupBox gbPalette;
        private ComboBox cbPalette;
    }
}
