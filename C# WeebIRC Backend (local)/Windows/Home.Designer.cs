namespace WeebIRCServerTray
{
    partial class Home
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.State = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.About = new MaterialSkin.Controls.MaterialRaisedButton();
            this.Debug = new MaterialSkin.Controls.MaterialRaisedButton();
            this.Start = new MaterialSkin.Controls.MaterialRaisedButton();
            this.Stop = new MaterialSkin.Controls.MaterialRaisedButton();
            this.Tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(16, 16);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(282, 133);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(255, 255);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(255, 255);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(255, 255);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // State
            // 
            this.State.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.State.AutoSize = true;
            this.State.Depth = 0;
            this.State.Font = new System.Drawing.Font("Roboto", 11F);
            this.State.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.State.Location = new System.Drawing.Point(267, 424);
            this.State.MouseState = MaterialSkin.MouseState.HOVER;
            this.State.Name = "State";
            this.State.Size = new System.Drawing.Size(64, 19);
            this.State.TabIndex = 20;
            this.State.Text = "Stopped";
            // 
            // materialLabel1
            // 
            this.materialLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(205, 424);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(56, 19);
            this.materialLabel1.TabIndex = 19;
            this.materialLabel1.Text = "Status:";
            // 
            // About
            // 
            this.About.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.About.BackColor = System.Drawing.Color.DodgerBlue;
            this.About.Cursor = System.Windows.Forms.Cursors.Hand;
            this.About.Depth = 0;
            this.About.ForeColor = System.Drawing.SystemColors.Highlight;
            this.About.Location = new System.Drawing.Point(647, 89);
            this.About.MouseState = MaterialSkin.MouseState.HOVER;
            this.About.Name = "About";
            this.About.Primary = true;
            this.About.Size = new System.Drawing.Size(152, 46);
            this.About.TabIndex = 18;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = false;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // Debug
            // 
            this.Debug.BackColor = System.Drawing.Color.DodgerBlue;
            this.Debug.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Debug.Depth = 0;
            this.Debug.ForeColor = System.Drawing.SystemColors.Highlight;
            this.Debug.Location = new System.Drawing.Point(26, 89);
            this.Debug.MouseState = MaterialSkin.MouseState.HOVER;
            this.Debug.Name = "Debug";
            this.Debug.Primary = true;
            this.Debug.Size = new System.Drawing.Size(152, 46);
            this.Debug.TabIndex = 17;
            this.Debug.Text = "Debug";
            this.Debug.UseCompatibleTextRendering = true;
            this.Debug.UseVisualStyleBackColor = false;
            this.Debug.Click += new System.EventHandler(this.Debug_Click_1);
            // 
            // Start
            // 
            this.Start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Start.BackColor = System.Drawing.Color.DodgerBlue;
            this.Start.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start.Depth = 0;
            this.Start.ForeColor = System.Drawing.SystemColors.Highlight;
            this.Start.Location = new System.Drawing.Point(26, 408);
            this.Start.MouseState = MaterialSkin.MouseState.HOVER;
            this.Start.Name = "Start";
            this.Start.Primary = true;
            this.Start.Size = new System.Drawing.Size(152, 46);
            this.Start.TabIndex = 16;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = false;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Stop
            // 
            this.Stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Stop.Depth = 0;
            this.Stop.Location = new System.Drawing.Point(647, 408);
            this.Stop.MouseState = MaterialSkin.MouseState.HOVER;
            this.Stop.Name = "Stop";
            this.Stop.Primary = true;
            this.Stop.Size = new System.Drawing.Size(152, 46);
            this.Stop.TabIndex = 15;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Tray
            // 
            this.Tray.BalloonTipText = "Click To Open";
            this.Tray.BalloonTipTitle = "WeebIRC Backend";
            this.Tray.Icon = ((System.Drawing.Icon)(resources.GetObject("Tray.Icon")));
            this.Tray.Text = "WeebIRC Backend";
            this.Tray.Visible = true;
            this.Tray.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Tray_MouseClick);
            // 
            // materialLabel2
            // 
            this.materialLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(287, 109);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(249, 19);
            this.materialLabel2.TabIndex = 22;
            this.materialLabel2.Text = "Click the logo to open the interface!";
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(833, 480);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.State);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.About);
            this.Controls.Add(this.Debug);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.pictureBox2);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Home";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WeebIRC Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Home_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Home_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MaterialSkin.Controls.MaterialLabel State;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialRaisedButton About;
        private MaterialSkin.Controls.MaterialRaisedButton Debug;
        private MaterialSkin.Controls.MaterialRaisedButton Start;
        private MaterialSkin.Controls.MaterialRaisedButton Stop;
        private System.Windows.Forms.NotifyIcon Tray;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
    }
}

