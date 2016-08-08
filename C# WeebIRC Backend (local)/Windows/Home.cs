using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using SimpleIRCLib;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WeebIRCServerTray
{
    public partial class Home : MaterialForm
    {

        private static HttpServer webserver = null;
        private static HttpServer streamserver = null;
        public static SimpleIRC irc = null;
        public static string dldir = "";
        public static string customdldir = "";
        public static string allFiles = "";
        public static string doesOSMCExist = "false";
        public static string previousstat = "";
        public static string allSeasonsJson = "";
        public static string animePerSeasonJson = "";
        public static string searchedAnimeJson = "";
        public static string niblSearchJson = "";
        public static string OS = "";
        public static string previousMessage = "";
        public static int port = 8080;
        public static int downloads = 0;
        public static bool loggedIn = false;
        public static bool saveDebug = false;
        public static bool showDebug = false;
        public static bool parsedSubtitle = false;
        private static Debug debug;
        private static string VersionControl = "v3";


        public Home()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK ;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue900, Primary.Blue500, Accent.Blue200, TextShade.WHITE);
            debug = new Debug();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       

        private void Debug_Click_1(object sender, EventArgs e)
        {           
            debug.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://weebirc-rareamv.c9users.io/");
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            bool noIssues = true;
            try
            {

                irc.stopClient();
                DebugCallbackMethod("|HOME|Stopped irc client");
            } catch (Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop irc client: " + ex.ToString());
            }

            try
            {

                webserver.StopServer();
                DebugCallbackMethod("|HOME|Stopped web server");
            } catch (Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop web server: " + ex.ToString());
            }

            try
            {

                streamserver.StopServer();
                DebugCallbackMethod("|HOME|Stopped stream server");
            } catch( Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop stream server: " + ex.ToString());
            }

            if (noIssues)
            {
                debug.Close();
                this.Close();
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            SetupDownloadDir(Directory.GetCurrentDirectory() + @"/Downloads/");

            RunningPlatform();

            GetCurrentFiles();

            irc = new SimpleIRC();

            webserver = new HttpServer(DebugCallbackMethod, HttpMessageReceived, port);
            webserver.SetWebHomeDir(Directory.GetCurrentDirectory() + @"/Views");
            webserver.SetFileDir(Directory.GetCurrentDirectory() + "/Downloads");
            webserver.SetDefaultPage("index.html");
            streamserver = new HttpServer(DebugCallbackMethod, HttpMessageReceived, port + 1);
            streamserver.SetWebHomeDir(Directory.GetCurrentDirectory() + @"/Downloads");
            streamserver.SetFileDir(Directory.GetCurrentDirectory() + "/Downloads");
            streamserver.SetDefaultPage("index.html");

            State.Text = "Running (" + GetLocalIPAddress() + ":" + port.ToString() + ")";
        }

        /// <ParseAnimeSeason>
        /// <summary>
        ///  Parses all the anime from the html source of Myanimelists.net 's seasons page
        /// </summary
        /// </ParseAnimeSeason>
        private static void ParseAnimeSeason(string url, string yearAndseason)
        {
            if (!Directory.Exists("DataStorage/Anime"))
            {
                Directory.CreateDirectory("DataStorage/Anime/");
            }
            string path = "DataStorage/Anime/anime" + yearAndseason + ".json";

            string localJson = "";
            if (!File.Exists(path))
            {



                var animeSeasonsJson = new List<AnimeInfo>();


                string prevAnime = "";


                HtmlParser parseCurrentSeason = new HtmlParser();
                parseCurrentSeason.ParseUrl(url);
                string[,,] test = parseCurrentSeason.CutHtml("seasonal-anime js-seasonal-anime");

                int bound0 = test.GetUpperBound(0);
                int bound1 = test.GetUpperBound(1);

                for (int i = 0; i <= bound0; i++)
                {
                    string animeTitle = "";
                    string animeCover = "";
                    string animeSynopsis = "";
                    string animeGenres = "";
                    string animeScore = "";
                    string animeId = "";
                    for (int a = 0; a <= bound1; a++)
                    {
                        string tag = test[i, a, 0];
                        string textInsidetag = test[i, a, 1];
                        try
                        {
                            if (tag.Contains("link-image") || textInsidetag.Contains("link-image"))
                            {
                                animeTitle = textInsidetag.Trim();
                            }
                            if (tag.Contains("lazyload") || textInsidetag.Contains("lazyload"))
                            {
                                animeCover = tag.Split('"')[3].Split('"')[0];
                                string[] partsOfUrl = animeCover.Split('/');
                                animeId = partsOfUrl[partsOfUrl.Length - 1].Split('.')[0];
                            }
                            if (tag.Contains("synopsis js-synopsis") || textInsidetag.Contains("synopsis js-synopsis"))
                            {
                                animeSynopsis = textInsidetag.Replace("\"", "\\\"").Replace("'", "\'").Split(new string[] { "<p" }, StringSplitOptions.None)[0];
                            }
                            if (tag.Contains("genre") || textInsidetag.Contains("genre"))
                            {
                                animeGenres = textInsidetag.Trim();
                            }
                            if (tag.Contains("score") || textInsidetag.Contains("score"))
                            {
                                animeScore = textInsidetag.Trim();
                            }
                            if (animeScore != "" && animeGenres != "" && animeSynopsis != "" && animeId != "" && animeCover != "" && animeTitle != "")
                            {
                                if (prevAnime != animeTitle)
                                {
                                    animeSeasonsJson.Add(new AnimeInfo() { id = animeId, title = animeTitle, cover = animeCover, synopsis = animeSynopsis, genres = animeGenres, score = animeScore });

                                    prevAnime = animeTitle;
                                }
                                break;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    string actualJson = JsonConvert.SerializeObject(animeSeasonsJson, Formatting.Indented);
                    DebugCallbackMethod("WRITING ANIME JSON");
                    localJson = "[{\"Anime\" : " + actualJson + "}]";
                    sw.Write(localJson);
                }
                DebugCallbackMethod(" |LAUNCH| " + Path.GetFileName(path) + " created!");
            }
            else
            {
                DebugCallbackMethod(" |LAUNCH| " + "No need to parse html, anime already parsed, reading file instead!");
                localJson = File.ReadAllText(path);
            }
            animePerSeasonJson = localJson;
            DebugCallbackMethod("DONE");
        }

        /// <ParseSearchAnime>
        /// <summary>
        ///  Parses html received from the search page of Myanimelist.net
        /// </summary
        /// </ParseSearchAnime>
        private static void ParseSearchAnime(string url)
        {


            searchedAnimeJson = "";
            string prevAnime = "";

            HtmlParser parser = new HtmlParser();
            parser.ParseUrl(url);

            var searchedAnimeJsonList = new List<AnimeInfo>();
            string[,,] test = parser.CutHtml("picSurround");

            int bound0 = test.GetUpperBound(0);
            int bound1 = test.GetUpperBound(1);

            for (int i = 0; i <= bound0; i++)
            {
                string animeTitle = "";
                string animeCover = "";
                string animeSynopsis = "";
                string animeGenres = "N/A";
                string animeScore = "";
                string animeId = "";
                for (int a = 0; a <= bound1; a++)
                {
                    string tag = test[i, a, 0];
                    string textInsidetag = test[i, a, 1];
                    try
                    {
                        if (tag.Contains("lazyload"))
                        {
                            animeTitle = tag.Split('"')[11].Split('"')[0].Trim();
                            animeCover = tag.Split('"')[3].Split('"')[0].Replace("r/50x70/", "").Trim();
                            string[] partsOfUrl = animeCover.Split('/');
                            animeId = partsOfUrl[partsOfUrl.Length - 1].Split('.')[0];
                        }

                        if (tag.Contains("pt4"))
                        {
                            animeSynopsis = textInsidetag.Trim();
                        }

                        if (tag.Contains("borderClass ac bgColor1\" width=\"50\"") || tag.Contains("borderClass ac bgColor0\" width=\"50\""))
                        {
                            animeScore = textInsidetag.Trim();
                        }

                        if (animeTitle != "" && animeSynopsis != "" && animeCover != "" && animeId != "" && animeScore != "")
                        {
                            searchedAnimeJsonList.Add(new AnimeInfo() { id = animeId, title = animeTitle, cover = animeCover, synopsis = animeSynopsis, genres = animeGenres, score = animeScore });
                            break;
                        }


                    }
                    catch (Exception e)
                    {

                    }
                }

            }

            string actualJson = JsonConvert.SerializeObject(searchedAnimeJsonList, Formatting.Indented);
            DebugCallbackMethod("WRITING ANIME JSON");
            searchedAnimeJson = "[{\"Anime\" : " + actualJson + "}]";
            DebugCallbackMethod("DONE");
        }

        /// <ParseAllSeasons>
        /// <summary>
        ///  Gets all the available seasons from Myanimelist.net, and writes them to a json file, or if the json file already exist, reads the json file(faster) instead of html parsing 
        /// </summary
        /// </ParseAllSeasons>
        private static void ParseAllSeasons()
        {
            if (!Directory.Exists("DataStorage"))
            {
                Directory.CreateDirectory("DataStorage");
            }

            string path = "DataStorage/allSeasonsJson.json";

            if (!File.Exists(path))
            {

                string prevSeason = "";
                var seasons = new List<AnimeSeasons>();
                HtmlParser parseSeasons = new HtmlParser();
                parseSeasons.ParseUrl("http://myanimelist.net/anime/season/archive");

                string[,,] test = parseSeasons.CutHtml("<td>");

                int bound0 = test.GetUpperBound(0);
                int bound1 = test.GetUpperBound(1);

                for (int i = 0; i <= bound0; i++)
                {
                    string seasonUrl = "";
                    string seasonYear = "";
                    for (int a = 0; a <= bound1; a++)
                    {
                        string tag = test[i, a, 0];
                        string textInsidetag = test[i, a, 1];
                        try
                        {
                            if (tag.Contains("http://myanimelist.net/anime/season") || textInsidetag.Contains("http://myanimelist.net/anime/season"))
                            {
                                seasonUrl = tag.Replace("a href=\"", "").Replace("\"", "").Trim();
                                seasonYear = textInsidetag.Trim();
                                if (prevSeason != seasonYear)
                                {
                                    seasons.Add(new AnimeSeasons() { season = seasonYear, url = seasonUrl });
                                    prevSeason = seasonYear;
                                }
                                break;
                            }

                        }
                        catch (Exception e)
                        {

                        }
                    }
                }


                using (StreamWriter sw = File.CreateText(path))
                {
                    string actualJson = JsonConvert.SerializeObject(seasons, Formatting.Indented);
                    allSeasonsJson = "[{\"allSeasons\" : " + actualJson + "}]";
                    sw.Write(allSeasonsJson);
                }
                DebugCallbackMethod(" |LAUNCH| " + Path.GetFileName(path) + " created!");
            }
            else
            {
                DebugCallbackMethod(" |LAUNCH| " + "No need to parse html, seasons already parsed, reading file instead!");
                allSeasonsJson = File.ReadAllText(path);
            }
        }

        public static void ParseSearchNibl(string[] searchQuery)
        {
            HtmlParser parser = new HtmlParser();
            var niblSearchResults = new List<NiblSearchInformation>();



            foreach (string searchParam in searchQuery)
            {
                parser.ParseUrl("http://nibl.co.uk/bots.php?search=" + searchParam);
                string[,,] test = parser.CutHtml("botlistitem");

                int bound0 = test.GetUpperBound(0);
                int bound1 = test.GetUpperBound(1);

                for (int i = 0; i <= bound0; i++)
                {
                    string bot = "";
                    string pack = "";
                    string filename = "";
                    string res = "";
                    for (int a = 0; a <= bound1; a++)
                    {
                        string tag = test[i, a, 0];
                        string textInsidetag = test[i, a, 1];
                        try
                        {
                            if (tag.Contains("botname") || textInsidetag.Contains("botname"))
                            {
                                bot = textInsidetag.Split('<')[0].Trim();
                            }

                            if (tag.Contains("packnumber"))
                            {
                                pack = textInsidetag.Trim();
                            }

                            if (tag.Contains("filename"))
                            {
                                filename = textInsidetag.Split('<')[0].Trim();

                                if (filename.Contains("480") || filename.Contains("360") || filename.Contains("576") || filename.Contains("SD"))
                                {
                                    res = "480";
                                }
                                else if (filename.Contains("720") || filename.Contains("768"))
                                {
                                    res = "720";
                                }
                                else if (filename.Contains("1080") || filename.Contains("1000"))
                                {
                                    res = "1080";
                                }
                                else
                                {
                                    res = "unknown";
                                }
                            }

                            if (filename != "" && bot != "" && pack != "")
                            {
                                niblSearchResults.Add(new NiblSearchInformation() { botname = bot, filename = filename, packnumber = pack, resolution = res });
                                break;
                            }



                        }
                        catch (Exception e)
                        {

                        }
                    }

                }
            }

            var sd = new List<NiblSearchInformation>();
            var hd = new List<NiblSearchInformation>();
            var fhd = new List<NiblSearchInformation>();
            var unknown = new List<NiblSearchInformation>();
            var bots = new List<string>();

            foreach (NiblSearchInformation info in niblSearchResults)
            {
                if (info.resolution.Contains("480"))
                {
                    sd.Add(info);
                }
                else if (info.resolution.Contains("720"))
                {
                    hd.Add(info);
                }
                else if (info.resolution.Contains("1080"))
                {
                    fhd.Add(info);
                }
                else
                {
                    unknown.Add(info);
                }
            }


            Dictionary<string, Dictionary<string, List<NiblSearchInformation>>> values = new Dictionary<string, Dictionary<string, List<NiblSearchInformation>>>();

            Dictionary<string, List<NiblSearchInformation>> everyResolution = new Dictionary<string, List<NiblSearchInformation>>();

            string currentBot = " ";
            string previousBot = "";
            foreach (NiblSearchInformation info in niblSearchResults)
            {
                currentBot = info.botname;

                if (currentBot != previousBot)
                {
                    if (!values.ContainsKey(currentBot))
                    {
                        bots.Add(currentBot);
                    }
                    previousBot = currentBot;
                }
            }



            foreach (string currentBotToSearch in bots)
            {

                var search480p = sd.FindAll(x => x.botname.Contains(currentBotToSearch));
                var search720p = hd.FindAll(x => x.botname.Contains(currentBotToSearch));
                var search1080p = fhd.FindAll(x => x.botname.Contains(currentBotToSearch));
                var searchUnkown = unknown.FindAll(x => x.botname.Contains(currentBotToSearch));
                everyResolution.Add("480", search480p);
                everyResolution.Add("720", search720p);
                everyResolution.Add("1080", search1080p);
                everyResolution.Add("unknown", searchUnkown);
                values.Add(currentBotToSearch, everyResolution);
                everyResolution = new Dictionary<string, List<NiblSearchInformation>>();
            }


            string actualJson = JsonConvert.SerializeObject(values, Formatting.Indented);
            niblSearchJson = "[{\"NIBL\" : " + actualJson + "}]";

        }

        /// <DebugCallbackMethod>
        /// <summary>
        ///  Prints debug messages to console and/or writes to a log file
        /// </summary
        /// </DebugCallbackMethod>
        private static bool isLogCreated = false;
        private static string pathDebug = "";
        private static void DebugCallbackMethod(string debugmsg)
        {

            if (!isLogCreated)
            {
                pathDebug = GetTimeStamp().Replace(":", "_") + "_LOG.log";
                isLogCreated = true;
            }
            if (saveDebug)
            {
                try
                {
                    if (!File.Exists(pathDebug))
                    {
                        // Create a file to write to.
                        using (StreamWriter sw = File.CreateText(pathDebug))
                        {
                            sw.WriteLine("LOG CREATED ON: " + GetTimeStamp());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(pathDebug))
                        {
                            sw.WriteLine(GetTimeStamp() + " -> " + debugmsg);
                        }
                    }
                }
                catch
                {

                }

            }
            debug.AppendToDebug(debugmsg);
        }

        /// <SendIRCMessageToHttpServer>
        /// <summary>
        ///  Sends message received from irc client to http com server
        /// </summary>
        /// </SendIRCMessageToHttpServer>
        private static void SendIRCMessageToHttpServer(string user, string message)
        {
            webserver.SendMessage("irc: " + user + " : " + message);
            DebugCallbackMethod("IRC RESPONSE: " + user + " : " + message);
            if (!loggedIn)
            {
                loggedIn = true;
            }
        }

        /// <HttpMessageReceived>
        /// <summary>
        ///  Executes action on message received from http com server, action depends on message
        /// </summary>
        /// </HttpMessageReceived>
        private static void HttpMessageReceived(string message)
        {
            if (message.Contains("ISCONNECTED"))
            {

                DebugCallbackMethod(message);
                webserver.SendMessage("WEEB HERE, VERSION~~" + VersionControl);
                webserver.SendMessage("MACHINE:" + Environment.MachineName);
            }

            if (previousMessage != message)
            {

                if (message.Contains("COMCLIENTCONNECTED"))
                {
                    GetCurrentFiles();
                }
                else if (message.Contains("ABORTDOWNLOAD"))
                {
                    DebugCallbackMethod(" |MAIN| " + "CLIENT WANTS TO ABORT DOWNLOAD! ");
                    if (irc.isClientRunning())
                    {
                        if (irc.stopXDCCDownload())
                        {
                            webserver.SendMessage("DOWNLOAD ABORTED");
                        } else
                        {
                            webserver.SendMessage("FAILED TO ABORT");
                        }
                        
                    }

                }
                else if (message.Contains("CURRENTSEASON"))
                {
                    DebugCallbackMethod(" |HTTPMESSAGE| " + "CLIENT ASKED CURRENT SEASON: " + message);
                    ParseAnimeSeason("http://myanimelist.net/anime/season", "Current");
                    if (animePerSeasonJson != "")
                    {
                        webserver.JsonToSend(animePerSeasonJson);
                    }

                }
                else if (message.Contains("GETALLSEASONS"))
                {

                    DebugCallbackMethod(" |HTTPMESSAGE| " + "CLIENT REQUESTED ALL SEASONS: " + message);
                    ParseAllSeasons();
                    if (allSeasonsJson != "")
                    {
                        webserver.JsonToSend(allSeasonsJson);
                    }

                }
                else if (message.Contains("GETSEASON"))
                {
                    string[] parts = message.Split('~');
                    string url = parts[1];
                    string season = parts[2];
                    DebugCallbackMethod(" |HTTPMESSAGE| " + "CLIENT ASKED SEASON: " + season + " URL : " + url);

                    ParseAnimeSeason(url, season);
                    if (animePerSeasonJson != "")
                    {
                        webserver.JsonToSend(animePerSeasonJson);
                    }

                }
                else if (message.Contains("SEARCHANIME"))
                {
                    string[] parts = message.Split('~');
                    string searchUrl = parts[1];
                    DebugCallbackMethod(" |HTTPMESSAGE| " + "CLIENT ASKED FOR SEARCH: " + searchUrl);

                    ParseSearchAnime(searchUrl);
                    if (searchedAnimeJson != "")
                    {
                        webserver.JsonToSend(searchedAnimeJson);
                    }

                }
                else if (message.Contains("SEARCHNIBL"))
                {
                    string[] parts = message.Split('~');

                    ParseSearchNibl(parts);
                    webserver.JsonToSend(niblSearchJson);
                }
                else if (message.Contains("GETLOCALFILES"))
                {
                    GetCurrentFiles();
                    webserver.SendMessage("LOCALFILESRECEIVED");
                    webserver.JsonToSend(allFiles);

                }
                else if (message.Contains("SETDLDIR"))
                {
                    string newDir = message.Split('~')[1];
                    DebugCallbackMethod(" |HTTPMESSAGE| " + "CLIENT REQUESTED CHANGE OF DIR: " + newDir);
                    if (irc.isClientRunning())
                    {
                        irc.setCustomDownloadDir(dldir.Trim() + "/" + newDir.Trim());
                        customdldir = newDir.Trim();
                    }

                }
                else if (message.Contains("server:") && message.Contains("channel:") && message.Contains("username:"))
                {
                    Regex regex1 = new Regex(@"^(?=.*(?<server>((?<=server: )(.*\n?)(?=channel: ))))(?=.*(?<channel>((?<=channel: )(.*\n?)(?=username: ))))(?=.*(?<username>((?<=username: )(.*\n?)(?=junk: ))))");
                    Match matches1 = regex1.Match(message);

                    if (matches1.Success)
                    {
                        string server = matches1.Groups["server"].Value.Trim();
                        string channel = matches1.Groups["channel"].Value.Trim();
                        string username = matches1.Groups["username"].Value.Trim().Replace("\0", string.Empty);

                        if (server != "")
                        {
                            if (irc.isClientRunning())
                            {
                                irc.stopClient();
                            }

                            DebugCallbackMethod(" |IRC| " + "Setting up IRC Connection");
                            DebugCallbackMethod(" |IRC| " + "SERVER: " + server);
                            DebugCallbackMethod(" |IRC| " + "CHANNEL(S): " + channel);
                            DebugCallbackMethod(" |IRC| " + "USERNAME: " + username);

                            irc.setupIrc(server, 6665, username, "", channel, SendIRCMessageToHttpServer);
                            irc.setCustomDownloadDir(dldir);
                            irc.setDownloadStatusChangeCallback(DownloadStatusChange);
                            irc.setDebugCallback(DebugIRC);

                            DebugCallbackMethod(" |LAUNCH| " + "Starting IRC Client");

                            irc.startClient();

                            DebugCallbackMethod(" |LAUNCH| " + "Succesfully started IRC Client");
                        }
                    }
                }
                else if (message.Contains("irc:"))
                {
                    DebugCallbackMethod("Received message to send to irc: ");
                    Regex regex1 = new Regex(@"(?<=irc:)(.*\n?)");
                    Match matches1 = regex1.Match(message);
                    if (matches1.Success)
                    {
                        string msg = matches1.Value;
                        DebugCallbackMethod(msg);
                        irc.sendMessage(msg);
                    }
                }
                else if (message.Contains("ISCLIENTRUNNING"))
                {
                    DebugCallbackMethod("asking if client is running");
                    if (loggedIn)
                    {
                        webserver.SendMessage("clientisrunning");
                    }
                    else
                    {
                        try
                        {
                            if (irc != null)
                            {
                                if (irc.isClientRunning())
                                {
                                    webserver.SendMessage("clientisrunning");
                                }
                                else
                                {
                                    webserver.SendMessage("clientisnotrunning");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            webserver.SendMessage("clientisnotrunning");
                        }
                    }

                }
                else if (message.Contains("IRCCLOSE"))
                {
                    loggedIn = false;
                    try
                    {
                        if (irc != null)
                        {
                            if (irc.isClientRunning())
                            {
                                irc.stopClient();
                                webserver.SendMessage("loginfailed");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        webserver.SendMessage("loginsucces");
                    }
                }
            }

            previousMessage = message;

        }

        /// <DownloadStatusChange>
        /// <summary>
        ///  Irc on download status chaned event, sends, when connected to an irc server, the update on the file download to the http server, which sends it to the webclient
        /// </summary>
        /// </DownloadStatusChange>
        private static void DownloadStatusChange()
        {

            DebugCallbackMethod("RECEIVED DOWNLOAD INFORMATION");
            string filename = irc.getDownloadProgress("filename").ToString();
            string status = irc.getDownloadProgress("status").ToString();
            string size = irc.getDownloadProgress("size").ToString();
            string progress = irc.getDownloadProgress("progress").ToString();
            string speed = irc.getDownloadProgress("kbps").ToString();
            string pack = irc.getDownloadProgress("pack").ToString();

            FileInformation curDownload = new FileInformation();
            curDownload.fileName = filename;
            curDownload.fileSize = size;
            curDownload.downloadStatus = status;
            curDownload.downloadProgress = progress;
            curDownload.downloadSpeed = speed;
            curDownload.downloadUrl = ":8081" + "/Downloads/" + customdldir + "/" + filename;
            curDownload.streamUrl = ":8081" + "/Downloads/" + customdldir + "/" + filename;
            curDownload.downloadPack = pack;
            curDownload.fileId = "-1";

            string dlstatusstring = "[{\"currentDownload\" : " + JsonConvert.SerializeObject(curDownload, Formatting.Indented) + "}]";
            DebugCallbackMethod(" |IRC| " + "DOWNLOADING FILE, UPDATING INFORMATION!");

            webserver.JsonToSend(dlstatusstring);
            DebugCallbackMethod(" |IRC| " + "DOWNLOADING FILE, UPDATING INFORMATION!");
            if (Int32.Parse(progress) > 5 && !parsedSubtitle)
            {
                GetSubtitle(filename, customdldir);
                parsedSubtitle = true;
            }
            else if (Int32.Parse(progress) < 1)
            {
                parsedSubtitle = false;
            }

            if (status == "COMPLETED")
            {
                GetCurrentFiles();
            }

            if (previousstat != status && status == "DOWNLOADING")
            {
                downloads++;
            }
            previousstat = status;
        }

        /// <Debug>
        /// <summary>
        ///  Calls debug callback with debug message from irc client
        /// </summary>
        /// </Debug>
        private static void DebugIRC(string debug)
        {
            try
            {
                DebugCallbackMethod(" |IRC| " + "DEBUG: " + debug);
            }
            catch { }
        }

        /// <GetSubtitle>
        /// <summary>
        ///  Parses subtitle from .mkv file using external application "mkvextract.exe" from MkvToolNix (https://github.com/mbunkus/mkvtoolnix/blob/master/COPYING)
        /// </summary>
        /// </GetSubtitle>

        private static void GetSubtitle(string file, string customdir)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            if (OS == "Windows")
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/mkvextract.exe";
                proc.StartInfo.Arguments = "tracks \"" + AppDomain.CurrentDomain.BaseDirectory + "\\Downloads\\" + customdir + "\\" + file + "\" 2:\"" + AppDomain.CurrentDomain.BaseDirectory + "\\Downloads\\" + customdir + "\\" + filename + ".ass\"";
                proc.Start();
                proc.WaitForExit();
                DebugCallbackMethod(" |LAUNCH| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "\\Downloads\\" + customdir + "\\" + filename + ".ass\"");
            }
            else
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "mkvextract";
                proc.StartInfo.Arguments = "tracks \"" + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + file + "\" 2:\"" + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + filename + ".ass\"";
                proc.Start();
                proc.WaitForExit();
                DebugCallbackMethod(" |LAUNCH| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + Path.GetFileNameWithoutExtension(filename) + ".ass\"");
            }
        }

        /// <GetLocalIPAddress>
        /// </summary>
        ///  Gets the local IP address received from your router/modem, this address is crucial to let others connect. Code from: http://stackoverflow.com/questions/6803073/get-local-ip-address
        /// </summary>
        /// </GetLocalIPAddress>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        /// <GetCurrentFiles>
        /// <summary>
        ///  Gets the current files in the download directory
        /// </summary>
        /// </GetCurrentFiles>
        private static void GetCurrentFiles()
        {

            string[] directories = Directory.GetDirectories(dldir);

            try
            {
                Dictionary<string, List<FileInformation>> dirFileList = new Dictionary<string, List<FileInformation>>();
                foreach (string directory in directories)
                {
                    string dirName = "";
                    string fullDirName = "";
                    string id = "";

                    try
                    {
                        string[] parts = directory.Split('/');
                        dirName = parts[parts.Length - 1].Split('_')[0];
                        fullDirName = parts[parts.Length - 1];
                        id = parts[parts.Length - 1].Split('_')[1];
                    }
                    catch
                    {
                        string[] parts = directory.Split('\\');
                        dirName = parts[parts.Length - 1].Split('_')[0];
                        fullDirName = parts[parts.Length - 1];
                        id = parts[parts.Length - 1].Split('_')[1];
                    }

                    var fileList = new List<FileInformation>();
                    string[] filesInDir = Directory.GetFiles(directory);
                    foreach (string filePath in filesInDir)
                    {
                        string filename = Path.GetFileName(filePath);
                        if (filename.Contains(".mkv") || filename.Contains(".mp4") || filename.Contains(".avi"))
                        {
                            FileInfo f = new FileInfo(filePath);
                            long s1 = f.Length;
                            fileList.Add(new FileInformation() { fileName = filename, fileSize = s1.ToString(), downloadStatus = "COMPLETED", downloadProgress = "100", downloadSpeed = "0", downloadUrl = ":8081/Downloads/" + fullDirName + "/" + filename, streamUrl = ":8081/Downloads/" + fullDirName + "/" + filename, downloadPack = "0000", fileId = id });
                            downloads++;
                            DebugCallbackMethod(" |LAUNCH| " + "Existing Files Found: " + filename);
                        }
                    }

                    dirFileList.Add(dirName, fileList);


                }
                allFiles = "[{\"LocalFiles\" : " + JsonConvert.SerializeObject(dirFileList, Formatting.Indented) + "}]";
            }
            catch (Exception e)
            {
                DebugCallbackMethod(" |LAUNCH| " + "No Files Found");
            }
        }

        /// <SetupDownloadDir>
        /// <summary>
        /// Sets the download directory to be used
        /// </summary>
        /// </SetupDownloadDir>
        private static void SetupDownloadDir(string dir)
        {

            dldir = dir;
            if (!Directory.Exists(dldir))
            {
                DebugCallbackMethod("Could not find download directory: " + dldir);
                DebugCallbackMethod(" |LAUNCH| " + "Could not locate download directory: " + dldir);
                Directory.CreateDirectory(dldir);
                DebugCallbackMethod(" |LAUNCH| " + "Download Directory: " + dldir + " created!");
            }
            else
            {
                DebugCallbackMethod(" |LAUNCH| " + "Current Download Directory: " + dldir);
            }
        }

        /// <Platform>
        /// <summary>
        /// Custom type for OS platform, got code from: http://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems
        /// </summary>
        /// </Platform>
        public enum Platform
        {
            Windows,
            Linux,
            Mac
        }

        /// <RunningPlatform>
        /// <summary>
        /// Executes OS related code, got code partially from: http://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems
        /// It tryes to locate "mkvextract.exe" from MkvToolNix (https://github.com/mbunkus/mkvtoolnix/blob/master/COPYING), if not available, prompts user with ability to download application
        /// Starts interface when windows is detected, assuming local usage
        /// </summary>
        /// </RunningPlatform>
        private static void RunningPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                    {

                        OS = "mac";

                        break;
                    }
                    //mac
                    else
                    {
                        //linux
                        OS = "linux";

                        try
                        {
                            System.Diagnostics.Process proc = new System.Diagnostics.Process();
                            proc.EnableRaisingEvents = false;
                            proc.StartInfo.FileName = "mkvextract";
                            proc.StartInfo.Arguments = "-h";
                            proc.Start();
                            proc.WaitForExit();
                            DebugCallbackMethod("|LAUNCH|MKVEXTRACT IS AVAILABLE ON LINUX!");
                        }
                        catch
                        {
                            //apt-get -y install
                            DebugCallbackMethod("|LAUNCH|MKVEXTRACT IS MISSING, WOULD YOU LIKE TO DOWNLOAD AND INSTALL?");
                            string dlMkvExtract = Console.ReadLine();
                            if (dlMkvExtract.ToLower().Contains("y"))
                            {
                                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                                proc.EnableRaisingEvents = false;
                                proc.StartInfo.FileName = "sudo apt-get install mkvtoolnix";
                                proc.StartInfo.Arguments = "-y";
                                proc.Start();
                                proc.WaitForExit();

                                try
                                {
                                    proc = new System.Diagnostics.Process();
                                    proc.EnableRaisingEvents = false;
                                    proc.StartInfo.FileName = "mkvextract";
                                    proc.StartInfo.Arguments = "-h";
                                    proc.Start();
                                    proc.WaitForExit();
                                    DebugCallbackMethod("|LAUNCH|MKVEXTRACT IS AVAILABLE ON LINUX!");
                                }
                                catch
                                {
                                    //apt-get -y install
                                    DebugCallbackMethod("|LAUNCH|MKVEXTRACT IS MISSING,  PLEASE INSTALL MANUALLY BY RUNNING FOLLOWING COMMAND:");
                                    DebugCallbackMethod("|LAUNCH|sudo apt-get install mkvtoolnix");
                                    DebugCallbackMethod("");
                                    DebugCallbackMethod("|LAUNCH|Press key to quit...");
                                    Console.ReadLine();
                                    Environment.Exit(0);

                                }
                            }
                        }

                        if (Directory.Exists("/home/osmc"))
                        {
                            DebugCallbackMethod(" |LAUNCH| " + "YOUR SERVER IS RUNNING ON OSMC");
                            doesOSMCExist = "true";
                        }

                        break;
                    }


                case PlatformID.MacOSX:
                    OS = "MacOSX";
                    break;

                default:
                    OS = "Windows";

                    if (File.Exists("mkvextract.exe"))
                    {
                        DebugCallbackMethod("|LAUNCH| MKVEXTRACT IS AVAILABLE ON WINDOWS!");
                    }
                    else
                    {
                        using (WebClient webclient = new WebClient())
                        {
                            DebugCallbackMethod("|LAUNCH| Downloading mkvextract from url: " + "https://github.com/RareAMV/WeebIRC/blob/master/Executables%20and%20Archives/mkvextract.exe?raw=true");
                            webclient.DownloadFile("https://github.com/RareAMV/WeebIRC/blob/master/Executables%20and%20Archives/mkvextract.exe?raw=true", "mkvextract.exe");

                            if (File.Exists("mkvextract.exe"))
                            {
                                DebugCallbackMethod("|LAUNCH|MKVEXTRACT IS AVAILABLE ON WINDOWS!");
                            }
                        }
                    }

                    
                    break;
            }
        }

        /// <GetTimeStamp>
        /// <summary>
        ///  Gets Current TimeStamp
        /// </summary>
        /// </GetTimeStamp>
        private static string GetTimeStamp()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        private void Home_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                // Do some stuff
            }
        }

        private void Tray_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            BringToFront();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://weebirc-rareamv.c9users.io/");
        }

        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool noIssues = true;
            try
            {

                irc.stopClient();
                DebugCallbackMethod("|HOME|Stopped irc client");
            }
            catch (Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop irc client: " + ex.ToString());
            }

            try
            {

                webserver.StopServer();
                DebugCallbackMethod("|HOME|Stopped web server");
            }
            catch (Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop web server: " + ex.ToString());
            }

            try
            {

                streamserver.StopServer();
                DebugCallbackMethod("|HOME|Stopped stream server");
            }
            catch (Exception ex)
            {
                noIssues = false;
                DebugCallbackMethod("|HOME|Could not stop stream server: " + ex.ToString());
            }

        }
    }
}
