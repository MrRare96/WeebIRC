using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace WeebIRCServerTray
{
    class HttpServer
    {

        public TcpListener listener;
        public Action<string> MessageReceivedCallback = null;
        public Action<string> DebugCallbackMethod = null;
        public string jsonDataToSend = "\"NOMESSAGES\",";
        public string rawJsonToSend = "[\"NOMESSAGES\"]";
        public string homeDir = "";
        public string fileDir = "";
        public string defaultPage = "";
        public Thread runServer;
        public static bool disconnect = false;

        public HttpServer()
        {
            this.MessageReceivedCallback = null;
            disconnect = false;
            runServer = new Thread(new ThreadStart(() => ServerLogic(8080)));
            runServer.Start();
        }

        public HttpServer(int port)
        {
            this.MessageReceivedCallback = null;
            disconnect = false;
            runServer = new Thread(new ThreadStart(() => ServerLogic(port)));
            runServer.Start();
        }

        public HttpServer(Action<string> MessageReceivedCallback)
        {
            this.MessageReceivedCallback = MessageReceivedCallback;
            disconnect = false;
            runServer = new Thread(new ThreadStart(() => ServerLogic(8080)));
            runServer.Start();
        }

        public HttpServer(Action<string> MessageReceivedCallback, int port)
        {

            this.MessageReceivedCallback = MessageReceivedCallback;
            disconnect = false;
            runServer = new Thread(new ThreadStart(() => ServerLogic(port)));
            runServer.Start();
        }

        public HttpServer(Action<string> DebugCallbackMethod, Action<string> MessageReceivedCallback, int port)
        {
            this.DebugCallbackMethod = DebugCallbackMethod;
            this.MessageReceivedCallback = MessageReceivedCallback;
            disconnect = false;
            runServer = new Thread(new ThreadStart(() => ServerLogic(port)));
            runServer.Start();
        }

        public void StopServer(){
            try
            {
                listener.Stop();

            } catch(Exception ex)
            {
                DebugCallbackMethod("|HTTP| Failed to stop listener: " + ex.ToString());
            }
            disconnect = true;
        }

        public void SetWebHomeDir(string dir)
        {
            homeDir = dir;
        }

        public void SetFileDir(string dir)
        {
            fileDir = dir;
        }
        
        public void SetDefaultPage(string fileName){
            defaultPage = fileName;
        }

        public void SendMessage(string msg)
        {
            jsonDataToSend = jsonDataToSend + "\"" + msg + "\",";
        }

        public void JsonToSend(string json)
        {
            rawJsonToSend = json;
        }

        public void ServerLogic(int port)
        {

       
            
            
            
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            
            while (!disconnect)
            {

                NetworkStream stream;
                StreamReader rd;
                StreamWriter wr;
                try
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    {
                        bool isRangeRequest = false;

                        try
                        {
                            stream = client.GetStream();
                            rd = new StreamReader(stream);
                            wr = new StreamWriter(stream);

                            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                            //DebugCallbackMethod("|HTTP|Client: " + clientIP + " Connected!");
                        }
                        catch (Exception e)
                        {
                            wr = null;
                            rd = null;
                            stream = null;
                            DebugCallbackMethod("|HTTP|COULD NOT START LISTENING: " + e.ToString());
                        }

                        if (rd != null && wr != null && client != null && stream != null)
                        {
                            string currentLine;
                            string[] headers = new string[100];
                            int i = 0;
                            while ((currentLine = rd.ReadLine()) != "")
                            {
                                try
                                {
                                    headers[i] += currentLine + Environment.NewLine;
                                    i++;
                                }
                                catch
                                {
                                    break;
                                }

                            }
                            long start = 0;
                            long end = -1;
                            string fileName = "";
                            string message = "";
                            foreach (string header in headers)
                            {
                                if (header != null)
                                {


                                    if (header.Contains("GET"))
                                    {
                                        if (header.Contains("message="))
                                        {
                                            message = HttpUtility.UrlDecode(header.Split(new string[] { "message=" }, StringSplitOptions.None)[1].Split(new string[] { "HTTP" }, StringSplitOptions.None)[0]);
                                        }
                                        else if (header.Contains(" / "))
                                        {
                                            fileName = defaultPage;
                                        }
                                        else
                                        {
                                            fileName = HttpUtility.UrlDecode(header.Split(' ')[1].Split(' ')[0].Trim());
                                            //DebugCallbackMethod(header);
                                        }
                                    }

                                    if (header.ToLower().Contains("range") && !IsWebPageFile(fileName) && !IsStylingFile(fileName) && !IsSubtitleFile(fileName) && !IsFontFile(fileName) && !IsImageFile(fileName))
                                    {
                                        try
                                        {
                                            start = int.Parse(header.Split('=')[1].Split('-')[0].Trim());
                                            try
                                            {
                                                end = int.Parse(header.Split('=')[1].Split('-')[1].Trim());
                                            }
                                            catch (Exception e2)
                                            {
                                            }
                                            DebugCallbackMethod(header + ", RANGE REQUEST :D");
                                        }
                                        catch (Exception e)
                                        {
                                        }

                                        isRangeRequest = true;
                                    }

                                    if (header.Contains("message="))
                                    {
                                        message = HttpUtility.UrlDecode(header.Split(new string[] { "message=" }, StringSplitOptions.None)[1].Split(new string[] { "HTTP" }, StringSplitOptions.None)[0]);
                                    }
                                }
                            }

                            if (isRangeRequest)
                            {
                                DebugCallbackMethod("|HTTP|SENDING RANGE STREAM!");
                                if (File.Exists(fileDir + "/" + fileName))
                                {
                                    using (FileStream fileStream = new FileStream(fileDir + "/" + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    {

                                        if (end < 0)
                                        {
                                            end = fileStream.Length;
                                        }
                                        byte[] buffer = new byte[end - start];
                                        fileStream.Position = start;

                                        int read = fileStream.Read(buffer, 0, (int)(end - start));

                                        StringBuilder responseHeader = new StringBuilder();
                                        responseHeader.Append("HTTP/1.1 206 Partial Content \r\n");
                                        responseHeader.Append("Access-Control-Allow-Origin: *\r\n");
                                        responseHeader.Append("Connection: keep-alive \r\n");
                                        responseHeader.Append("Content-Type: video/mp4 \r\n");
                                        responseHeader.Append("Accept-Ranges: bytes \r\n");
                                        int totalCount = (int)fileStream.Length;
                                        responseHeader.Append(string.Format("Content-Range: bytes {0}-{1}/{2}", start, totalCount - 1, totalCount) + "\r\n");
                                        responseHeader.Append("Content-Length: " + buffer.Length.ToString() + "\r\n");
                                        responseHeader.Append("X-Content-Duration: 0.00 \r\n");
                                        responseHeader.Append("Content-Duration: 0.00 \r\n\r\n");
                                        try
                                        {
                                            wr.Write(responseHeader);
                                            wr.Flush();
                                            stream.Write(buffer, 0, buffer.Length);
                                            stream.Flush();
                                            wr.Close();
                                            rd.Close();
                                            stream.Close();
                                            client.Close();

                                        }
                                        catch
                                        {

                                        }

                                        fileStream.Flush();
                                        fileStream.Close();
                                    }

                                }
                                else
                                {
                                    DebugCallbackMethod("|HTTP|stream file not found: " + fileDir + "/" + fileName);

                                    StringBuilder responseHeader = new StringBuilder();
                                    responseHeader.Append("HTTP/1.1 404 Not Found \r\n\r\n");
                                    wr.Write(responseHeader);
                                    wr.Flush();
                                    wr.Close();
                                    rd.Close();
                                    stream.Close();
                                    client.Close();
                                }

                            }
                            else if (message != "")
                            {
                                if (MessageReceivedCallback != null)
                                {
                                    MessageReceivedCallback(message);

                                    StringBuilder str = new StringBuilder();
                                    str.Append("HTTP/1.1 200 OK\r\n");
                                    str.Append("Access-Control-Allow-Origin: *\r\n");
                                    str.Append("Content-Type: application/json \r\n");
                                    str.Append("Connection: keep-alive\r\n");
                                    str.Append("\r\n");
                                    str.Append("{\"messages\" : [" + jsonDataToSend.Substring(0, jsonDataToSend.Length - 1) + "], \"rawjson\" : " + rawJsonToSend + "}");
                                    try
                                    {
                                        wr.Write(str);
                                        wr.Flush();
                                        wr.Close();
                                        rd.Close();
                                        stream.Close();
                                        client.Close();
                                        if (rawJsonToSend != "[\"NOMESSAGES\"]")
                                        {

                                            rawJsonToSend = "[\"NOMESSAGES\"]";
                                        }
                                        if (jsonDataToSend != "\"NOMESSAGES\",")
                                        {

                                            jsonDataToSend = "\"NOMESSAGES\",";
                                        }
                                    }
                                    catch
                                    {

                                        DebugCallbackMethod("|HTTP|Could not write data to client :*(. ");
                                    }
                                }
                                else
                                {
                                    DebugCallbackMethod("|HTTP|stream file not found: " + fileDir + "/" + fileName);
                                    StringBuilder responseHeader = new StringBuilder();
                                    responseHeader.Append("HTTP/1.1 404 Not Found \r\n\r\n");
                                    wr.Write(responseHeader);
                                    wr.Flush();
                                    wr.Close();
                                    rd.Close();
                                    stream.Close();
                                    client.Close();
                                }
                            }
                            else
                            {

                                string mimeType = "text/html";
                                string correctDir = homeDir;
                                if (IsFontFile(fileName))
                                {
                                    mimeType = "font/opentype";
                                }
                                if (IsWebPageFile(fileName))
                                {
                                    mimeType = "text/html";
                                }
                                if (IsSubtitleFile(fileName))
                                {
                                    mimeType = "text/plain";
                                    correctDir = fileDir;
                                }
                                if (IsStylingFile(fileName))
                                {
                                    mimeType = "text/css";
                                }

                                if (IsMediaFile(fileName))
                                {
                                    mimeType = "application/octet-stream";
                                }

                                if (IsImageFile(fileName))
                                {
                                    mimeType = "image/svg+xml";
                                }
                                DebugCallbackMethod("|HTTP|MimeType: " + mimeType + ", file: " + correctDir + "/" + fileName);
                                if (File.Exists(correctDir + "/" + fileName))
                                {




                                    byte[] buffer = File.ReadAllBytes(correctDir + "/" + fileName);

                                    StringBuilder responseHeader = new StringBuilder();
                                    responseHeader.Append("HTTP/1.1 200 OK \r\n");
                                    responseHeader.Append("Access-Control-Allow-Origin: *\r\n");
                                    responseHeader.Append("Content-Type: " + mimeType + " \r\n");
                                    responseHeader.Append("Connection: close \r\n");
                                    responseHeader.Append("Content-Length: " + buffer.Length.ToString() + "\r\n\r\n");

                                    try
                                    {
                                        wr.Write(responseHeader);
                                        wr.Flush();
                                        stream.Write(buffer, 0, buffer.Length);
                                        stream.Flush();
                                    }
                                    catch
                                    {
                                        DebugCallbackMethod("|HTTP|Client just pissed off to somewere... ha... good riddance ... ( ͡°⍘ ͡°)");
                                    }
                                    wr.Close();
                                    rd.Close();
                                    stream.Close();
                                    client.Close();
                                }
                                else
                                {
                                    DebugCallbackMethod("|HTTP|Could not find file: " + fileName);
                                    StringBuilder responseHeader = new StringBuilder();
                                    responseHeader.Append("HTTP/1.1 404 Not Found \r\n\r\n");
                                    wr.Write(responseHeader);
                                    wr.Flush();
                                    wr.Close();
                                    rd.Close();
                                    stream.Close();
                                    client.Close();
                                }
                            }
                        }
                    }
                }  catch( Exception ex)
                {
                    DebugCallbackMethod("|HTTP| Finally listener stopped.. " + ex.ToString());
                    Thread.Sleep(1000);
                    break;
                }
                
            }
            
        }

        public bool IsStylingFile(string filename)
        {
            string[] fileExtensions = new string[] { ".css" };
            string extension = Path.GetExtension(filename);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public  bool IsWebPageFile(string filename)
        {
            string[] fileExtensions = new string[] {".html", ".htm", ".js"};
            string extension = Path.GetExtension(filename);
            //DebugCallbackMethod("|HTTP|File Extension: " + extension);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                //DebugCallbackMethod("|HTTP|FILE IS HTML FILE");
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool IsSubtitleFile(string filename)
        {
            string[] fileExtensions = new string[] { ".ass", ".vtt", ".srt" };
            string extension = Path.GetExtension(filename);
            DebugCallbackMethod("|HTTP|SubFile Extension: " + extension);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                DebugCallbackMethod("|HTTP|FILE IS SUB FILE");
                return true;
            }
            else
            {
                 DebugCallbackMethod("|HTTP|FILE IS NOT SUB FILE");
                return false;
            }
        }

        public bool IsFontFile(string filename)
        {
            string[] fileExtensions = new string[] { ".woff", ".woff2", ".eot", ".tff" };
            string extension = Path.GetExtension(filename);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool IsMediaFile(string filename)
        {
            string[] fileExtensions = new string[] { ".mkv", ".mp4", ".avi", ".flak", ".mp3", ".aac"  };
            string extension = Path.GetExtension(filename);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public bool IsImageFile(string filename){
            string[] fileExtensions = new string[] {".jpg", ".png", ".gif", ".svg", ".bmp"};
             string extension = Path.GetExtension(filename);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if (inArray > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
