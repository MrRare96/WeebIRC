using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WeebIRC
{
    class DCCClient
    {

        //Member vars

        public static bool bussy = false;
        public static bool abortDl = false;

        private string newDccString;
        private string newFileName;
        private long newIpAddress;
        private int newPortNum;
        private int newFileSize;
        private string newIp;
        private string[] dccparts;
        private bool Continue;
        private Form1 form;
        //default constructor
        public DCCClient()
        {
            newDccString = String.Empty;
        }

        //overload constructor
        public DCCClient(string dccString, Form1 formIn)
        {
            newDccString = dccString;
            form = formIn;
        }

        //Accessor functions
        public string getFileName()
        {
            return newFileName;
        }

        public int getFileSize()
        {
            return newFileSize;
        }

        public int getPortNum()
        {
            return newPortNum;
        }

        public string getIp()
        {
            return newIp;
        }

        //program functions

        //strips the response by a dcc server/bot into parts
        public void getDccParts()
        {
            /*
             * :bot PRIVMSG nickname :DCC SEND \"filename\" ip_networkbyteorder port filesize
            */

            int posStart = newDccString.IndexOf("SEND");
            string dccstringp1 = newDccString.Substring(posStart + 5);

            string[] fileExtensions = { ".mkv", ".mp4", ".avi", ".mp3", ".pdf", ".jpg" };

            int posEnd = 0;

            string extensionUsed = "";
            foreach(string extension in fileExtensions)
            {
                if (dccstringp1.IndexOf(extension) != 0)
                {
                    posEnd = dccstringp1.IndexOf(extension);
                    extensionUsed = extension;
                    break;
                }
            }
            
            
            newFileName = dccstringp1.Substring(0, posEnd + 4);

            if (newFileName.Contains("\""))
            {
                newFileName = newFileName.Substring(1);
            }

            form.AppendDebug("FILE NAME: " + newFileName);
            
            string dccstring = Regex.Split(dccstringp1, extensionUsed)[1].Substring(2);

            dccparts = dccstring.Split(' ');

            if (dccparts[0] != "")
            {
                string[] tempArray = { "", dccparts[0], dccparts[1], dccparts[2] };

                dccparts = tempArray;
            }




            if (!form.batchMode)
            {
                if (form.perDownloadAFolder)
                {
                    string[] removeExtParts = newFileName.Split('.');
                    string FileNameWithoutExt = removeExtParts[removeExtParts.Length - 2];

                    if (!Directory.Exists(form.downloadFolder + "\\" + FileNameWithoutExt))
                    {
                        Directory.CreateDirectory(form.downloadFolder + "\\" + FileNameWithoutExt);
                    }
                    form.fileLocation = form.downloadFolder + "\\" + newFileName;
                } else
                {
                    form.fileLocation = form.downloadFolder + "\\" + newFileName;
                }

                form.appendToDownloadList(newFileName, IrcSend.packNumber.Substring(1), IrcSend.bot, form.fileLocation);
            }

            int i = 0;
            foreach(string dccSPart in dccparts)
            {
                form.AppendDebug("dcc part " + i.ToString() + ": " + dccSPart);
                i++;
            }

        }

        //sets all the variables needed for a dcc transfer, parts are from getDccParts()
        public void setDccValues()
        {

            if (IrcConnect.newChannel == "RareIRC")
            {
                try
                {
                    Continue = true;
                    newFileName = dccparts[5];
                    newIpAddress = Convert.ToInt64(dccparts[6]);
                    newPortNum = Convert.ToInt32(dccparts[7]);

                    IPEndPoint hostIPEndPoint = new IPEndPoint(newIpAddress, newPortNum);
                    string[] ipadressinfoparts = hostIPEndPoint.ToString().Split(':');
                    string[] ipnumbers = ipadressinfoparts[0].Split('.');
                    string ip = ipnumbers[3] + "." + ipnumbers[2] + "." + ipnumbers[1] + "." + ipnumbers[0];
                    newIp = ip;
                }
                catch
                {
                    form.AppendIrcOutput("Could not set values: ERROR NO IP ADDRESS RETRIEVED!");
                    Continue = false;
                }
            }
            else
            {
                //Continue = true;
                try
                {
                    newIpAddress = Convert.ToInt64(dccparts[1]);
                    newPortNum = Convert.ToInt32(dccparts[2]);
                    IPEndPoint hostIPEndPoint = new IPEndPoint(newIpAddress, newPortNum);
                    string[] ipadressinfoparts = hostIPEndPoint.ToString().Split(':');
                    string[] ipnumbers = ipadressinfoparts[0].Split('.');
                    string ip = ipnumbers[3] + "." + ipnumbers[2] + "." + ipnumbers[1] + "." + ipnumbers[0];
                    newIp = ip;
                    Continue = true;
                    //.Substring(0, dccparts[3].Length - 1)
                    string filesizes = Regex.Match(dccparts[3], @"\d+").Value; ;
                    newFileSize = Convert.ToInt32(filesizes);
                }
                catch
                {
                    form.AppendIrcOutput("Could not set values: ERROR NO IP ADDRESS RETRIEVED!");
                    Continue = false;
                }

            }
        }

        //starts the download thread, thread needed to keep cmd window responsive



        public void StartDownload()
        {
            try
            {
                if (Continue)
                {
                    Thread downloader = new Thread(new ThreadStart(this.Downloader));
                }
                else
                {
                    form.AppendIrcOutput("Could not start downloader: false given on continue!");
                }
            }
            catch
            {
                form.AppendIrcOutput("Could not start downloader: false given on continue!");
            }
        }

        //creates a tcp socket connection for the retrieved ip/port from the dcc ctcp by the dcc bot/server

        public void Downloader() { 
        
            try {

                string fileLocation = form.getFileLocation(newFileName).Trim();
                form.AppendDebug("filelocation for download: " + fileLocation);

                if(!File.Exists(fileLocation))
                {
                    

                    TcpClient dltcp = new TcpClient(newIp, newPortNum);
                    NetworkStream dlstream = dltcp.GetStream();

                    Int64 bytesReceived = 0;
                    Int64 oldBytesReceived = 0;
                    Int64 oneprocent = newFileSize / 100;
                    Int64 done = 0;
                    int count;
                    byte[] buffer = new byte[1048576];

                    FileStream writeStream = new FileStream(fileLocation, FileMode.Append, FileAccess.Write);

                    DateTime start = DateTime.Now;

                    bussy = true;

                    bool timedOut = false;
                    abortDl = false;

                    while (dltcp.Connected && bytesReceived < newFileSize && !form.StopProgram)
                    {
                        count = dlstream.Read(buffer, 0, buffer.Length);
                        DateTime end = DateTime.Now;
                        if (start.Second != end.Second)
                        {

                            Int64 Bytes_Seconds = bytesReceived - oldBytesReceived;
                            done = bytesReceived / oneprocent;
                            Int64 KBytes_Seconds = Bytes_Seconds / 1024;
                            Int64 MBytes_Seconds = KBytes_Seconds / 1024;
                            form.updateFileStatus(newFileName, done, KBytes_Seconds);
                            oldBytesReceived = bytesReceived;
                            start = DateTime.Now;
                        }
                        writeStream.Write(buffer, 0, count);

                        bytesReceived += count;
                        int timeOut = 0;


                        while (!dlstream.DataAvailable)
                        {
                            if (timeOut == 1000)
                            {
                                break;  
                            } else if (!dltcp.Connected)
                            {
                                break;
                            }
                            timeOut++;
                            Thread.Sleep(1);  
                        }

                        if(timeOut == 1000)
                        {
                            form.AppendDebug("TIMEOUT ON DATA RETREIVAL");
                            timedOut = true;
                            break;
                        }

                        if (abortDl)
                        {
                            break;
                        }
                    
                    }

                    dltcp.Close();
                    dlstream.Dispose();
                    writeStream.Close();
                    bussy = false;


                    if (abortDl && done < 95)
                    {
                        abortDl = false;
                        File.Delete(fileLocation);
                        form.updateFileStatus(newFileName, 103, 0);
                    }
                    else if (timedOut && done < 95)
                    {
                        File.Delete(fileLocation);
                        form.updateFileStatus(newFileName, 102, 0);
                    }
                     else
                    {
                        form.updateFileStatus(newFileName, 101, 0);
                    }

                } else
                {
                    IrcSend.sendMsg("/msg " + IrcSend.bot + " xdcc remove " + IrcSend.packNumber, form);
                    IrcSend.sendMsg("/msg " + IrcSend.bot + " xdcc cancel", form);
                    form.updateFileStatus(newFileName, 101, 0);
                }
            } catch (Exception e){
                form.updateFileStatus(newFileName, 102, 0);
                form.AppendDebug("couldnt start download :X");
                form.AppendDebug(e.ToString());
            }
            
        }


        
    }
}
