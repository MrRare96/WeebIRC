using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using SimpleIRCLib;

namespace WeebIRC
{
   

    public partial class Form1 : Form
    {

        //global values
        public SimpleIRC simpleIrc = new SimpleIRC();

        public string downloadFolder = "";
        public string batchDownloadFolder = "";
        public string fileLocation = "";
        public int dlProgress = 0;

        public bool perDownloadAFolder = false;
        public bool perBatchAFolder = false;
        public bool batchMode = false;
        public bool connected = false;
        public bool StopProgram = false;

        //form init
        public Form1()
        {
            InitializeComponent();

            setDownloadPath();

            createSettingsConf();

            ircTabs.TabPages[1].Enabled = false;
            ircTabs.TabPages[2].Enabled = false;
            ircTabs.TabPages[3].Enabled = false;

            botSelector.SelectedIndex = 0;

            IpAddress.Text = getDefaultIP();
            ircPort.Text = getDefaultPORT();
            userName.Text = getDefaultUsername();
            ircChannel.Text = getDefaultChannel();

            folderPerBatch.Checked = getFolderPerBatch();
            folderForEachFile.Checked = getFolderForEachFile();

            autoJoin.Checked = getAutoJoin();

            if (autoJoin.Checked)
            {
                
                runClient();
            }
        }


        //Stop everything
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopProgram = true;
            simpleIrc.stopClient();
        }

        //-----------Connect tab--------
        

        //run client
        private void runClient()
        {

            if (isDefault.Checked)
            {
              
                setDefaultConnect(IpAddress.Text, ircPort.Text, userName.Text, ircChannel.Text, getAutoJoin());
            }
            string ip = getDefaultIP();
            int port = Int32.Parse(getDefaultPORT());
            string username = getDefaultUsername();
            string channel = getDefaultChannel();

            simpleIrc.setupIrc(IpAddress.Text, Int32.Parse(ircPort.Text), userName.Text, "", ircChannel.Text, AppendIrcOutput);
            simpleIrc.setDownloadStatusChangeCallback(downloadStatusCallback);
            simpleIrc.setCustomDownloadDir(downloadFolder);
            simpleIrc.setDebugCallback(AppendDebug);
            simpleIrc.startClient();
            
            connected = simpleIrc.isClientRunning();
            ircTabs.TabPages[1].Enabled = true;
            ircTabs.TabPages[2].Enabled = true;
            ircTabs.TabPages[3].Enabled = true;
            ircTabs.SelectedIndex = 1;
        }
        
        //starts client on buttonClick
        private void connect_Click(object sender, EventArgs e)
        {
            runClient();
        }

