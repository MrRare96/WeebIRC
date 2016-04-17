using System;
using System.Collections.Generic;
using SimpleIRCLibMono;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace WeebIRCWebEdition
{
    class Program
    {

       
        private static HttpServer httpserver =  null;
        private static SimpleIRC irc =          null;
        private static string   dldir =         "";
        private static string   allFiles =      "";
        private static string   doesOSMCExist = "false";
        private static string   previousstat =  "";
        private static string   allSeasonsJson = "";
        private static string   animePerSeasonJson = "";
        private static string   searchedAnimeJson = "";
        private static string   OS = "";
        private static int      port =          8008;
        private static int      downloads =     0;
        private static bool     loggedIn =      false;
        private static bool     saveDebug =     false;
        private static bool     showDebug =     false;
        private static bool parsedSubtitle = false;

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
            Console.WriteLine("Do you want to save debug messages to log? (Yes/No) (Left Empty means No): ");

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

            SetupDownloadDir(Directory.GetCurrentDirectory() + @"/GUI/Downloads/");

            RunningPlatform();            

            GetCurrentFiles();

            irc = new SimpleIRC();

            httpserver = new HttpServer(8008, HttpMessageReceived, DebugCallbackMethod);

            while (true)
            {
                string toSend = Console.ReadLine();
                if(toSend != "")
                {
                    httpserver.SendMessage(toSend);
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
                HtmlParser parseCurrentSeason = new HtmlParser();
                parseCurrentSeason.ParseUrl(url);
                parseCurrentSeason.CutHtml("seasonal-anime js-seasonal-anime");

                string prevAnime = "";
                foreach (Dictionary<string, List<string>> dic in parseCurrentSeason.seperateDictionary)
                {

                    string animeTitle = "";
                    string animeCover = "";
                    string animeSynopsis = "";
                    string animeGenres = "";
                    string animeScore = "";
                    foreach (var pair in dic)
                    {
                        List<string> insidetags = pair.Value;


                        int i = 0;
                        foreach (string tagtext in insidetags)
                        {

                            if (pair.Key.Contains("link-image") || insidetags.Contains("link-image"))
                            {
                                animeTitle = tagtext.Trim();
                            }
                            if (pair.Key.Contains("background-image:url(") || insidetags.Contains("background-image:url("))
                            {
                                animeCover = pair.Key.Split('(')[1].Split(')')[0];
                            }
                            if (pair.Key.Contains("synopsis js-synopsis") || insidetags.Contains("synopsis js-synopsis"))
                            {
                                animeSynopsis = tagtext.Replace("\"", "\\\"").Replace("'", "\'").Split(new string[] { "<p" }, StringSplitOptions.None)[0];
                            }
                            if (pair.Key.Contains("genre") || insidetags.Contains("genre"))
                            {
                                animeGenres = tagtext.Trim();
                            }
                            if (pair.Key.Contains("score") || insidetags.Contains("score"))
                            {
                                animeScore = tagtext.Trim();
                            }
                            i++;

                        }
                    }

                    if (prevAnime != animeTitle)
                    {

                        localJson = localJson + "\"" + animeTitle + "\" : [{\"cover\":\"" + animeCover + "\", \"synopsis\":\"" + animeSynopsis + "\", \"genres\":\"" + animeGenres + "\", \"score\":\"" + animeScore + "\"}],";
                        prevAnime = animeTitle;
                    }
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    Console.WriteLine("WRITING ANIME JSON");
                    localJson = "[{\"Anime\" : [{" + localJson.Substring(0, localJson.Length - 1) + "}]}]";
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
            HtmlParser parseSearchedAnime = new HtmlParser();
            parseSearchedAnime.ParseUrl(url);
            parseSearchedAnime.CutHtml("picSurround");

            searchedAnimeJson = "";
            string prevAnime = "";
            foreach (Dictionary<string, List<string>> dic in parseSearchedAnime.seperateDictionary)
            {

                string animeTitle = "";
                string animeCover = "";
                string animeSynopsis = "";
                string animeGenres = "";
                string animeScore = "";
                foreach (var pair in dic)
                {
                    List<string> insidetags = pair.Value;


                    int i = 0;
                    foreach (string tagtext in insidetags)
                    {

                        if (pair.Key.Contains("img border") || insidetags.Contains("img border"))
                        {
                            //<img border="0" src="http://cdn.myanimelist.net/images/anime/13/76519t.jpg" alt="One Punch Man Specials">

                            animeCover = pair.Key.Split('"')[3];
                            animeTitle = pair.Key.Split('"')[5];
                        }
                        if (pair.Key.Contains("pt4") || insidetags.Contains("pt4"))
                        {
                            animeSynopsis = tagtext.Replace("\"", "\\\"");
                        }
                        if (pair.Key.Contains("genre") || insidetags.Contains("genre"))
                        {
                            animeGenres = "WIP";
                        }
                        if (pair.Key.Contains("td class=\"borderClass ac bgColor0\" width=\"50\"") || insidetags.Contains("td class=\"borderClass ac bgColor0\" width=\"50\"") || pair.Key.Contains("td class=\"borderClass ac bgColor1\" width=\"50\"") || insidetags.Contains("td class=\"borderClass ac bgColor10\" width=\"50\""))
                        {
                            animeScore = tagtext.Trim();
                        }
                        i++;

                    }
                }

                if (prevAnime != animeTitle)
                {
                    searchedAnimeJson = searchedAnimeJson + "\"" + animeTitle + "\" : [{\"cover\":\"" + animeCover + "\", \"synopsis\":\"" + animeSynopsis + "\", \"genres\":\"" + animeGenres + "\", \"score\":\"" + animeScore + "\"}],";
                    prevAnime = animeTitle;
                }
            }

            searchedAnimeJson = "[{\"Anime\" : [{" + searchedAnimeJson.Substring(0, searchedAnimeJson.Length - 1) + "}]}]";
        
            
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

                HtmlParser parseSeasons = new HtmlParser();
                parseSeasons.ParseUrl("http://myanimelist.net/anime/season/archive");
                parseSeasons.CutHtml("<td>");

                string prevSeason = "";
                foreach (Dictionary<string, List<string>> dic in parseSeasons.seperateDictionary)
                {
                    string seasonUrl = "";
                    string seasonYear = "";
                    foreach (var pair in dic)
                    {
                        List<string> insidetags = pair.Value;


                        int i = 0;
                        foreach (string tagtext in insidetags)
                        {

                            if (pair.Key.Contains("http://myanimelist.net/anime/season") || insidetags.Contains("http://myanimelist.net/anime/season"))
                            {
                                seasonUrl = pair.Key.Replace("a href=\"", "").Replace("\"", "").Trim();
                                seasonYear = tagtext.Trim();
                            }

                            i++;

                        }
                    }

                    if (prevSeason != seasonYear)
                    {

                        allSeasonsJson = allSeasonsJson + "\"" + seasonUrl + "\" : \"" + seasonYear + "\", ";
                        prevSeason = seasonYear;
                    }


                   
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    allSeasonsJson = "[{\"allSeasons\" : [{" + allSeasonsJson.Substring(0, allSeasonsJson.Length - 2) + "}]}]";
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
            httpserver.SendMessage("irc:" + user + " : " + message);
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
                httpserver.SendMessage("ISCONNECTED");
            } else if (message.Contains("SERVERRESET"))
            {
                httpserver.StopServer();
                GetCurrentFiles();
                loggedIn = false;
                try
                {
                    if (irc != null)
                    {
                        if (irc.isClientRunning())
                        {
                            irc.stopClient();
                        }
                    }
                }
                catch
                {
                }

                httpserver = new HttpServer(8008, HttpMessageReceived, DebugCallbackMethod);
            }
            else if (message.Contains("COMCLIENTCONNECTED"))
            {
                GetCurrentFiles();
            }
            else if (message.Contains("CURRENTSEASON"))
            {
                DebugCallbackMethod(" |MAIN| " + "CLIENT ASKED CURRENT SEASON: " + message);
                ParseAnimeSeason("http://myanimelist.net/anime/season", "Current");
                if (animePerSeasonJson != "")
                {
                    httpserver.JsonToSend(animePerSeasonJson);
                }

            }
            else if (message.Contains("GETALLSEASONS"))
            {

                DebugCallbackMethod(" |MAIN| " + "CLIENT REQUESTED ALL SEASONS: " + message);
                ParseAllSeasons();
                if(allSeasonsJson != "")
                {
                    httpserver.JsonToSend(allSeasonsJson);
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
                    httpserver.JsonToSend(animePerSeasonJson);
                }

            }
            else if (message.Contains("SEARCHANIME"))
            {
                string[] parts = message.Split('~');
                string searchUrl = parts[1];
                DebugCallbackMethod(" |MAIN| " + "CLIENT ASKED FOR SEARCH: " + searchUrl);

                ParseSearchAnime(searchUrl);
                if (animePerSeasonJson != "")
                {
                    httpserver.JsonToSend(searchedAnimeJson);
                }

            }
            else if (message.Contains("GETLOCALFILES"))
            {
                httpserver.SendMessage(allFiles);
                httpserver.SendMessage("LOCALFILESRECEIVED");

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

                        irc.setupIrc(server, 6667, username, "", channel, SendIRCMessageToHttpServer);
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
                Regex regex1 = new Regex(@"(?<=irc:)(.*\n?)");
                Match matches1 = regex1.Match(message);
                if (matches1.Success)
                {
                    string msg = matches1.Value;
                    irc.sendMessage(msg);
                }
            }
            else if (message.Contains("ISCLIENTRUNNING"))
            {
                if (loggedIn)
                {
                    httpserver.SendMessage("loginsucces");
                } else
                {
                    try
                    {
                        if (irc != null)
                        {
                            if (irc.isClientRunning())
                            {
                                httpserver.SendMessage("loginsucces");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        httpserver.SendMessage("loginfailed");
                    }
                }
                
            }
            else if (message.Contains("IRCCLOSE") || message.Contains("disconnect"))
            {
                loggedIn = false;
                try
                {
                    if (irc != null)
                    {
                        if (irc.isClientRunning())
                        {
                            irc.stopClient();
                            httpserver.SendMessage("loginfailed");
                        }
                    }
                }
                catch (Exception e)
                {
                    httpserver.SendMessage("loginsucces");
                }
            }
        }

        /// <DownloadStatusChange>
        /// <summary>
        ///  Irc on download status chaned event, sends, when connected to an irc server, the update on the file download to the http server, which sends it to the webclient
        /// </summary>
        /// </DownloadStatusChange>
        private static void DownloadStatusChange()
        {
            if (irc != null)
            {
                if (irc.isClientRunning())
                {
                    string filename = irc.getDownloadProgress("filename").ToString();
                    string status = irc.getDownloadProgress("status").ToString();
                    string size = irc.getDownloadProgress("size").ToString();
                    string progress = irc.getDownloadProgress("progress").ToString();
                    string speed = irc.getDownloadProgress("kbps").ToString();
                    string pack = irc.getDownloadProgress("pack").ToString();
                    string dlstatusstring = "download:~" + downloads.ToString() + "~" + filename + "~" + status + "~" + progress + "~" + speed + "~/Downloads/" + filename + "?" + size + "~" + pack + "~" + doesOSMCExist + "~THIS IS JUNK";
                    httpserver.SendMessage(dlstatusstring);

                    if (Int32.Parse(progress) > 5 && !parsedSubtitle)
                    {
                         GetSubtitle(filename);
                         parsedSubtitle = true;
                    } else if(Int32.Parse(progress) < 1)
                    {
                        parsedSubtitle = false;
                    }

                    if (previousstat != status && status == "DOWNLOADING")
                    {
                        downloads++;
                    }
                    previousstat = status;
                }
            }
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

        private static void GetSubtitle(string file)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            while (!File.Exists(filename + ".ass"))
            {
                if (OS == "Windows")
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/mkvextract.exe";
                    proc.StartInfo.Arguments = "tracks \"" + AppDomain.CurrentDomain.BaseDirectory + "\\GUI\\Downloads\\" + file + "\" 2:\"" + AppDomain.CurrentDomain.BaseDirectory + "\\GUI\\Downloads\\" + filename + ".ass\"";
                    proc.Start();
                    proc.WaitForExit();
                    DebugCallbackMethod(" |MAIN| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "\\GUI\\Downloads\\" + filename + ".ass\"");
                }
                else
                {
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "/mkvextract.exe";
                    proc.StartInfo.Arguments = "tracks \"" + AppDomain.CurrentDomain.BaseDirectory + "/GUI/Downloads/" + file + "\" 2:\"" + AppDomain.CurrentDomain.BaseDirectory + "/GUI/Downloads/" + filename + ".ass\"";
                    proc.Start();
                    proc.WaitForExit();
                    DebugCallbackMethod(" |MAIN| " + "Subtitle Created at: " + AppDomain.CurrentDomain.BaseDirectory + "/GUI/Downloads/" + Path.GetFileNameWithoutExtension(filename) + ".ass\"");
                }
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
            string[] filePaths = Directory.GetFiles(dldir);

            try
            {
                foreach (string filePath in filePaths)
                {
                    string filename = Path.GetFileName(filePath);
                    if(!filename.Contains(".ass") || !filename.Contains(".exe"))
                    {
                        FileInfo f = new FileInfo(filePath);
                        long s1 = f.Length;
                        allFiles = allFiles + "\"download:~" + downloads.ToString() + "~" + filename + "~COMPLETED~100~0~/Downloads/" + filename + "~0000~" + doesOSMCExist + "~THIS IS JUNK\",";
                        downloads++;
                        DebugCallbackMethod(" |MAIN| " + "Existing Files Found: " + filename);
                    }
                
                }

                allFiles = allFiles.Substring(1, allFiles.Length - 3);
            }
            catch
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
                    System.Diagnostics.Process.Start("http://" + GetLocalIPAddress() + ":" + port.ToString());
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
