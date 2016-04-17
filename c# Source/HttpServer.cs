using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace WeebIRCWebEdition
{
    class HttpServer
    {

        private Thread ComServerThread;
        private Thread ServerThread;
        private Thread FileStreamServerThread;
        private Thread FileDownloadServerThread;
        private TcpListener streamTcpServer;
        private Thread uploader;

        public int port { get; set; }
        public int streamPort { get; set; }
        public int dlPort { get; set; }
        public int comPort { get; set; }

        public string clientIp;
        public string jsonDataToSend = "\"NOMESSAGES\",";
        public string rawJsonToSend = "[\"NOMESSAGES\"]";

        public Action<string> MessageReceivedCallback;
        public Action<string> DebugMessageCallback;

        private bool disconnect = false;
        

        /// <summary>
        ///   HttpServer Constructor, sets up all 4 http servers : Web, File Download, File Stream and Communication
        /// </summary>
        public HttpServer(int port, Action<string> MessageReceivedCallback, Action<string> DebugMessageCallback)
        {
            disconnect = false;
            this.MessageReceivedCallback = MessageReceivedCallback;
            this.DebugMessageCallback = DebugMessageCallback;
            this.port = port;
            dlPort = port + 10;
            comPort = dlPort + 10;
            streamPort = comPort + 10;  

            ComServerThread = new Thread(new ThreadStart(() => ComServerThreadProgram()));
            ComServerThread.Start();
            ServerThread = new Thread(new ThreadStart(() => ServerThreadProgram()));
            ServerThread.Start();
            FileStreamServerThread  = new Thread(new ThreadStart(() => FileStreamServerThreadProgram()));
            FileStreamServerThread.Start();
            FileDownloadServerThread = new Thread(new ThreadStart(() => FileDownloadServerThreadProgram()));
            FileDownloadServerThread.Start();
        }

        /// <GetLocalIPAddress>
        /// <summary>
        ///  Gets IP from all network connection available on pc,and compares it to the base IP of the client, 
        ///  this is necesary to figure out to which endpoint the interface has to connect, 
        ///  code is edited version of: http://stackoverflow.com/questions/6803073/get-local-ip-address
        ///  </summary>
        /// </GetLocalIPAddress>
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
                        return ip.ToString();
                    }
                } catch
                {
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        /// <SendMessage>
        /// <summary>
        /// Appends to a string buffer which will be send to the web client on request. See CommunicationServer method!
        /// </summary>
        /// </SendMessage>
        public void SendMessage(string msg)
        {
            jsonDataToSend = jsonDataToSend + "\"" + msg + "\",";
        }

        public void JsonToSend(string json)
        {
            rawJsonToSend = json;
        }

        /// <StopServer>
        ///  <summary>
        /// Aborts all running server threads
        ///  </summary>
        /// </StopServer>
        public void StopServer()
        {
            disconnect = true;
            Thread.Sleep(500);
           
        }

        /// <ServerThreadProgram>
        ///  <summary>
        /// TCP Server which acts as a HTTP web server, providing the browser the interface html/js and other visual elements
        ///  </summary>
        /// </ServerThreadProgram>
        private void ServerThreadProgram()
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            DebugMessageCallback("|HTTP WEB SERVER| " + "Started on " + IPAddress.Any.ToString() + ":" + port.ToString());

          while(!disconnect)
            {
                string receive = "";
               
                DebugMessageCallback("|HTTP WEB SERVER| " + "Waiting for connections!");

                TcpClient client = server.AcceptTcpClient();

                clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                DebugMessageCallback("|HTTP WEB SERVER| " + "A client connected from " + clientIp + ".");

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


                            string fileToSend = "";

                            string stufftosend = "";
                      
                            Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                            Match matches1 = regex1.Match(receive);
                            
                            StringBuilder str = new StringBuilder();
                            str.Append("HTTP/1.1 200 OK\r\n");
                            str.Append("Access-Control-Allow-Origin: *\r\n");
                            str.Append("Connection: close\r\n");
                            str.Append("\r\n");


                            if (matches1.Success)
                            {
                                string valueReceived = matches1.Value; 
                                if (!(valueReceived == "/"))
                                {
                                    if (valueReceived.Contains("isthisaserver"))
                                    {
                                        DebugMessageCallback("|HTTP WEB SERVER| " + "Client requested server confirmation!");
                                        str.Append("YUP");
                                        sw.Write(str);
                                        sw.Flush();
                                    }
                                    else if (valueReceived.Contains("?url:"))
                                    {
                                        DebugMessageCallback("|HTTP WEB SERVER| " + "Requested source code of page: '" + valueReceived.Substring(1).Replace("?url:", "") + "'!");
                                        using (WebClient webclient = new WebClient())
                                        {
                                            stufftosend = webclient.DownloadString(valueReceived.Substring(1).Replace("?url:", "").Trim());
                                            str.Append(stufftosend);
                                            sw.Write(str);
                                            sw.Flush();
                                        }
                                    }
                                    else
                                    {
                                        fileToSend = valueReceived.Substring(1);
                                        DebugMessageCallback("|HTTP WEB SERVER| " + "Requested file '" + fileToSend + "'!");
                                    }
                                }
                                else
                                {
                                    fileToSend = @"home.html";
                                    DebugMessageCallback("|HTTP WEB SERVER| " + "Requested interface page: '" + fileToSend + "'!");
                                }
                            }
                            else
                            {
                                fileToSend = @"home.html";
                                DebugMessageCallback("|HTTP WEB SERVER| " + "Requested interface page: '" + fileToSend + "'!");
                            }
                            
                            if(fileToSend != "")
                            {
                                if (File.Exists(@"GUI/" + fileToSend))
                                {
                                    if (IsWebPageFile(fileToSend))
                                    {
                                        DebugMessageCallback("|HTTP WEB SERVER| " + "File is textfile: '" + fileToSend + "'!");
                                        using (StreamReader sr = new StreamReader(@"GUI\" + fileToSend))
                                        {
                                            stufftosend = sr.ReadToEnd();
                                        }

                                        if (stufftosend.Contains("myIP"))
                                        {
                                            string addedIp = Regex.Replace(stufftosend, "myIP", "\"" + GetLocalIPAddress() + "\"");
                                            if (stufftosend.Contains("myPORT"))
                                            {
                                                DebugMessageCallback("|HTTP WEB SERVER| " + "File Requires IP and PORT '" + fileToSend + "'!");
                                                Console.WriteLine("HTTP: putting server ip in js");
                                                string addedIpAndPort = Regex.Replace(addedIp, "myPORT", "\"" + port + "\"");
                                                str.Append(addedIpAndPort);
                                            }
                                            else
                                            {

                                                DebugMessageCallback("|HTTP WEB SERVER| " + "File Requires IP '" + fileToSend + "'!");
                                                str.Append(addedIp);
                                            }
                                        }
                                        else
                                        {

                                            str.Append(stufftosend);
                                        }
                                        sw.Write(str);
                                        sw.Flush();
                                    } else
                                    {
                                        if (IsFontFile(@"GUI/" + fileToSend))
                                        {
                                            DebugMessageCallback("|HTTP WEB SERVER| " + "Font File Requested: '" + @"GUI/" + fileToSend + "'!");
                                            FileInfo f = new FileInfo(@"GUI/" + fileToSend);
                                            string filesize = f.Length.ToString();

                                            StringBuilder str2 = new StringBuilder();
                                            str2.Append("HTTP/1.1 200 OK\r\n");
                                            str2.Append("Content-Type: font/opentype\r\n");
                                            str2.Append("Connection: close\r\n");
                                            str2.Append("Content-Length: " + filesize + "\r\n");
                                            str2.Append("\r\n");
                                            sw.Write(str2);
                                            sw.Flush();

                                            byte[] fontInBytes = File.ReadAllBytes(@"GUI/" + fileToSend);
                                            stream.Write(fontInBytes, 0, fontInBytes.Length);
                                            stream.Flush();
                                        }
                                        else
                                        {
                                            DebugMessageCallback("|HTTP WEB SERVER| " + "Other File Requested: '" + @"GUI/" + fileToSend + "'!");
                                            FileInfo f = new FileInfo(@"GUI/" + fileToSend);
                                            string filesize = f.Length.ToString();

                                            StringBuilder str2 = new StringBuilder();
                                            str2.Append("HTTP/1.1 200 OK\r\n");
                                            str2.Append("Connection: close\r\n");
                                            str2.Append("Content-Length: " + filesize + "\r\n");
                                            str2.Append("\r\n");
                                            sw.Write(str2);
                                            sw.Flush();

                                            byte[] fontInBytes = File.ReadAllBytes(@"GUI/" + fileToSend);
                                            stream.Write(fontInBytes, 0, fontInBytes.Length);
                                            stream.Flush();
                                        }
                                        
                                    }
                                  
                                }
                                else
                                {
                                    DebugMessageCallback("|HTTP WEB SERVER| " + "FILE NOT FOUND: '" + fileToSend + "'!");
                                    StringBuilder str2 = new StringBuilder();
                                    str2.Append("HTTP/1.1 404 Not Found\r\n");
                                    sw.Write(str2);
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
                    rd.Close();
                    sw.Close();
                    stream.Close();
                    client.Close();
                    DebugMessageCallback("|HTTP WEB SERVER| |ERROR| " + "Connection aborted with client, request is null : error: '" + e.ToString() + "'!");

                }
                Thread.Sleep(1);
            }
            server.Stop();
        }


        /// <FileServerThreadProgram>
        /// <summary>
        /// TCP Server which acts as a HTTP file stream server, used for streaming file to a html5 video tag for example
        /// </summary>
        /// </FileServerThreadProgram>
        private void FileStreamServerThreadProgram()
        {
            streamTcpServer = new TcpListener(IPAddress.Any, dlPort);
            streamTcpServer.Start();
            Console.WriteLine("STEP 1");
          while(!disconnect)
            {
                Thread.Sleep(1);
                DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "Started on " + IPAddress.Any.ToString() + ":" + dlPort.ToString());
                TcpClient client = streamTcpServer.AcceptTcpClient();
                Console.WriteLine("STEP 2");
                string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "A client connected from " + clientIP + ".");
                NetworkStream stream = client.GetStream();

                StreamReader rd = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                string received = "";
                string file = "";
                long filesize = 0;
                int startRead = 0;
                int stopRead = -1;
              while(!disconnect)
                {
                    
                    try
                    {
                        stream = client.GetStream();
                        received = rd.ReadLine();
                    } catch
                    {
                        break;
                    }
                    
                    Thread.Sleep(1);
                    try
                    {
                        Regex.Replace(received, @"\s+", "");
                        if (received != "")
                        {
                            try
                            {
                                received = received.Replace("%20", " ");
                            }
                            catch
                            {

                            }
                            if (received.Contains("GET"))
                            {
                                try
                                {
                                    Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                                    Match matches1 = regex1.Match(received);
                                    string valueReceived = matches1.Value;
                                    file = valueReceived.Trim().Substring(1).Split('?')[0];
                                    filesize = Int64.Parse(valueReceived.Trim().Substring(1).Split('?')[1]);

                                    DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "Client requested file stream: " + file + ", filesize: " + filesize);
                                } catch {
                                    Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                                    Match matches1 = regex1.Match(received);
                                    string valueReceived = matches1.Value;
                                    file = valueReceived.Trim().Substring(1).Split('?')[0];
                                    filesize = 0;

                                    DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "Client requested file download: " + file + ", filesize: " + filesize);
                                }
                                
                            } 

                            if (received.Contains("Range:"))
                            {
                                string range = received.Split('=')[1];
                                string[] parts = range.Split('-');
                                string start = parts[0];
                                string end = "";
                                try
                                {
                                    try
                                    {

                                        end = parts[1].Split('/')[0];
                                    }
                                    catch
                                    {
                                        end = parts[1];
                                    }
                                    stopRead = Int32.Parse(end);
                                }
                                catch
                                {
                                    end = "";
                                }
                                startRead = Int32.Parse(start);

                                DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "Client requested range: " + startRead + "-> " + stopRead);

                                if (file != "")
                                {

                                    string filepath = @"GUI/" + file;
                                    if (File.Exists(filepath))
                                    {
                                        DebugMessageCallback("|HTTP FILE STREAM SERVER| " + "FILE LOCAL LOCATION: " + filepath);

                                        if (filesize == 0)
                                        {

                                            FileInfo f = new FileInfo(filepath);
                                            filesize = f.Length;
                                        }


                                        uploader = new Thread(new ThreadStart(() => SendFile(filepath, filesize, stream, startRead)));
                                        uploader.Start();

                                    }
                                    else
                                    {
                                        DebugMessageCallback("|HTTP FILE SERVER|" + "OUT OF RANGE: " + startRead + "-> " + stopRead);
                                        break;
                                    }
                                }
                            } else
                            {
                                DebugMessageCallback("|HTTP FILE SERVER|" + "CLIENT DID NOT REQUEST STREAM ");
                                startRead = 0;
                            }

                           
                        } 
                        received = "";
                       
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            DebugMessageCallback("|HTTP FILE SERVER|" + "SOMETHING ELSE HAPPEND WHICH I AM NOT AWARE OF, CLOSING UPLOADER:  " + e.ToString());
                            uploader.Abort();
                        }
                        catch
                        {
                            DebugMessageCallback("|HTTP FILE SERVER|" + "SOMETHING ELSE HAPPEND WHICH I AM NOT AWARE OF, UPLOADER CLOSED OR NOT STARTED:  " + e.ToString());
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            streamTcpServer.Stop();
        }

        /// <SendFile>
        /// <summary>
        /// Code for streaming a file to the client, assuming client connection is made
        /// </summary>
        /// </SendFile>
        private void SendFile(string filepath, long currentfilesize, NetworkStream stream, int startRead)
        {
            StreamWriter sw = new StreamWriter(stream);
            string HttpDate = DateTime.Now.ToUniversalTime().ToString("r");
            StringBuilder str = new StringBuilder();
            if(startRead == 0)
            {
                str.Append("HTTP/1.1 200 OK\r\n");
                str.Append("Date: " + HttpDate + "\r\n");
                str.Append("Connection: keep-alive\r\n");
            } else
            {
                str.Append("HTTP/1.1 206 Partial Content\r\n");
                str.Append("Date: " + HttpDate + "\r\n");
                str.Append("Connection: keep-alive\r\n");
            }

            str.Append("Content-Type: application/octet-stream\r\n");
            str.Append("Content-Length: " + (currentfilesize - startRead).ToString()  + "\r\n");
            str.Append("Accept-Ranges: bytes\r\n");
            str.Append("Content-Range: bytes " + startRead.ToString() + "-" + (currentfilesize - 1).ToString() + "/*\r\n");
            str.Append("\r\n");
            sw.Write(str);
            sw.Flush();
            using (FileStream source = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[131072];
                int bytesRead;
                long bytesTotalRead = 0;
                source.Position = startRead;

                Thread.Sleep(1);

                try
                {

                  while(!disconnect && bytesTotalRead != currentfilesize)
                    {
                        Thread.Sleep(1);
                        bytesRead = source.Read(buffer, 0, buffer.Length);
                        bytesTotalRead += bytesRead;
                        try
                        {
                                if (stream.CanWrite)
                                {
                                    stream.Write(buffer, 0, bytesRead);
                                }
                                else
                                {
                                    DebugMessageCallback("|HTTP FILE STREAM SERVER|" + "Can't write to stream");
                                    TcpClient client = streamTcpServer.AcceptTcpClient();
                                    stream = client.GetStream();
                                    sw = new StreamWriter(stream);
                                    Console.WriteLine("STEP 5");
                            }
                        } catch(Exception e)
                        {
                            DebugMessageCallback("|HTTP FILE STREAM SERVER|" + "Can't write to stream, error: " + e.ToString());
                            break;
                        }
                                      

                    }
                    
                }

                catch (Exception ex)
                {
                    DebugMessageCallback("|HTTP FILE STREAM SERVER| |ERROR| " + "Client probably aborted download: '" + ex.ToString() + "'!");
                }
                finally
                {
                    stream.Close();
                }
                
                DebugMessageCallback("|HTTP FILE STREAM SERVER|" + "DONE UPLOADING FILE: " + filepath);
            }
        }


        /// <FileDownloadServerThreadProgram>
        /// <summary>
        /// Tcp server for File Downloading (NOT STREAMING), servers file sequential/non seekable to the client
        /// </summary>
        /// </FileDownloadServerThreadProgram>
        private void FileDownloadServerThreadProgram()
        {

            TcpListener server = new TcpListener(IPAddress.Any, streamPort);

            server.Start();
          while(!disconnect)
            {
                Thread.Sleep(1);
                DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|" + "Server has started on " + IPAddress.Any.ToString() + ":" + dlPort.ToString() + ". Waiting for a connection...");
                TcpClient client = server.AcceptTcpClient();


                DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|" + "CLIENT CONNECTED FOR DOWNLOAD");

                NetworkStream stream = client.GetStream();

                StreamReader rd = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                string received = "";
                string file = "";
                string filesize = "0";

                try
                {
                  while(!disconnect && (received = rd.ReadLine()).Length != 0)
                    {

                        received = received.Replace("%20", " ");
                        if (received.Contains("GET"))
                        {
                            Regex regex1 = new Regex(@"(?<=GET )(.*\n?)(?= HTTP)");
                            Match matches1 = regex1.Match(received);
                            file = matches1.Value.Trim().Substring(1).Split('?')[0];
                            filesize = matches1.Value.Trim().Substring(1).Split('?')[1];

                            DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|  File requested: " + file );
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|  RECEIVED NULL, IGNORING");
                }

                if (file != "")
                {

                    string filepath = @"GUI/" + file;
                    if (File.Exists(filepath))
                    {
                        Console.WriteLine("\n HTTP FS: File location: " + filepath + "\n");
                        DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER| File location: " + filepath);
                        if (filesize == "0")
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
                        
                        using (FileStream source = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            byte[] buffer = new byte[131072];
                            int bytesRead;
                            long bytesTotalRead = 0;
                          while(!disconnect && bytesTotalRead.ToString() != filesize)
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
                                        DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER| Client aborted download ");
                                        break;
                                    }
                                }
                                else
                                {
                                    
                                    DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|  Cant write to stream: " + filepath);
                                    break;
                                }
                            }
                            DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER| DONE UPLOADING FILE: " + filepath);
                            DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER| BYTES UPLOADED: " + bytesTotalRead.ToString() + " OF " + filesize);
                        }
                        rd.Close();
                        sw.Close();
                        stream.Close();
                        client.Close();


                    }
                    else
                    {
                        rd.Close();
                        sw.Close();
                        stream.Close();
                        client.Close();
                        DebugMessageCallback("|HTTP FILE DOWNLOAD SERVER|COULD NOT FIND FILE: " + filepath);
                    }
                }
            }
            server.Stop();
        }

        /// <ComServerThreadProgram>
        /// <summary>
        /// Runs communication between client and server, for example: 
        /// client does a ajax call to this server, asking for information, the server response is the jsonDataToSend buffer.
        /// after sending, it resets the buffer
        /// Advantages: Server is not dependend on a reliable connection with client and vice versa, making server not hang on disconnect for example.
        /// Disadvantages: HTTP overhead is huge, though using an existing TCP connection instead, its not bad.
        /// </summary>
        /// </ComServerThreadProgram>
        private void ComServerThreadProgram()
        {

            TcpListener server = new TcpListener(IPAddress.Any, comPort);
            DebugMessageCallback("|HTTP COMMUNICATION SERVER| " + "Started on " + IPAddress.Any.ToString() + ":" + comPort.ToString());
            server.Start();

          while(!disconnect)
            {
                TcpClient client = server.AcceptTcpClient();
                string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                DebugMessageCallback("|HTTP COMMUNICATION SERVER| " + "A client connected from " + clientIP + ".");
               

              while(!disconnect && client.Connected)
                {

                    NetworkStream stream = client.GetStream();
                    StreamReader rd = new StreamReader(stream);
                    StreamWriter sw = new StreamWriter(stream);

                    string received = "";
                    try
                    {
                        byte[] myReadBuffer = new byte[1024];
                        int numberOfBytesRead;

                      while(!disconnect && (numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length)) != 0 && !disconnect)
                        {
                            received = Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead);
                            if (received.Contains("message="))
                            {
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
                            str.Append("{\"messages\" : [" + jsonDataToSend.Substring(0, jsonDataToSend.Length - 1) + "], \"rawjson\" : " + rawJsonToSend + "}");
                            sw.Write(str);
                            sw.Flush();
                            if (jsonDataToSend != "\"NOMESSAGES\",")
                            {
                                jsonDataToSend = "\"NOMESSAGES\",";
                            }

                            if(rawJsonToSend != "[\"NOMESSAGES\"]")
                            {
                                rawJsonToSend = "[\"NOMESSAGES\"]";
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

                        DebugMessageCallback("|HTTP COMMUNICATION SERVER| |ERROR| " + "Ignoring request, request is null : error: '" + e.ToString() + "'!");

                        break;

                    }
                }

                client.Close();
            }

            server.Stop();
        }

        /// <IsFontFile>
        /// <summary>
        /// Checks if filename contains extension for a textfile
        /// </summary>
        /// </IsFontFile>
        private bool IsWebPageFile(string filename)
        {
            string[] fileExtensions = new string[] { ".htm", ".html", ".css", ".svg", ".js", ".ass", ".vtt"};
            string extension = Path.GetExtension(filename);

            int inArray = Array.IndexOf(fileExtensions, extension);
            if(inArray > -1)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <IsFontFile>
        /// <summary>
        /// Checks if filename contains extension for a font
        /// </summary>
        /// </IsFontFile>
        private bool IsFontFile(string filename)
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
    }
}