        //start client on enterpress after defining channel
        private void ircChannel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((IpAddress.Text != "" || IpAddress.Text != null) && (ircPort.Text != "" || ircPort.Text != null) && (userName.Text != "" || userName.Text != null))
                {
                    runClient();
                }
            }
        }

        //save current connection to settings.conf
        private void isDefault_CheckedChanged(object sender, EventArgs e)
        {
            AppendDebug("default checked");
            if (isDefault.Checked)
            {
                AppendDebug("default updated");
                setDefaultConnect(IpAddress.Text, ircPort.Text, userName.Text, ircChannel.Text, autoJoin.Checked);
            }
        }

        private void autoJoin_CheckedChanged(object sender, EventArgs e)
        {
            if (autoJoin.Checked)
            {
                AppendDebug("default updated");
                setDefaultConnect(IpAddress.Text, ircPort.Text, userName.Text, ircChannel.Text, autoJoin.Checked);
            }
        }

        //-----------Chat tab--------

        //sends message to irc channel(s) on send button click
        private void send_Click(object sender, EventArgs e)
        {
            if(chatInput.Text != "" || chatInput.Text != null)
            {
                if(chatInput.Text.Contains("xdcc send"))
                {
                    batchMode = false;
                }
                simpleIrc.sendMessage(chatInput.Text);
            }
            
        }

        //Add irc output to richTextBox
        public void AppendIrcOutput(string username,  string input)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string,string>(AppendIrcOutput), new object[] { username, input });
                return;
            }
            chatOutput.Text += username + " : " + input + "\n";
            chatOutput.SelectionStart = chatOutput.Text.Length;
            chatOutput.ScrollToCaret();
        }


        //sends message to irc channel(s) on enter press
        private void chatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (chatInput.Text != "" || chatInput.Text != null)
                {
                    simpleIrc.sendMessage(chatInput.Text);
                }
            }
        }

        //--------Downloads Tab---------

        //add file to download que/list
        private void downloadStatusCallback(string[] downloadStatus) //see below for definition of each index in this array
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action<string[]>(downloadStatusCallback), new object[] { downloadStatus });
                return;
            }

           
            int progress = Int32.Parse(downloadStatus[7]);
            dlProgress = Int32.Parse(downloadStatus[7]);
            try
            {
                int counter = downloadList.FindItemWithText(downloadStatus[1]).Index;
               
                if (progress == 0)
                {
                    downloadList.Items[counter].SubItems[1].Text = "WAITING";
                    downloadList.Items[counter].SubItems[2].Text = "";
                } else if(progress < 95)
                {
                    downloadList.Items[counter].SubItems[1].Text = "DONE";
                    downloadList.Items[counter].SubItems[2].Text = "";
                }
                else
                {
                    downloadList.Items[counter].SubItems[1].Text = downloadStatus[7] + "%";
                    downloadList.Items[counter].SubItems[2].Text = downloadStatus[9] + " kb/s";
                }
            }
            catch (Exception e)
            {
                ListViewItem item = new ListViewItem(downloadStatus[1]);
                if (System.IO.File.Exists(fileLocation))
                {
                    item.SubItems.Add("DONE");
                }
                else
                {
                    item.SubItems.Add("WAITING");
                }


                item.SubItems.Add("");
                item.SubItems.Add(downloadStatus[5]);
                item.SubItems.Add(downloadStatus[6]);
                downloadList.Items.Add(item);
            }

        }

        public void appendToDownloadList(string fileName, string packNum, string botName, string fileLocation)
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action<string, string, string, string>(appendToDownloadList), new object[] { fileName, packNum, botName, fileLocation });
                return;
            }


            ListViewItem item = new ListViewItem(fileName);
            if (System.IO.File.Exists(fileLocation))
            {
                item.SubItems.Add("DONE");
            }
            else
            {
                item.SubItems.Add("Waiting");
            }

            item.SubItems.Add("");
            item.SubItems.Add(fileLocation);
            item.SubItems.Add(packNum);
            item.SubItems.Add(botName);
            downloadList.Items.Add(item);

        }



        //get file location from downloadlist by filename
        public string getFileLocation(string fileName)
        {

            string fileLoc = "";
            try
            {

                if (InvokeRequired)
                {
                    return (string)this.Invoke(new Func<String>(() => getFileLocation(fileName)));
                }
                int index = downloadList.FindItemWithText(fileName).Index;
                fileLoc = downloadList.Items[index].SubItems[3].Text;

            }
            catch (Exception E)
            {

            }

            return fileLoc;

        }



        public void updateFileStatusbyPack(string pack)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(updateFileStatusbyPack), new object[] { pack });
                return;
            }

            try
            {
                int packIndex = downloadList.FindItemWithText(pack, true, 0).Index;

                downloadList.Items[packIndex].SubItems[1].Text = "FAILED";
                downloadList.Items[packIndex].SubItems[2].Text = "";
            }
            catch (Exception e)
            {
                AppendDebug("couldnt update pack: " + pack);
                AppendDebug(e.ToString());
            }

        }

        //get next pack number on downloadlist from which still needs to be downloaded
        public string getNextPackNum()
        {

            if (InvokeRequired)
            {
                return (string)this.Invoke(new Func<String>(() => getNextPackNum()));
            }


            string packNum = "null";
            foreach (ListViewItem item in downloadList.Items)
            {
                if (item.SubItems[1].Text == "Waiting")
                {
                    packNum = item.SubItems[4].Text;
                    break;
                }
                else
                {
                    packNum = "null";
                }
            }

            return packNum;

        }

        //gets botsname from file that still needs to be downloaded
        public string getNextBot()
        {
            if (InvokeRequired)
            {
                return (string)this.Invoke(new Func<String>(() => getNextBot()));
            }


            string bot = "null";
            foreach (ListViewItem item in downloadList.Items)
            {
                if (item.SubItems[1].Text == "Waiting")
                {
                    bot = item.SubItems[5].Text;
                    break;
                }
                else
                {
                    bot = "null";
                }
            }

            return bot;

        }

        //start download every file in downloadList(as long it doesnt exist locally)
        private void StartDownload_Click(object sender, EventArgs e)
        {
            if (simpleIrc.isClientRunning())
            {
                string bot = botSelector.SelectedItem.ToString();
                Thread batchDownloader = new Thread(() => startBatchDownload());
                batchDownloader.Start();
            }
            else
            {
                MessageBox.Show("You are not connected to an irc channel :X");
            }
        }

        //code to be able to download every file in downloadList
        private void startBatchDownload()
        {

            string previousXdccMessage = "";
            int timeOutCounter = 0;
            while (true)
            {
                
                if (dlProgress >= 95 || dlProgress == 0)
                {
                    string packNum = getNextPackNum();
                    string bot = getNextBot();
                    if (packNum != "null")
                    {
                        string createXDCCMessage = "/msg " + bot + " xdcc send #" + packNum;

                        if (timeOutCounter < 2)
                        {
                            simpleIrc.sendMessage(createXDCCMessage);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            updateFileStatusbyPack(packNum);
                            AppendDebug("PackNum TimedOut: Couldn't download: " + packNum);
                            timeOutCounter = 0;
                        }

                        if (previousXdccMessage == createXDCCMessage)
                        {
                            timeOutCounter++;
                        }

                        previousXdccMessage = createXDCCMessage;                        
                    }
                    else
                    {
                        AppendDebug("Done with batch files :D");
                        break;
                    }

                    if (dlProgress>= 95)
                    {
                        AppendDebug("Stopping batch loop :D");
                        break;
                    }

                }

                Thread.Sleep(1000);
            }

        }


        

        //remove all items from downloadList
        private void clearDlQue_Click(object sender, EventArgs e)
        {
            downloadList.Items.Clear();
        }

        //open file with default assigned program on double click in downloadList
        private void downloadList_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = downloadList.SelectedItems[0];

            if (item.SubItems[1].Text != "Waiting" && item.SubItems[1].Text != "FAILED" && item.SubItems[1].Text != "ABORTED" && System.IO.File.Exists(item.SubItems[3].Text))
            {
                try
                {
                    Process.Start(item.SubItems[3].Text);
                }
                catch (Exception ex)
                {
                    AppendDebug(item.SubItems[3].Text);
                    AppendDebug(ex.ToString());
                }
            }
            else
            {
                AppendDebug("File does not exist");
            }
        }       

        //---------- Nibl Search Tab -----------

        //start search for anime on nibl
        private void Search_Click(object sender, EventArgs e)
        {
            animeSearchResult.Items.Clear();

            string searchInput = botSearchInput.Text;
            string bot = botSelector.SelectedItem.ToString();

            if(bot != "Search All Bots" && searchInput != "" && bot != null && searchInput != null)
            {
                Thread searchBot = new Thread(() => searchAnime(this, searchInput, bot));
                searchBot.Start();
            } else
            {
                Thread searchBot = new Thread(() => searchAnime(this, searchInput, ""));
                searchBot.Start();
            }
        }


        //Add Bot to botSelector
        public void addBot(string botname)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(addBot), new object[] { botname });
                return;
            }

            if (botSelector.FindString(botname) < 0)
            {
                botSelector.Items.Add(botname);
            }
        }

        //on bot select: research for same search query on bot specific
        private void botSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            animeSearchResult.Items.Clear();

            string searchInput = botSearchInput.Text;
            string bot = botSelector.SelectedItem.ToString();

            Thread searchBot = new Thread(() => searchAnime(this, searchInput, bot));
            searchBot.Start();
        }

        //add search results to list
        public void setAnimeSearchResult(string fileName, string size, string packNum, string botName)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string, string, string, string>(setAnimeSearchResult), new object[] { fileName, size, packNum, botName });
                return;
            }

            ListViewItem item = new ListViewItem(fileName);
            item.SubItems.Add(size);
            item.SubItems.Add(packNum);
            item.SubItems.Add(botName);

            animeSearchResult.Items.Add(item);
        }


        //thread code for searching anime while not holding gui
        private void searchAnime(Form1 form, string SearchInput, string bot)
        {

            SearchInput = SearchInput.Replace(' ', '+').ToLower();

            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                if(bot == "" || bot == null)
                {
                    htmlCode = client.DownloadString("http://nibl.co.uk/bots.php?search=" + SearchInput);
                } else
                {
                    htmlCode = client.DownloadString("http://nibl.co.uk/bots.php?search=" + SearchInput + "&bot=" + bot + "&searchthis=Search+this+bot");
                }
                if (htmlCode != null || htmlCode != "")
                {
                    int posOfBotPart = 3;
                    string searchQuery = "botlistitem";

                    string lastFile = "";

                    while (posOfBotPart > 0 && !htmlCode.Contains("No packs found") && !htmlCode.Contains("Search query too vague"))
                    {
                        posOfBotPart = htmlCode.IndexOf(searchQuery);
                        htmlCode = htmlCode.Substring(posOfBotPart + searchQuery.Length);
                        string partOfHtmlCode = htmlCode;

                        int startBotName = partOfHtmlCode.IndexOf("botname\">");
                        string botName = partOfHtmlCode.Substring(startBotName + 9);
                        int endBotName = botName.IndexOf("<a");
                        botName = botName.Substring(0, endBotName);

                        form.addBot(botName);

                        int startFileName= partOfHtmlCode.IndexOf("filename\">");
                        string fileName = partOfHtmlCode.Substring(startFileName + 10);
                        int endFileName = fileName.IndexOf("<a");
                        fileName = fileName.Substring(0, endFileName).Trim();

                        if(fileName == lastFile)
                        {
                            break;
                        }
                        

                        int startPackNum = partOfHtmlCode.IndexOf("packnumber\">");
                        string packNum = partOfHtmlCode.Substring(startPackNum + 12);
                        int endPackNum = packNum.IndexOf("</td>");
                        packNum = packNum.Substring(0, endPackNum);

                        int startFileSize = partOfHtmlCode.IndexOf("filesize\">");
                        string fileSize = partOfHtmlCode.Substring(startFileSize + 10);
                        int endFileSize = fileSize.IndexOf("</td>");
                        fileSize = fileSize.Substring(0, endFileSize);

                        lastFile = fileName;

                        form.setAnimeSearchResult(fileName, fileSize, packNum, botName);
                    }
                }
            }
        }

        //append every file on animeSearchList to downloadList
        private void QueBatch_Click(object sender, EventArgs e)
        {
            if(botSelector.SelectedItem.ToString() != "Search All Bots")
            {
                batchMode = true;

                foreach (ListViewItem item in animeSearchResult.Items)
                {
                    string newFileName = item.SubItems[0].Text;
                    

                    if (batchFolderName.Text != "" && batchFolderName != null)
                    {
                        fileLocation = downloadFolder + "\\" + batchFolderName.Text;
                    }
                    else
                    {
                        fileLocation = downloadFolder + "\\" + botSearchInput.Text;
                    }

                    if (!Directory.Exists(fileLocation))
                    {
                        Directory.CreateDirectory(fileLocation);
                    }
                    
                    fileLocation = fileLocation + "\\" + item.SubItems[0].Text;

                    appendToDownloadList(item.SubItems[0].Text, item.SubItems[2].Text, item.SubItems[3].Text, fileLocation.Trim());
                }

                
            } else
            {
                MessageBox.Show("You did not select a bot!");
            }
            
        }

        //append every file selected in animeSearchList to downloadList
        private void dlSelected_Click(object sender, EventArgs e)
        {
            batchMode = true;
            foreach (ListViewItem item in animeSearchResult.SelectedItems)
            {
                string newFileName = item.SubItems[0].Text;


                if (batchFolderName.Text != "" && batchFolderName != null)
                {
                    fileLocation = downloadFolder + "\\" + batchFolderName.Text;
                }
                else
                {
                    fileLocation = downloadFolder + "\\" + botSearchInput.Text;
                }

                if (!Directory.Exists(fileLocation))
                {
                    Directory.CreateDirectory(fileLocation);
                }

                fileLocation = fileLocation + "\\" + newFileName;

                appendToDownloadList(item.SubItems[0].Text, item.SubItems[2].Text, item.SubItems[3].Text, fileLocation.Trim());
            }

            ircTabs.SelectedIndex = 2;


        }

        //-------Settings tab --------

        //set download path at launch
        private void setDownloadPath()
        {
            string userName = Environment.UserName;
            string defaultDlFolder = @"C:\Users\" + userName + @"\Downloads";


            if (!Directory.Exists(defaultDlFolder))
            {
                MessageBox.Show("Default download folder \"" + defaultDlFolder + "\" does not exists. \n Please set default download folder in settings before using this application!");
                ircTabs.SelectedIndex = 4;
            }
            else
            {
                downloadFolder = defaultDlFolder + @"\WeebIRC\";
                if (!Directory.Exists(downloadFolder))
                {
                    Directory.CreateDirectory(downloadFolder);
                    Directory.CreateDirectory(downloadFolder + @"\Settings");
                }
            }
            downloadFolderLoc.Text = downloadFolder;
            simpleIrc.setCustomDownloadDir(downloadFolder);
        }


        //set default download folder
        private void setDlFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != "" || fbd.SelectedPath != null)
            {
                downloadFolder = fbd.SelectedPath;

                if (!Directory.Exists(downloadFolder + @"\Settings"))
                {
                    Directory.CreateDirectory(downloadFolder + @"\Settings");
                    createSettingsConf();
                    setDefaultConnect(getDefaultIP(), getDefaultPORT(), getDefaultUsername(), getDefaultChannel(), getAutoJoin());
                }

                updateDownloadFolder(downloadFolder);

                downloadFolderLoc.Text = downloadFolder;
                simpleIrc.setCustomDownloadDir(downloadFolder);
            }

        }

        //set or unset per file a folder
        private void folderForEachFile_Click(object sender, EventArgs e)
        {
            if (!folderForEachFile.Checked)
            {
                folderForEachFile.Checked = true;
                perDownloadAFolder = true;
            }
            else
            {
                folderForEachFile.Checked = false;
                perDownloadAFolder = false;
            }
            updateFolderForEachFile(perDownloadAFolder);
        }

        //set or unset per batch a folder
        private void folderPerBatch_Click(object sender, EventArgs e)
        {
            if (!folderPerBatch.Checked)
            {
                folderPerBatch.Checked = true;
                perBatchAFolder = true;
            }
            else
            {
                folderPerBatch.Checked = false;
                perBatchAFolder = false;
            }

            updateFolderPerBatch(perBatchAFolder);
        }

        //create settings.xml

        private void createSettingsConf()
        {

            if(!System.IO.File.Exists(downloadFolder + @"\Settings\Settings.conf"))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(downloadFolder + @"\Settings\Settings.conf"))
                    {
                        sw.WriteLine("DefaultIP = irc.rizon.net;");
                        sw.WriteLine("DefaultPORT = 6667;");
                        sw.WriteLine("DefaultUsername = WeebIRCDev;");
                        sw.WriteLine("DefaultChannel = #horriblesubs,#nibl;");
                        sw.WriteLine("folderForEachFile = 0;");
                        sw.WriteLine("folderPerBatch = 1;");
                        sw.WriteLine("autoJoin = 0;");
                        sw.WriteLine("defaultDownloadFolder = " + downloadFolder + ";");
                    }
                }
                catch (Exception EX)
                {
                    AppendDebug("Couldnt create and/or write to settings file (@" + downloadFolder + @"\Settings\Settings.conf" + "):X");
                    AppendDebug(EX.ToString());
                }

            }

        }

        private void setDefaultConnect(string ip, string port, string username, string channel, bool autoJoin)
        {
            try
            {
                AppendDebug("updating: " + getDefaultIP() + "->" + ip);
                AppendDebug("updating: " + getDefaultPORT() + "->" + port);
                AppendDebug("updating: " + getDefaultUsername() + "->" + username);
                AppendDebug("updating: " + getDefaultChannel() + "->" + channel);
                string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");

                AppendDebug("CURRENT SETTINGS: ");
                AppendDebug(settings);

                settings = settings.Replace(getDefaultIP(), ip);
                settings = settings.Replace(getDefaultPORT(), port);
                settings = settings.Replace(getDefaultUsername(), username);
                settings = settings.Replace(getDefaultChannel(), channel);

                if (autoJoin)
                {
                    if (!getAutoJoin())
                    {
                        settings = settings.Replace("autoJoin = 0;", "autoJoin = 1;");
                    }
                } else
                {
                    if (getAutoJoin())
                    {
                        settings = settings.Replace("autoJoin = 1;", "autoJoin = 0;");
                    }
                }
                AppendDebug("NEW SETTINGS: ");
                AppendDebug(settings);

                System.IO.File.WriteAllText(downloadFolder + @"\Settings\Settings.conf", settings);
            }
            catch (Exception EX)
            {
                AppendDebug("Couldnt write to settings file (update default connect settings):X");
                AppendDebug(EX.ToString());
            }

        }

        private void updateDownloadFolder(string dlFolder)
        {
            try
            {
                string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
                settings.Replace(getDownloadFolder(), dlFolder);
                System.IO.File.WriteAllText(downloadFolder + @"\Settings\Settings.conf", settings);
            }
            catch (Exception E)
            {
                AppendDebug("Couldnt write to settings file :X (update dl folder)");
                AppendDebug(E.ToString());
            }
        }

        private void updateFolderForEachFile(bool dlFolderForEachFile)
        {
            try
            {
                string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
                if (dlFolderForEachFile)
                {
                    if (!getFolderForEachFile())
                    {
                        settings.Replace("folderForEachFile = 0;", "folderForEachFile = 1;");
                    }
                }
                else
                {
                    if (getFolderForEachFile())
                    {
                        settings.Replace("folderForEachFile = 1;", "folderForEachFile = 0;");
                    }
                }
                System.IO.File.WriteAllText(downloadFolder + @"\Settings\Settings.conf", settings);
            }
            catch (Exception E)
            {
                AppendDebug("Couldnt write to settings file (folderPerFile):X");
                AppendDebug(E.ToString());
            }
        }

        private void updateFolderPerBatch(bool dlFolderPerBatch)
        {
            try
            {
                string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
                if (dlFolderPerBatch)
                {
                    if (!getFolderPerBatch())
                    {
                        settings.Replace("folderPerBatch = 0;", "folderPerBatch = 1;");
                    }
                }
                else
                {
                    if (getFolderPerBatch())
                    {
                        settings.Replace("folderPerBatch = 1;", "folderPerBatch = 0;");
                    }
                }
                System.IO.File.WriteAllText(downloadFolder + @"\Settings\Settings.conf", settings);
            }
            catch (Exception E)
            {
                AppendDebug("Couldnt write to settings file (folderPerbatch) :X");
                AppendDebug(E.ToString());
            }
        }

        //gets default ip
        private string getDefaultIP()
        {
            string settings =  System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            return settings.Split('=')[1].Split(';')[0].Trim();
        }

        private string getDefaultPORT()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            return settings.Split('=')[2].Split(';')[0].Trim();
        }

        private string getDefaultUsername()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            return settings.Split('=')[3].Split(';')[0].Trim();
        }

        private string getDefaultChannel()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            return settings.Split('=')[4].Split(';')[0].Trim();

        }


        private string getDownloadFolder()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            return settings.Split('=')[8].Split(';')[0].Trim();
        }

        private bool getAutoJoin()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            if(settings.Split('=')[7].Split(';')[0].Trim() == "1")
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool getFolderForEachFile()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            if(settings.Split('=')[5].Split(';')[0].Trim() == "1")
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool getFolderPerBatch()
        {
            string settings = System.IO.File.ReadAllText(downloadFolder + @"\Settings\Settings.conf");
            if (settings.Split('=')[6].Split(';')[0].Trim() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }



      
        
       

        //----------Debug Tab----------------

        //Add debug messages to a richTextBox
        public void AppendDebug(string input)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendDebug), new object[] { input });
                return;
            }
            debugWindow.Text += input + "\n";
            debugWindow.SelectionStart = chatOutput.Text.Length;
            debugWindow.ScrollToCaret();
        }

        
    }
    
}
