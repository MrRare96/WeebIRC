namespace WeebIRC
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.IpAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ircPort = new System.Windows.Forms.TextBox();
            this.userName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ircChannel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.connect = new System.Windows.Forms.Button();
            this.ircTabs = new System.Windows.Forms.TabControl();
            this.connectPage = new System.Windows.Forms.TabPage();
            this.autoJoin = new System.Windows.Forms.CheckBox();
            this.isDefault = new System.Windows.Forms.CheckBox();
            this.chat = new System.Windows.Forms.TabPage();
            this.send = new System.Windows.Forms.Button();
            this.chatInput = new System.Windows.Forms.TextBox();
            this.chatOutput = new System.Windows.Forms.RichTextBox();
            this.download = new System.Windows.Forms.TabPage();
            this.StartDownload = new System.Windows.Forms.Button();
            this.clearDlQue = new System.Windows.Forms.Button();
            this.abortDL = new System.Windows.Forms.Button();
            this.downloadList = new System.Windows.Forms.ListView();
            this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Progress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Speed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PackNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BotName1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.niblBatch = new System.Windows.Forms.TabPage();
            this.dlSelected = new System.Windows.Forms.Button();
            this.botSelector = new System.Windows.Forms.ComboBox();
            this.Search = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.botSearchInput = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.batchFolderName = new System.Windows.Forms.TextBox();
            this.QueBatch = new System.Windows.Forms.Button();
            this.animeSearchResult = new System.Windows.Forms.ListView();
            this.epFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.packNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BotName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Settings = new System.Windows.Forms.TabPage();
            this.folderPerBatch = new System.Windows.Forms.CheckBox();
            this.folderForEachFile = new System.Windows.Forms.CheckBox();
            this.setDlFolder = new System.Windows.Forms.Button();
            this.downloadFolderLoc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Debug = new System.Windows.Forms.TabPage();
            this.debugWindow = new System.Windows.Forms.RichTextBox();
            this.about = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.performanceCounter1 = new System.Diagnostics.PerformanceCounter();
            this.ircTabs.SuspendLayout();
            this.connectPage.SuspendLayout();
            this.chat.SuspendLayout();
            this.download.SuspendLayout();
            this.niblBatch.SuspendLayout();
            this.Settings.SuspendLayout();
            this.Debug.SuspendLayout();
            this.about.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter1)).BeginInit();
            this.SuspendLayout();
            // 
            // IpAddress
            // 
            this.IpAddress.Location = new System.Drawing.Point(405, 125);
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.Size = new System.Drawing.Size(229, 20);
            this.IpAddress.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(402, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP/ADDRESS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(402, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "PORT";
            // 
            // ircPort
            // 
            this.ircPort.Location = new System.Drawing.Point(405, 164);
            this.ircPort.Name = "ircPort";
            this.ircPort.Size = new System.Drawing.Size(229, 20);
            this.ircPort.TabIndex = 3;
            // 
            // userName
            // 
            this.userName.Location = new System.Drawing.Point(405, 203);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(229, 20);
            this.userName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(402, 187);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "USERNAME";
            // 
            // ircChannel
            // 
            this.ircChannel.Location = new System.Drawing.Point(405, 242);
            this.ircChannel.Name = "ircChannel";
            this.ircChannel.Size = new System.Drawing.Size(229, 20);
            this.ircChannel.TabIndex = 7;
            this.ircChannel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ircChannel_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(402, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "CHANNEL";
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(405, 291);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(229, 23);
            this.connect.TabIndex = 8;
            this.connect.Text = "Connect!";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // ircTabs
            // 
            this.ircTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ircTabs.Controls.Add(this.connectPage);
            this.ircTabs.Controls.Add(this.chat);
            this.ircTabs.Controls.Add(this.download);
            this.ircTabs.Controls.Add(this.niblBatch);
            this.ircTabs.Controls.Add(this.Settings);
            this.ircTabs.Controls.Add(this.Debug);
            this.ircTabs.Controls.Add(this.about);
            this.ircTabs.Location = new System.Drawing.Point(1, 0);
            this.ircTabs.Name = "ircTabs";
            this.ircTabs.SelectedIndex = 0;
            this.ircTabs.Size = new System.Drawing.Size(1093, 487);
            this.ircTabs.TabIndex = 9;
            // 
            // connectPage
            // 
            this.connectPage.Controls.Add(this.autoJoin);
            this.connectPage.Controls.Add(this.isDefault);
            this.connectPage.Controls.Add(this.label1);
            this.connectPage.Controls.Add(this.connect);
            this.connectPage.Controls.Add(this.IpAddress);
            this.connectPage.Controls.Add(this.ircChannel);
            this.connectPage.Controls.Add(this.label2);
            this.connectPage.Controls.Add(this.label4);
            this.connectPage.Controls.Add(this.ircPort);
            this.connectPage.Controls.Add(this.userName);
            this.connectPage.Controls.Add(this.label3);
            this.connectPage.Location = new System.Drawing.Point(4, 22);
            this.connectPage.Name = "connectPage";
            this.connectPage.Padding = new System.Windows.Forms.Padding(3);
            this.connectPage.Size = new System.Drawing.Size(1085, 461);
            this.connectPage.TabIndex = 0;
            this.connectPage.Text = "Connect";
            this.connectPage.UseVisualStyleBackColor = true;
            // 
            // autoJoin
            // 
            this.autoJoin.AutoSize = true;
            this.autoJoin.Location = new System.Drawing.Point(510, 268);
            this.autoJoin.Name = "autoJoin";
            this.autoJoin.Size = new System.Drawing.Size(124, 17);
            this.autoJoin.TabIndex = 10;
            this.autoJoin.Text = "Auto Join on Launch";
            this.autoJoin.UseVisualStyleBackColor = true;
            this.autoJoin.CheckedChanged += new System.EventHandler(this.autoJoin_CheckedChanged);
            // 
            // isDefault
            // 
            this.isDefault.AutoSize = true;
            this.isDefault.Location = new System.Drawing.Point(405, 268);
            this.isDefault.Name = "isDefault";
            this.isDefault.Size = new System.Drawing.Size(94, 17);
            this.isDefault.TabIndex = 9;
            this.isDefault.Text = "Set As Default";
            this.isDefault.UseVisualStyleBackColor = true;
            this.isDefault.CheckedChanged += new System.EventHandler(this.isDefault_CheckedChanged);
            // 
            // chat
            // 
            this.chat.Controls.Add(this.send);
            this.chat.Controls.Add(this.chatInput);
            this.chat.Controls.Add(this.chatOutput);
            this.chat.Location = new System.Drawing.Point(4, 22);
            this.chat.Name = "chat";
            this.chat.Padding = new System.Windows.Forms.Padding(3);
            this.chat.Size = new System.Drawing.Size(1085, 461);
            this.chat.TabIndex = 1;
            this.chat.Text = "Chat";
            this.chat.UseVisualStyleBackColor = true;
            // 
            // send
            // 
            this.send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.send.Location = new System.Drawing.Point(1009, 433);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(75, 23);
            this.send.TabIndex = 2;
            this.send.Text = "Send";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // chatInput
            // 
            this.chatInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatInput.Location = new System.Drawing.Point(6, 433);
            this.chatInput.Name = "chatInput";
            this.chatInput.Size = new System.Drawing.Size(996, 20);
            this.chatInput.TabIndex = 1;
            this.chatInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatInput_KeyDown);
            // 
            // chatOutput
            // 
            this.chatOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatOutput.Location = new System.Drawing.Point(0, 0);
            this.chatOutput.Name = "chatOutput";
            this.chatOutput.Size = new System.Drawing.Size(1084, 427);
            this.chatOutput.TabIndex = 0;
            this.chatOutput.Text = "";
            // 
            // download
            // 
            this.download.Controls.Add(this.StartDownload);
            this.download.Controls.Add(this.clearDlQue);
            this.download.Controls.Add(this.abortDL);
            this.download.Controls.Add(this.downloadList);
            this.download.Location = new System.Drawing.Point(4, 22);
            this.download.Name = "download";
            this.download.Size = new System.Drawing.Size(1085, 461);
            this.download.TabIndex = 2;
            this.download.Text = "Download";
            this.download.UseVisualStyleBackColor = true;
            // 
            // StartDownload
            // 
            this.StartDownload.Location = new System.Drawing.Point(960, 430);
            this.StartDownload.Name = "StartDownload";
            this.StartDownload.Size = new System.Drawing.Size(115, 23);
            this.StartDownload.TabIndex = 3;
            this.StartDownload.Text = "Start Download";
            this.StartDownload.UseVisualStyleBackColor = true;
            this.StartDownload.Click += new System.EventHandler(this.StartDownload_Click);
            // 
            // clearDlQue
            // 
            this.clearDlQue.Location = new System.Drawing.Point(0, 430);
            this.clearDlQue.Name = "clearDlQue";
            this.clearDlQue.Size = new System.Drawing.Size(157, 23);
            this.clearDlQue.TabIndex = 2;
            this.clearDlQue.Text = "Clear Download Que";
            this.clearDlQue.UseVisualStyleBackColor = true;
            this.clearDlQue.Click += new System.EventHandler(this.clearDlQue_Click);
            // 
            // abortDL
            // 
            this.abortDL.Location = new System.Drawing.Point(787, 430);
            this.abortDL.Name = "abortDL";
            this.abortDL.Size = new System.Drawing.Size(157, 23);
            this.abortDL.TabIndex = 1;
            this.abortDL.Text = "Stop Download";
            this.abortDL.UseVisualStyleBackColor = true;
            // 
            // downloadList
            // 
            this.downloadList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.File,
            this.Progress,
            this.Speed,
            this.PackNumber,
            this.BotName1});
            this.downloadList.FullRowSelect = true;
            this.downloadList.GridLines = true;
            this.downloadList.Location = new System.Drawing.Point(0, 0);
            this.downloadList.Name = "downloadList";
            this.downloadList.Size = new System.Drawing.Size(1082, 424);
            this.downloadList.TabIndex = 0;
            this.downloadList.UseCompatibleStateImageBehavior = false;
            this.downloadList.View = System.Windows.Forms.View.Details;
            this.downloadList.DoubleClick += new System.EventHandler(this.downloadList_DoubleClick);
            // 
            // File
            // 
            this.File.Text = "File";
            this.File.Width = 411;
            // 
            // Progress
            // 
            this.Progress.Text = "Progress";
            // 
            // Speed
            // 
            this.Speed.Text = "Speed";
            this.Speed.Width = 98;
            // 
            // PackNumber
            // 
            this.PackNumber.Text = "Pack Number";
            this.PackNumber.Width = 105;
            // 
            // BotName1
            // 
            this.BotName1.Text = "Bot ";
            this.BotName1.Width = 106;
            // 
            // niblBatch
            // 
            this.niblBatch.Controls.Add(this.dlSelected);
            this.niblBatch.Controls.Add(this.botSelector);
            this.niblBatch.Controls.Add(this.Search);
            this.niblBatch.Controls.Add(this.label8);
            this.niblBatch.Controls.Add(this.botSearchInput);
            this.niblBatch.Controls.Add(this.label7);
            this.niblBatch.Controls.Add(this.label6);
            this.niblBatch.Controls.Add(this.batchFolderName);
            this.niblBatch.Controls.Add(this.QueBatch);
            this.niblBatch.Controls.Add(this.animeSearchResult);
            this.niblBatch.Location = new System.Drawing.Point(4, 22);
            this.niblBatch.Name = "niblBatch";
            this.niblBatch.Padding = new System.Windows.Forms.Padding(3);
            this.niblBatch.Size = new System.Drawing.Size(1085, 461);
            this.niblBatch.TabIndex = 4;
            this.niblBatch.Text = "NiblBatch (Anime)";
            this.niblBatch.UseVisualStyleBackColor = true;
            // 
            // dlSelected
            // 
            this.dlSelected.Location = new System.Drawing.Point(800, 432);
            this.dlSelected.Name = "dlSelected";
            this.dlSelected.Size = new System.Drawing.Size(128, 23);
            this.dlSelected.TabIndex = 9;
            this.dlSelected.Text = "Download Selected";
            this.dlSelected.UseVisualStyleBackColor = true;
            this.dlSelected.Click += new System.EventHandler(this.dlSelected_Click);
            // 
            // botSelector
            // 
            this.botSelector.FormattingEnabled = true;
            this.botSelector.Items.AddRange(new object[] {
            "Search All Bots"});
            this.botSelector.Location = new System.Drawing.Point(11, 63);
            this.botSelector.Name = "botSelector";
            this.botSelector.Size = new System.Drawing.Size(1068, 21);
            this.botSelector.TabIndex = 8;
            this.botSelector.SelectedIndexChanged += new System.EventHandler(this.botSelector_SelectedIndexChanged);
            // 
            // Search
            // 
            this.Search.Location = new System.Drawing.Point(1000, 19);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(75, 20);
            this.Search.TabIndex = 2;
            this.Search.Text = "Search";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(238, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Select Specific Bot (Needed for batch download)";
            // 
            // botSearchInput
            // 
            this.botSearchInput.Location = new System.Drawing.Point(11, 19);
            this.botSearchInput.Name = "botSearchInput";
            this.botSearchInput.Size = new System.Drawing.Size(983, 20);
            this.botSearchInput.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 420);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(355, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Batch Folder Name (Folder will have name from search query if left empty!)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Search Anime";
            // 
            // batchFolderName
            // 
            this.batchFolderName.Location = new System.Drawing.Point(11, 435);
            this.batchFolderName.Name = "batchFolderName";
            this.batchFolderName.Size = new System.Drawing.Size(573, 20);
            this.batchFolderName.TabIndex = 5;
            // 
            // QueBatch
            // 
            this.QueBatch.Location = new System.Drawing.Point(934, 432);
            this.QueBatch.Name = "QueBatch";
            this.QueBatch.Size = new System.Drawing.Size(128, 23);
            this.QueBatch.TabIndex = 4;
            this.QueBatch.Text = "Download All";
            this.QueBatch.UseVisualStyleBackColor = true;
            this.QueBatch.Click += new System.EventHandler(this.QueBatch_Click);
            // 
            // animeSearchResult
            // 
            this.animeSearchResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animeSearchResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.epFileName,
            this.Size,
            this.packNum,
            this.BotName});
            this.animeSearchResult.FullRowSelect = true;
            this.animeSearchResult.GridLines = true;
            this.animeSearchResult.Location = new System.Drawing.Point(11, 92);
            this.animeSearchResult.Name = "animeSearchResult";
            this.animeSearchResult.Size = new System.Drawing.Size(1068, 324);
            this.animeSearchResult.TabIndex = 3;
            this.animeSearchResult.UseCompatibleStateImageBehavior = false;
            this.animeSearchResult.View = System.Windows.Forms.View.Details;
            // 
            // epFileName
            // 
            this.epFileName.Text = "File Name";
            this.epFileName.Width = 376;
            // 
            // Size
            // 
            this.Size.Text = "Size  (MB)";
            this.Size.Width = 80;
            // 
            // packNum
            // 
            this.packNum.Text = "Pack Number";
            this.packNum.Width = 89;
            // 
            // BotName
            // 
            this.BotName.Text = "Bot Name";
            this.BotName.Width = 111;
            // 
            // Settings
            // 
            this.Settings.Controls.Add(this.folderPerBatch);
            this.Settings.Controls.Add(this.folderForEachFile);
            this.Settings.Controls.Add(this.setDlFolder);
            this.Settings.Controls.Add(this.downloadFolderLoc);
            this.Settings.Controls.Add(this.label5);
            this.Settings.Location = new System.Drawing.Point(4, 22);
            this.Settings.Name = "Settings";
            this.Settings.Padding = new System.Windows.Forms.Padding(3);
            this.Settings.Size = new System.Drawing.Size(1085, 461);
            this.Settings.TabIndex = 3;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // folderPerBatch
            // 
            this.folderPerBatch.AutoCheck = false;
            this.folderPerBatch.AutoSize = true;
            this.folderPerBatch.Location = new System.Drawing.Point(11, 73);
            this.folderPerBatch.Name = "folderPerBatch";
            this.folderPerBatch.Size = new System.Drawing.Size(105, 17);
            this.folderPerBatch.TabIndex = 4;
            this.folderPerBatch.Text = "Folder Per Batch";
            this.folderPerBatch.UseVisualStyleBackColor = true;
            this.folderPerBatch.Click += new System.EventHandler(this.folderPerBatch_Click);
            // 
            // folderForEachFile
            // 
            this.folderForEachFile.AutoCheck = false;
            this.folderForEachFile.AutoSize = true;
            this.folderForEachFile.Location = new System.Drawing.Point(11, 50);
            this.folderForEachFile.Name = "folderForEachFile";
            this.folderForEachFile.Size = new System.Drawing.Size(120, 17);
            this.folderForEachFile.TabIndex = 3;
            this.folderForEachFile.Text = "Folder For Each File";
            this.folderForEachFile.UseVisualStyleBackColor = true;
            this.folderForEachFile.Click += new System.EventHandler(this.folderForEachFile_Click);
            // 
            // setDlFolder
            // 
            this.setDlFolder.Location = new System.Drawing.Point(538, 22);
            this.setDlFolder.Name = "setDlFolder";
            this.setDlFolder.Size = new System.Drawing.Size(101, 23);
            this.setDlFolder.TabIndex = 2;
            this.setDlFolder.Text = "Set Folder";
            this.setDlFolder.UseVisualStyleBackColor = true;
            this.setDlFolder.Click += new System.EventHandler(this.setDlFolder_Click);
            // 
            // downloadFolderLoc
            // 
            this.downloadFolderLoc.Location = new System.Drawing.Point(11, 24);
            this.downloadFolderLoc.Name = "downloadFolderLoc";
            this.downloadFolderLoc.Size = new System.Drawing.Size(496, 20);
            this.downloadFolderLoc.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Download Folder:";
            // 
            // Debug
            // 
            this.Debug.Controls.Add(this.debugWindow);
            this.Debug.Location = new System.Drawing.Point(4, 22);
            this.Debug.Name = "Debug";
            this.Debug.Padding = new System.Windows.Forms.Padding(3);
            this.Debug.Size = new System.Drawing.Size(1085, 461);
            this.Debug.TabIndex = 5;
            this.Debug.Text = "Debug";
            this.Debug.UseVisualStyleBackColor = true;
            // 
            // debugWindow
            // 
            this.debugWindow.Location = new System.Drawing.Point(-3, 3);
            this.debugWindow.Name = "debugWindow";
            this.debugWindow.Size = new System.Drawing.Size(1082, 458);
            this.debugWindow.TabIndex = 0;
            this.debugWindow.Text = "";
            // 
            // about
            // 
            this.about.Controls.Add(this.richTextBox1);
            this.about.Location = new System.Drawing.Point(4, 22);
            this.about.Name = "about";
            this.about.Padding = new System.Windows.Forms.Padding(3);
            this.about.Size = new System.Drawing.Size(1085, 461);
            this.about.TabIndex = 6;
            this.about.Text = "About";
            this.about.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(242, 21);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(582, 420);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 487);
            this.Controls.Add(this.ircTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "WeebIRC";
            this.ircTabs.ResumeLayout(false);
            this.connectPage.ResumeLayout(false);
            this.connectPage.PerformLayout();
            this.chat.ResumeLayout(false);
            this.chat.PerformLayout();
            this.download.ResumeLayout(false);
            this.niblBatch.ResumeLayout(false);
            this.niblBatch.PerformLayout();
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            this.Debug.ResumeLayout(false);
            this.about.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox IpAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ircPort;
        private System.Windows.Forms.TextBox userName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ircChannel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.TabControl ircTabs;
        private System.Windows.Forms.TabPage connectPage;
        private System.Windows.Forms.TabPage chat;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.TextBox chatInput;
        private System.Windows.Forms.RichTextBox chatOutput;
        private System.Windows.Forms.TabPage download;
        private System.Windows.Forms.ListView downloadList;
        private System.Windows.Forms.ColumnHeader File;
        private System.Windows.Forms.ColumnHeader Progress;
        private System.Windows.Forms.ColumnHeader Speed;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.CheckBox folderPerBatch;
        private System.Windows.Forms.CheckBox folderForEachFile;
        private System.Windows.Forms.Button setDlFolder;
        private System.Windows.Forms.TextBox downloadFolderLoc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabPage niblBatch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox batchFolderName;
        private System.Windows.Forms.Button QueBatch;
        private System.Windows.Forms.ListView animeSearchResult;
        private System.Windows.Forms.ColumnHeader epFileName;
        private System.Windows.Forms.ColumnHeader Size;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox botSearchInput;
        private System.Windows.Forms.ComboBox botSelector;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ColumnHeader packNum;
        private System.Windows.Forms.TabPage Debug;
        private System.Windows.Forms.RichTextBox debugWindow;
        private System.Windows.Forms.Button abortDL;
        private System.Diagnostics.PerformanceCounter performanceCounter1;
        private System.Windows.Forms.Button clearDlQue;
        private System.Windows.Forms.ColumnHeader BotName;
        private System.Windows.Forms.ColumnHeader PackNumber;
        private System.Windows.Forms.ColumnHeader BotName1;
        private System.Windows.Forms.Button dlSelected;
        private System.Windows.Forms.Button StartDownload;
        private System.Windows.Forms.TabPage about;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox autoJoin;
        private System.Windows.Forms.CheckBox isDefault;
    }
}

