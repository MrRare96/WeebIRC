using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace WeebIRCWebEdition
{
    class HttpServer
    {

        private Thread ServerThread;
        private Thread FileServerThread;
        private Thread ComServerThread;
        public int port { get; set; }
        public int dlPort { get; set; }
        public int comPort { get; set; }
        public object Server { get; private set; }

        public string clientIp;
        public Action<string> MessageReceivedCallback;
        public string jsonDataToSend = "\"NOMESSAGES\",";

        public HttpServer(int port)
        {
            this.port = port;
            dlPort = port + 10;
            comPort = dlPort + 10;

            ComServerThread = new Thread(new ThreadStart(() => communicationServer()));
            ComServerThread.Start();
            ServerThread = new Thread(new ThreadStart(() => ServerThreadProgram()));
            ServerThread.Start();
            FileServerThread  = new Thread(new ThreadStart(() => sendLargeFile()));
            FileServerThread.Start();
        }
        

        public void SetMessageReceivedCallback(Action<string> callback)
        {
            MessageReceivedCallback = callback;
        }

        public string GetLocalIPAddress()
        {
            string thirdPartOfIP = clientIp.Split('.')[2];
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                try
                {
                    string thirdPartOfIPlocal = ip.ToString().Split('.')[2];
                    if (thirdPartOfIPlocal == thirdPartOfIP)
                    {
                        Console.WriteLine("FOUND LOCAL SERVER IP CLIENT CONNECTED TO: " + ip.ToString());
                        return ip.ToString();
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Could not find " + clientIp + " in interfaces :X");
                    Console.WriteLine(e.ToString());
                }
               
            }
            throw new Exception("Local IP Address Not Found!");
            

           
        }

        public void SendMessage(string msg)
        {
            jsonDataToSend = jsonDataToSend + "\"" + msg + "\",";
        }

        public void StopServer()
        {
            ServerThread.Abort(); ComServerThread.Abort(); FileServerThread.Abort();
        }

        private void ServerThreadProgram()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);

            server.Start();

            string receive = "";

            while (true)
            {
                receive = "";
                Console.WriteLine("HTTP WEB Server has started on " + IPAddress.Any.ToString() + ":" + port.ToString() + ".{0}Waiting for a connection...", Environment.NewLine);

                TcpClient client = server.AcceptTcpClient();
                clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                Console.WriteLine("A client connected from " + clientIp + ".");
                MessageReceivedCallback("NEW CLIENT");
                NetworkStream stream = client.GetStream();

                StreamReader rd = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                try
                {
                    receive = rd.ReadLine().Replace("%20", " ");
                    if (receive != null)
                    {
                        if (receive.Contains("GET"))
                        {
                            Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                            Match matches1 = regex1.Match(receive);
                            string fileToSend = "";
                            string filesize = "";




                            StringBuilder str = new StringBuilder();
                            str.Append("HTTP/1.1 200 OK\r\n");
                            str.Append("Access-Control-Allow-Origin: *\r\n");
                            str.Append("Connection: close\r\n");
                            str.Append("\r\n");

                            string stufftosend = "";

                            if (matches1.Success)
                            {

                                {
                                    if (!(matches1.Value == "/"))
                                    {
                                        if (matches1.Value.Contains("isthisaserver"))
                                        {
                                            Console.WriteLine("HTTP: A client searched and found this server :D!");
                                            str.Append("YUP");
                                            sw.Write(str);
                                            sw.Flush();
                                        }
                                        else if (matches1.Value.Contains("=") && !matches1.Value.Contains("?url:"))
                                        {
                                            string[] fileinfo = matches1.Value.Split('=');
                                            Console.WriteLine("\n File size: " + fileinfo[1] + "\n");
                                            Console.WriteLine("\n File requested: " + fileinfo[0] + "\n");
                                            fileToSend = fileinfo[0].Substring(1);
                                            filesize = fileinfo[1];
                                        }
                                        else if (matches1.Value.Contains("?url:"))
                                        {
                                            Console.WriteLine("HTTP: RECEIVED HTML DATA REQUEST :D : " + matches1.Value.Substring(1).Replace("?url:", ""));
                                            using (WebClient webclient = new WebClient())
                                            {
                                                stufftosend = webclient.DownloadString(matches1.Value.Substring(1).Replace("?url:", "").Trim());
                                                Console.WriteLine("HTTP: data length: " + stufftosend.Length);
                                                str.Append(stufftosend);
                                                sw.Write(str);
                                                sw.Flush();
                                            }
                                        }
                                        else
                                        {
                                            //fileToSend = @"home.html";
                                            fileToSend = matches1.Value.Substring(1);
                                            Console.WriteLine(" HTTP: File Requested: " + fileToSend + "\n");
                                        }
                                    }
                                    else
                                    {
                                        fileToSend = @"home.html";
                                        /*stufftosend = WebPage.GetHtml("\"" + GetLocalIPAddress() + "\"");
                                        str.Append(stufftosend);

                                        sw.Write(str);
                                        sw.Flush();*/
                                    }
                                }


                            }
                            else
                            {
                                Console.WriteLine("HTTP: could not parse get request\n");
                                fileToSend = @"GUI\home.html";
                            }

                            Console.WriteLine("HTTP: Sending file: " + fileToSend + "\n");
                            if(fileToSend != "")
                            {
                                if (File.Exists(@"GUI\" + fileToSend) && (fileToSend.Contains(".html") || fileToSend.Contains(".tff") || fileToSend.Contains(".ico") || fileToSend.Contains(".woff2") || fileToSend.Contains(".woff") || fileToSend.Contains(".htm") || fileToSend.Contains(".css") || fileToSend.Contains(".js")))
                                {

                                    using (StreamReader sr = new StreamReader(@"GUI\" + fileToSend))
                                    {
                                        // Read the stream to a string, and write the string to the console.
                                        stufftosend = sr.ReadToEnd();
                                    }

                                    if (stufftosend.Contains("myIP"))
                                    {
                                        Console.WriteLine("HTTP: putting server ip in js");
                                        string addedIp = Regex.Replace(stufftosend, "myIP", "\"" + GetLocalIPAddress() + "\"");
                                        str.Append(addedIp);
                                    }
                                    else
                                    {

                                        str.Append(stufftosend);
                                    }



                                    sw.Write(str);
                                    sw.Flush();
                                }
                                else if (File.Exists(@"GUI/" + fileToSend) && (fileToSend.Contains(".html") || fileToSend.Contains(".tff") || fileToSend.Contains(".ttf") || fileToSend.Contains(".ico") || fileToSend.Contains(".woff2") || fileToSend.Contains(".woff") || fileToSend.Contains(".htm") || fileToSend.Contains(".css") || fileToSend.Contains(".js")))

                                {
                                    using (StreamReader sr = new StreamReader(@"GUI/" + fileToSend))
                                    {
                                        // Read the stream to a string, and write the string to the console.
                                        stufftosend = sr.ReadToEnd();
                                    }

                                    if (stufftosend.Contains("myIP"))
                                    {
                                        Console.WriteLine("HTTP: putting server ip in js");
                                        string addedIp = Regex.Replace(stufftosend, "myIP", "\"" + GetLocalIPAddress() + "\"");
                                        str.Append(addedIp);
                                    }
                                    else
                                    {

                                        str.Append(stufftosend);
                                    }



                                    sw.Write(str);
                                    sw.Flush();
                                }
                                else
                                {
                                    stufftosend = WebPage.GetHtml("\"" + GetLocalIPAddress() + "\"");
                                    str.Append(stufftosend);

                                    sw.Write(str);
                                    sw.Flush();
                                }

                            }

                            rd.Close();
                            sw.Close();
                            stream.Close();
                            client.Close();

                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("HTTP: string is null again: " + e.ToString());
                    rd.Close();
                    sw.Close();
                    stream.Close();
                    client.Close();

                }
                Thread.Sleep(1);
            }
        }

        private void sendLargeFile()
        {

            TcpListener server = new TcpListener(IPAddress.Any, dlPort);

            server.Start();
            while (true)
            {
                Thread.Sleep(1);
                Console.WriteLine("HTTP FS: HTTP FILE Server has started on " + IPAddress.Any.ToString() + ":" + dlPort.ToString() + ".{0}Waiting for a connection...", Environment.NewLine);

                TcpClient client = server.AcceptTcpClient();


                Console.WriteLine("HTTP FS: A client connected for download :D");
                NetworkStream stream = client.GetStream();

                StreamReader rd = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                string received = "";
                string file = "";
                string filesize = "0";

                try
                {
                    while ((received = rd.ReadLine()).Length != 0)
                    {

                        received = received.Replace("%20", " ");
                        Console.WriteLine(received);
                        if (received.Contains("GET"))
                        {
                            Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                            Match matches1 = regex1.Match(received);
                            file = matches1.Value.Trim().Substring(1).Split('?')[0];
                            filesize = matches1.Value.Trim().Substring(1).Split('?')[1];

                            Console.WriteLine("HTTP FS: File requested: " + file + "\n");
                        }
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("HTTP FS: RECEIVED NULL, IGNORING");
                }

                if (file != "")
                {
                    
                    string filepath = @"GUI/" + file;
                    if (File.Exists(filepath))
                    {
                        Console.WriteLine("\n HTTP FS: File location: " + filepath + "\n");
                        if(filesize == "0")
                        {
                            FileInfo f = new FileInfo(filepath);
                            filesize = f.Length.ToString();
                        }

                        StringBuilder str = new StringBuilder();
                        str.Append("HTTP/1.1 200 OK\r\n");
                        str.Append("Content-Type: application/octet-stream\r\n");
                        str.Append("Connection: close\r\n");
                        str.Append("Content-Length: " + filesize + "\r\n");
                        str.Append("Accept-Ranges: */*\r\n");
                        str.Append("\r\n");
                        sw.Write(str);
                        sw.Flush();
                        Console.WriteLine("\n HTTP FS: HEADERS SEND: " + filepath + "\n");
                        //
                        using (FileStream source = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] buffer = new byte[131072];
                            int bytesRead;
                            long bytesTotalRead = 0;
                            while (bytesTotalRead.ToString() != filesize)
                            {
                                Thread.Sleep(1);
                                bytesRead = source.Read(buffer, 0, buffer.Length);
                                if (stream.CanWrite && client.Connected)
                                {
                                    try
                                    {


                                        stream.Write(buffer, 0, bytesRead);
                                        stream.Flush();
                                        bytesTotalRead += bytesRead;

                                    }
                                    catch
                                    {
                                        Console.WriteLine("HTTP FS: Client aborted download :X");
                                        break;
                                    }
                                }
                                else
                                {

                                    Console.WriteLine("HTTP FS: Cant write to stream: " + filepath);
                                    break;
                                }
                            }
                            Console.WriteLine("HTTP FS: DONE UPLOADING FILE: " + filepath);
                            Console.WriteLine("HTTP FS: BYTES UPLOADED: " + bytesTotalRead.ToString() + " OF " + filesize);
                        }
                        rd.Close();
                        sw.Close();
                        stream.Close();
                        client.Close();


                    } else
                    {
                        rd.Close();
                        sw.Close();
                        stream.Close();
                        client.Close();
                        Console.WriteLine("HTTP FS: COULD NOT FIND FILE *(");
                    }
                }
            }
        }

        private void communicationServer()
        {

            TcpListener server = new TcpListener(IPAddress.Any, comPort);
            Console.WriteLine("HTTP COM: HTTP COM Server has started on " + IPAddress.Any.ToString() + ":" + comPort.ToString() + ".{0}Waiting for a connection...", Environment.NewLine);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                
                while (client.Connected)
                {
                    Console.WriteLine("client is connected");   


                    NetworkStream stream = client.GetStream();
                    StreamReader rd = new StreamReader(stream);
                    StreamWriter sw = new StreamWriter(stream);
                    Console.WriteLine("HTTP COM: A client connected for communication:D");

                    string received = "";
                    try
                    {
                        byte[] myReadBuffer = new byte[1024];
                        int numberOfBytesRead;
                        while ((numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length)) != 0)
                        {
                            received = received + Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead);
                            if (received.Contains("message="))
                            {

                                Console.WriteLine("MESSAGE FOUND!");
                                break;
                            }
                            myReadBuffer = new byte[1024];

                        }
                        received = received.Replace("+", " ");
                        string message = received.Split(new string[] { "message=" }, StringSplitOptions.None)[1];
                        MessageReceivedCallback(HttpUtility.UrlDecode(message));
                        if (message.Length > 0)
                        {
                            StringBuilder str = new StringBuilder();
                            str.Append("HTTP/1.1 200 OK\r\n");
                            str.Append("Access-Control-Allow-Origin: *\r\n");
                            str.Append("Connection: keep-alive\r\n");
                            str.Append("\r\n");
                            str.Append("{\"messages\" : [" + jsonDataToSend.Substring(0, jsonDataToSend.Length - 1) + "]}");
                            sw.Write(str);
                            sw.Flush();
                            if (jsonDataToSend != "\"NOMESSAGES\",")
                            {
                                jsonDataToSend = "\"NOMESSAGES\",";
                            }
                            rd.Close();
                            sw.Close();
                            stream.Close();
                        }
                        else
                        {
                            StringBuilder str = new StringBuilder();
                            str.Append("HTTP/1.1 200 OK\r\n");
                            str.Append("Access-Control-Allow-Origin: *\r\n");
                            str.Append("Connection: close\r\n");
                            str.Append("\r\n");
                            rd.Close();
                            sw.Close();
                            stream.Close();
                        }


                    }
                    catch (Exception e)
                    {
                        StringBuilder str = new StringBuilder();
                        str.Append("HTTP/1.1 200 OK\r\n");
                        str.Append("Access-Control-Allow-Origin: *\r\n");
                        str.Append("Connection: close\r\n");
                        str.Append("\r\n");
                        rd.Close();
                        sw.Close();
                        stream.Close();
                        break;

                        Console.WriteLine("HTTP COM: RECEIVED NULL, IGNORING, " + e.ToString());
                    }
                }

                client.Close();
            }
           
            
        }
    }
}
