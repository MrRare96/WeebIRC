using System;
using System.Collections.Generic;
using SimpleIRCLib;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace WeebIRCWebEdition
{
    class Program
    {

       
        private static HttpServer webserver =  null;
        private static HttpServer streamserver =  null;
        private static SimpleIRC irc =          null;
        private static string   dldir =         "";
        private static string   customdldir =   "";
        private static string   allFiles =      "";
        private static string   doesOSMCExist = "false";
        private static string   previousstat =  "";
        private static string   allSeasonsJson = "";
        private static string   animePerSeasonJson = "";
        private static string   searchedAnimeJson = "";
        private static string   niblSearchJson = "";
        private static string   OS = "";
        private static string   previousMessage = "";
        private static int      port =          80;
        private static int      downloads =     0;
        private static bool     loggedIn =      false;
        private static bool     saveDebug =     false;
        private static bool     showDebug =     false;
        private static bool parsedSubtitle = false;
        private static string VersionControl = "v3";

        /// <Main>
        /// <summary>
        ///  Prompts user with the options for saving a log file and printing log messages to the console window
        ///  After that, it sets the default download directory
        ///  Following it checks the current operating system, and performs actions related to the operating system, see '<RunningPlatform>'
        ///  After that, it tries to locate all already downloaded files
        ///  Then initiates the IRC client for later usage
        ///  And after all that, starts the http servers (see class for more information)
        /// </summary
        /// </Main>
        static void Main(string[] args)
        {
        
            Console.WriteLine("Do you want to save debug messages to a log file? (Yes/No) (Left Empty means No): ");
            string shouldSaveDebug = Console.ReadLine();

            if (shouldSaveDebug.ToLower().Contains("yes"))
            {
                saveDebug = true;
            } else{
                saveDebug = false;
            }

            Console.WriteLine("Do you want to show debug messages in console? (Yes/No) (Left Empty means No): ");

            string shouldShowDebug = Console.ReadLine();

            if (shouldShowDebug.ToLower().Contains("yes"))
            {
                showDebug = true;
            }
            else
            {
                showDebug = false;
            }   

            Console.WriteLine("URL To use in your webbrowser: " + GetLocalIPAddress() + ":" + port.ToString());

            SetupDownloadDir(Directory.GetCurrentDirectory() + @"/Downloads/");
            
            RunningPlatform();            

            GetCurrentFiles();

            irc = new SimpleIRC();

            webserver = new HttpServer(HttpMessageReceived, port);
            webserver.SetWebHomeDir(Directory.GetCurrentDirectory() + @"/Views");
            webserver.SetFileDir(Directory.GetCurrentDirectory() + "/Downloads");
            webserver.SetDefaultPage("index.html");
            streamserver = new HttpServer(HttpMessageReceived, port+1);
            streamserver.SetWebHomeDir(Directory.GetCurrentDirectory() + @"/Downloads");
            streamserver.SetFileDir(Directory.GetCurrentDirectory() + "/Downloads");
            streamserver.SetDefaultPage("index.html");
            
            while (true)
            {
                string toSend = Console.ReadLine();
                if(toSend != "")
                {
                    if(toSend == "curanime"){
                        ParseAnimeSeason("http://myanimelist.net/anime/season", "current");
                        Console.WriteLine("starting to parse anime!");
                    } else {
                        webserver.SendMessage(toSend);
                    }
                    
                }
            }
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
                            if(animeScore != "" && animeGenres != "" && animeSynopsis != "" && animeId != "" && animeCover != "" && animeTitle != "")
                            {
                                if (prevAnime != animeTitle)
                                {   
                                    animeSeasonsJson.Add(new AnimeInfo() {id = animeId, title = animeTitle, cover = animeCover, synopsis =  animeSynopsis, genres = animeGenres, score = animeScore});
            
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
                    Console.WriteLine("WRITING ANIME JSON");
                    localJson = "[{\"Anime\" : " + actualJson + "}]";
                    sw.Write(localJson);
                }
                DebugCallbackMethod(" |MAIN| " + Path.GetFileName(path) + " created!");
            }
            else
            {
                DebugCallbackMethod(" |MAIN| " + "No need to parse html, anime already parsed, reading file instead!");
                localJson = File.ReadAllText(path);
            }
            animePerSeasonJson = localJson;
            Console.WriteLine("DONE");
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
                            searchedAnimeJsonList.Add(new AnimeInfo() {id = animeId, title = animeTitle, cover = animeCover, synopsis =  animeSynopsis, genres = animeGenres, score = animeScore});
                            break;
                        }


                    }
                    catch (Exception e)
                    {

                    }
                }

            }
            
            string actualJson = JsonConvert.SerializeObject(searchedAnimeJsonList, Formatting.Indented);
            Console.WriteLine("WRITING ANIME JSON");
            searchedAnimeJson  = "[{\"Anime\" : " + actualJson + "}]";
            Console.WriteLine("DONE");
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
                                    seasons.Add(new AnimeSeasons() { season = seasonYear, url = seasonUrl});
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
                DebugCallbackMethod(" |MAIN| " + Path.GetFileName(path) + " created!");
            }
            else
            {
                DebugCallbackMethod(" |MAIN| " + "No need to parse html, seasons already parsed, reading file instead!");
                allSeasonsJson = File.ReadAllText(path);
            }
        }
        
        public static void ParseSearchNibl(string[] searchQuery){
            HtmlParser parser = new HtmlParser();
            var niblSearchResults = new List<NiblSearchInformation>();
            
            
            
            foreach(string searchParam in searchQuery){
                parser.ParseUrl("http://nibl.co.uk/bots.php?search=" + searchParam);
                string[,,] test = parser.CutHtml("botlistitem");
    
                int bound0 = test.GetUpperBound(0);
                int bound1 = test.GetUpperBound(1);
    
                for (int i = 0; i <= bound0; i++)
                {
                    string bot = "";
                    string pack = "";
                    string filename = "";
                    string res= "";
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
                                
                                if(filename.Contains("480") ||  filename.Contains("360") || filename.Contains("576") || filename.Contains("SD")){
                                    res = "480";
                                } else if(filename.Contains("720") || filename.Contains("768")){
                                    res = "720";
                                } else if (filename.Contains("1080") || filename.Contains("1000")){
                                    res= "1080";
                                } else {
                                    res = "unknown";
                                }
                            }
    
                            if (filename != "" && bot != "" && pack != "")
                            {
                                niblSearchResults.Add(new NiblSearchInformation() { botname = bot, filename = filename, packnumber = pack, resolution = res});
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
           
           foreach(NiblSearchInformation info in niblSearchResults){    
                if(info.resolution.Contains("480")){
                    sd.Add(info);
                } else if(info.resolution.Contains("720")) {
                    hd.Add(info);
                } else if(info.resolution.Contains("1080")){
                    fhd.Add(info);
                } else {
                    unknown.Add(info);   
                }
            }
           
           
	        Dictionary<string, Dictionary<string, List<NiblSearchInformation>>> values = new Dictionary<string, Dictionary<string, List<NiblSearchInformation>>>();
	        
	        Dictionary<string, List<NiblSearchInformation>> everyResolution = new Dictionary<string, List<NiblSearchInformation>>();
	        
	        string currentBot = " ";
	        string previousBot = "";
            foreach(NiblSearchInformation info in niblSearchResults){
                currentBot = info.botname;
                
                if(currentBot != previousBot){
                    if(!values.ContainsKey(currentBot)){
                        bots.Add(currentBot);
                    } 
                    previousBot = currentBot;
                }
            }
            
            
        
            foreach(string currentBotToSearch in bots)
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
                } catch
                {

                }
                
            }
            if (showDebug)
            {
                Console.WriteLine(GetTimeStamp() + " -> " + debugmsg);
            }

        }

        /// <SendIRCMessageToHttpServer>
        /// <summary>
        ///  Sends message received from irc client to http com server
        /// </summary>
        /// </SendIRCMessageToHttpServer>
        private static void SendIRCMessageToHttpServer(string user, string message)
        {
            webserver.SendMessage("irc: " + user + " : " + message);
            Console.WriteLine("IRC RESPONSE: " + user + " : " + message);
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
                
                Console.WriteLine(message);
                webserver.SendMessage("WEEB HERE, VERSION~~" + VersionControl);
                webserver.SendMessage("MACHINE:" + Environment.MachineName);
            } 
            
            if(previousMessage != message){
               
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
                        }
                        else
                        {
                            webserver.SendMessage("FAILED TO ABORT");
                        }
                    }

                }
                else if (message.Contains("CURRENTSEASON"))
                {
                    DebugCallbackMethod(" |MAIN| " + "CLIENT ASKED CURRENT SEASON:  " + message);
                    ParseAnimeSeason("http://myanimelist.net/anime/season", "Current");
                    if (animePerSeasonJson != "")
                    {
                        webserver.JsonToSend(animePerSeasonJson);
                    }
    
                }
                else if (message.Contains("GETALLSEASONS"))
                {
    
                    DebugCallbackMethod(" |MAIN| " + "CLIENT REQUESTED ALL SEASONS: " + message);
                    ParseAllSeasons();
                    if(allSeasonsJson != "")
                    {
                        webserver.JsonToSend(allSeasonsJson);
                    }
                   
                }
                else if(message.Contains("GETSEASON"))
                {
                    string[] parts = message.Split('~');
                    string url = parts[1];
                    string season = parts[2];
                    DebugCallbackMethod(" |MAIN| " + "CLIENT ASKED SEASON: " + season + " URL : " + url);
                    
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
                    DebugCallbackMethod(" |MAIN| " + "CLIENT ASKED FOR SEARCH: " + searchUrl);
    
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
                    if (irc.isClientRunning())
                    {
                        irc.setCustomDownloadDir(dldir.Trim() + "/" + newDir.Trim());
                        customdldir = newDir.Trim() ;
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
                            irc.setDebugCallback(Debug);
    
                            DebugCallbackMethod(" |MAIN| " + "Starting IRC Client");
    
                            irc.startClient();
    
                            DebugCallbackMethod(" |MAIN| " + "Succesfully started IRC Client");
                        }
                    }
                }
                else if (message.Contains("irc:"))
                {
                    Console.WriteLine("Received message to send to irc: ");
                    Regex regex1 = new Regex(@"(?<=irc:)(.*\n?)");
                    Match matches1 = regex1.Match(message);
                    if (matches1.Success)
                    {
                        string msg = matches1.Value;
                        Console.WriteLine(msg);
                        irc.sendMessage(msg);
                    }
                }
                else if (message.Contains("ISCLIENTRUNNING"))
                {
                    Console.WriteLine("asking if client is running");
                    if (loggedIn)
                    {
                        webserver.SendMessage("clientisrunning");
                    } else
                    {
                        try
                        {
                            if (irc != null)
                            {
                                if (irc.isClientRunning())
                                {
                                    webserver.SendMessage("clientisrunning");
                                } else {
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
  
            Console.WriteLine("RECEIVED DOWNLOAD INFORMATION");
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
            } else if(Int32.Parse(progress) < 1)
            {
                parsedSubtitle = false;
            }
            
            if(status == "COMPLETED"){
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
        private static void Debug(string debug)
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
                    DebugCallbackMethod(" |MAIN| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "\\Downloads\\" + customdir + "\\" + filename + ".ass\"");
                }
                else
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = "mkvextract";
                    proc.StartInfo.Arguments = "tracks \"" + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + file + "\" 2:\"" + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + filename + ".ass\"";
                    proc.Start();
                    proc.WaitForExit();
                    DebugCallbackMethod(" |MAIN| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "/Downloads/" + customdir + "/" + Path.GetFileNameWithoutExtension(filename) + ".ass\"");
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
                    string fullDirName ="";
                    string id = "";
                    
                    try{
                        string[] parts = directory.Split('/');
                        dirName = parts[parts.Length - 1].Split('_')[0];
                        fullDirName = parts[parts.Length - 1];
                        id = parts[parts.Length - 1].Split('_')[1];
                    } catch{
                        string[] parts = directory.Split('\\');
                        dirName = parts[parts.Length - 1].Split('_')[0];
                        fullDirName = parts[parts.Length - 1];
                        id = parts[parts.Length - 1].Split('_')[1];
                    }
                    
                    var fileList = new List<FileInformation>();
                    string[] filesInDir = Directory.GetFiles(directory);
                    foreach(string filePath in filesInDir){
                        string filename = Path.GetFileName(filePath);
                        if(filename.Contains(".mkv") || filename.Contains(".mp4") || filename.Contains(".avi"))
                        {
                            FileInfo f = new FileInfo(filePath);
                            long s1 = f.Length;
                            fileList.Add(new FileInformation(){fileName = filename, fileSize = s1.ToString(), downloadStatus = "COMPLETED", downloadProgress = "100", downloadSpeed = "0", downloadUrl = ":8081/Downloads/" + fullDirName + "/" + filename, streamUrl = ":8081/Downloads/" + fullDirName + "/" + filename, downloadPack = "0000", fileId = id});
                            downloads++;
                            DebugCallbackMethod(" |MAIN| " + "Existing Files Found: " + filename);
                        }
                    }
                    
                    dirFileList.Add(dirName, fileList);
                    
                
                }
                allFiles = "[{\"LocalFiles\" : " + JsonConvert.SerializeObject(dirFileList, Formatting.Indented) + "}]";
            }
            catch(Exception e)
            {
                DebugCallbackMethod(" |MAIN| " + "No Files Found");
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
                Console.WriteLine("Could not find download directory: " + dldir);
                DebugCallbackMethod(" |MAIN| " + "Could not locate download directory: " + dldir);
                Directory.CreateDirectory(dldir);
                DebugCallbackMethod(" |MAIN| " + "Download Directory: " + dldir + " created!");
            }
            else
            {
                DebugCallbackMethod(" |MAIN| " + "Current Download Directory: " + dldir);
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
                    else{
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
                            Console.WriteLine("MKVEXTRACT IS AVAILABLE ON LINUX!");
                        }
                        catch
                        {
                            //apt-get -y install
                            Console.WriteLine("MKVEXTRACT IS MISSING, WOULD YOU LIKE TO DOWNLOAD AND INSTALL?");
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
                                    Console.WriteLine("MKVEXTRACT IS AVAILABLE ON LINUX!");
                                }
                                catch
                                {
                                    //apt-get -y install
                                    Console.WriteLine("MKVEXTRACT IS MISSING,  PLEASE INSTALL MANUALLY BY RUNNING FOLLOWING COMMAND:");
                                    Console.WriteLine("sudo apt-get install mkvtoolnix");
                                    Console.WriteLine("");
                                    Console.WriteLine("Press key to quit...");
                                    Console.ReadLine();
                                    Environment.Exit(0);

                                }
                            }
                        }

                        if (Directory.Exists("/home/osmc"))
                        {
                            DebugCallbackMethod(" |MAIN| " + "YOUR SERVER IS RUNNING ON OSMC");
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
                        Console.WriteLine("MKVEXTRACT IS AVAILABLE ON WINDOWS!");
                    }
                    else
                    {
                        Console.WriteLine("MKVEXTRACT IS MISSING, WOULD YOU LIKE TO DOWNLOAD?");
                        string dlMkvExtract = Console.ReadLine();
                        if (dlMkvExtract.ToLower().Contains("y"))
                        {
                            using (WebClient webclient = new WebClient())
                            {
                                Console.WriteLine("Downloading mkvextract from url: " + "https://github.com/RareAMV/WeebIRC/blob/master/Executables%20and%20Archives/mkvextract.exe?raw=true");
                                webclient.DownloadFile("https://github.com/RareAMV/WeebIRC/blob/master/Executables%20and%20Archives/mkvextract.exe?raw=true", "mkvextract.exe");

                                if (File.Exists("mkvextract.exe"))
                                {
                                    Console.WriteLine("MKVEXTRACT IS AVAILABLE ON WINDOWS!");
                                }
                            }
                        }
                    }

                    DebugCallbackMethod(" |MAIN| " + "RUNNING ON WINDOWS, ASSUME LOCAL CLIENT, OPENING INTERFACE IN BROWSER!");
                    System.Diagnostics.Process.Start("http://http://weebirc-rareamv.c9users.io/");
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
    }

    
}
