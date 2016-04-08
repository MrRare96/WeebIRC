using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleIRCLibMono;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace WeebIRCWebEdition
{
    class Program
    {
        private static HttpServer httpserver = null;
        private static SimpleIRC irc = null;
        private static string dldir = "";
        private static int downloads = 0;
        private static string allFiles = "";
        private static string doesOSMCExist = "false";
        private static int port = 8008;

        static void Main(string[] args)
        {


            Console.WriteLine("URL To use in your webbrowser: " + GetLocalIPAddress() + ":" + port.ToString());


            dldir = Directory.GetCurrentDirectory() + @"/GUI/Downloads/";
            if (!Directory.Exists(dldir))
            {
                Console.WriteLine("Could not find download directory: " + dldir);
                Directory.CreateDirectory(dldir);
                Console.WriteLine("Download Directory: " + dldir + " created!");
            } else
            {
                Console.WriteLine("CURRENT DL DIR: " + dldir);
            }


            if (Directory.Exists("/home/osmc"))
            {
                Console.WriteLine("YOUR SERVER IS RUNNING ON OSMC!!!");
                doesOSMCExist = "true";
            }

            irc = new SimpleIRC();
            httpserver = new HttpServer(8008);
            httpserver.SetMessageReceivedCallback(HttpMessageReceived);


            if(RunningPlatform() == Platform.Windows)
            {
                Console.WriteLine("RUNNING ON WINDOWS, ASSUME LOCAL CLIENT, OPENING INTERFACE IN BROWSER!");
                System.Diagnostics.Process.Start("http://" + GetLocalIPAddress() + ":" + port.ToString());
            }

            string[] filePaths = Directory.GetFiles(dldir);

            try
            {
                foreach (string filePath in filePaths)
                {
                    string filename = Path.GetFileName(filePath);
                    FileInfo f = new FileInfo(filePath);
                    long s1 = f.Length;
                    allFiles = allFiles + "\"download:~" + downloads.ToString() + "~" + filename + "~COMPLETED~100~0~/Downloads/" + filename + "?" + s1 + "~0000~" + doesOSMCExist + "~THIS IS JUNK\",";
                    downloads++;
                    Console.WriteLine("EXISTING FILES FOUND: " + filename);
                }

                allFiles = allFiles.Substring(1, allFiles.Length - 3);
            } catch 
            {
                Console.WriteLine("No Files Found ");
            }
            

            while (true)
            {
                string toSend = Console.ReadLine();
                if(toSend != "")
                {
                    httpserver.SendMessage(toSend);
                }
            }
            Console.ReadLine();
        }
        private static string previousstat = "";

        private static void downloadStatusChange()
        {
            if(irc != null)
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
                    if (previousstat != status && status == "DOWNLOADING")
                    {
                        downloads++;
                    }
                    previousstat = status;
                }
            }
        }

        private static void debug(string debug)
        {
            Console.WriteLine("==== IRC DEBUG === \n " + debug + "\n ================ ");
        }

        private static bool loggedIn = false;
        private static void sendMessageToWssServer(string user, string message)
        {

            debug("MSG TO HTTP COM: " + message);
            httpserver.SendMessage("irc:" + user + " : " + message);
            if (!loggedIn)
            {
                loggedIn = true;
                httpserver.SendMessage("loginsucces");
            }
        }
        private static void HttpMessageReceived(string message)
        {
            Console.WriteLine("Received from HTTP server: " + message);
            if (message.Contains("ISCONNECTED"))
            {
                httpserver.SendMessage("ISCONNECTED");
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
                    try
                    {
                        if (irc.isClientRunning())
                        {
                            irc.shouldClientStop = true;
                            irc.stopClient();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if (server != "")
                    {
                        debug("setting up irc connection with following information: \n server: " + server + " \n channel: " + channel + "\n username: " + username);
                        irc.setupIrc(server, 6667, username, "", channel, sendMessageToWssServer);
                        irc.setCustomDownloadDir(dldir);
                        irc.setDownloadStatusChangeCallback(downloadStatusChange);
                        irc.setDebugCallback(debug);
                        debug("starting irc connection");
                        irc.startClient();
                        debug("client is running ;D");
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
            else if (message.Contains("IRCCLOSE"))
            {
                loggedIn = false;
                try
                {
                    if (irc != null)
                    {
                        if (irc.isClientRunning())
                        {
                            loggedIn = false;
                            irc.shouldClientStop = true;
                            irc.stopClient();
                        }
                    }
                }
                catch (Exception e)
                {
                    httpserver.SendMessage("failed connect");
                }
            }
            else if (message.Contains("ISCLIENTRUNNING"))
            {
                loggedIn = false;
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
                    httpserver.SendMessage("failed connect");
                }
            }
            else if (message.Contains("disconnect"))
            {
                loggedIn = false;
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
                    httpserver.SendMessage("failed connect");
                }

            }
        }

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

        public enum Platform
        {
            Windows,
            Linux,
            Mac
        }

        private static Platform RunningPlatform()
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
                        return Platform.Mac;
                    else
                        return Platform.Linux;

                case PlatformID.MacOSX:
                    return Platform.Mac;

                default:
                    return Platform.Windows;
            }
        }
    }

    
}
