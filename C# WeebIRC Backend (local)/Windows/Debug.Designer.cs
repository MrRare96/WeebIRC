namespace WeebIRCServerTray
{
    partial class Debug
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debug));
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.SaveToLog = new MaterialSkin.Controls.MaterialCheckBox();
            this.DebugWindow = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
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
            this.pictureBox2.TabIndex = 9;
            this.pictureBox2.TabStop = false;
            // 
            // SaveToLog
            // 
            this.SaveToLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveToLog.AutoSize = true;
            this.SaveToLog.Depth = 0;
            this.SaveToLog.Font = new System.Drawing.Font("Roboto", 10F);
            this.SaveToLog.Location = new System.Drawing.Point(22, 488);
            this.SaveToLog.Margin = new System.Windows.Forms.Padding(0);
            this.SaveToLog.MouseLocation = new System.Drawing.Point(-1, -1);
            this.SaveToLog.MouseState = MaterialSkin.MouseState.HOVER;
            this.SaveToLog.Name = "SaveToLog";
            this.SaveToLog.Ripple = true;
            this.SaveToLog.Size = new System.Drawing.Size(106, 30);
            this.SaveToLog.TabIndex = 10;
            this.SaveToLog.Text = "Save To Log";
            this.SaveToLog.UseVisualStyleBackColor = true;
            this.SaveToLog.CheckedChanged += new System.EventHandler(this.SaveToLog_CheckedChanged);
            // 
            // DebugWindow
            // 
            this.DebugWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DebugWindow.Font = new System.Drawing.Font("Roboto Condensed", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DebugWindow.Location = new System.Drawing.Point(22, 81);
            this.DebugWindow.Margin = new System.Windows.Forms.Padding(0);
            this.DebugWindow.Name = "DebugWindow";
            this.DebugWindow.ReadOnly = true;
            this.DebugWindow.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.DebugWindow.Size = new System.Drawing.Size(585, 393);
            this.DebugWindow.TabIndex = 11;
            this.DebugWindow.Text = "Welcome to the debug heaven :D This wall of text is a feast to read for developer" +
    "s with no life... so keep at it :D";
            // 
            // Debug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 527);
            this.Controls.Add(this.DebugWindow);
            this.Controls.Add(this.SaveToLog);
            this.Controls.Add(this.pictureBox2);
            this.Name = "Debug";
            this.Text = "Debug";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Debug_FormClosing);
            this.Load += new System.EventHandler(this.Debug_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private MaterialSkin.Controls.MaterialCheckBox SaveToLog;
        private System.Windows.Forms.RichTextBox DebugWindow;
    }
}