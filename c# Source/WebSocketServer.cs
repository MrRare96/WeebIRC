using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WeebIRCWebEdition
{
    class WebSocketServer
    {
        private Thread ServerThread;
        private int port;
        private NetworkStream stream = null;
        private TcpListener server = null;
        private TcpClient client = null;

        public string customIp;
        public Action<string> MessageReceivedCallback;
        public bool PongReceived = false;
        public bool shouldDisconnect = false;

        public WebSocketServer(int port)
        {
            this.port = port;
            customIp = "";
            ServerThread = new Thread(new ThreadStart(() => ServerThreadProgram()));
            ServerThread.Start();
        }

        public WebSocketServer(string customIp, int port)
        {
            this.port = port;
            this.customIp = customIp;
            ServerThread = new Thread(new ThreadStart(() => ServerThreadProgram()));
            ServerThread.Start();
        }

        public void StopServer()
        {
            try
            {
                if (server != null)
                {
                    server.Stop();
                }
                if (client != null)
                {
                    client.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                ServerThread.Abort();
            } catch (Exception e)
            {
                Console.WriteLine("FAILED TO STOP WS SERVER");
            }
           
        }

        public string GetLocalIPAddress()
        {
            if (customIp == "")
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
            else
            {
                return customIp;
            }


        }

        public void SetMessageReceivedCallback(Action<string> callback)
        {
            MessageReceivedCallback = callback;
        }

        public void SendMessage(string input)
        {
            byte[] msg = EncodeMessage(input);
            if(stream != null )
            {
                stream.Write(msg, 0, msg.Length);
                stream.Flush();
            }
        }

        private byte[] EncodeMessage(string input)
        {
            byte[] array = Encoding.UTF8.GetBytes(input.Replace("\0", string.Empty));
            byte[] bytes;


            if (array.Length > 125)
            {
                bytes = new byte[array.Length + 4];
                bytes[0] = 0x81; //text message
                byte b0 = (byte)array.Length, b1 = (byte)(array.Length >> 8);
                bytes[1] = 0x7E; //size, 126
                bytes[2] = b1;
                bytes[3] = b0;
                for (int i = 4; i < array.Length; i++)
                {
                    bytes[i] = array[i-4];
                }
            } else
            {
                bytes = new byte[127];
                bytes[0] = 0x81; //text message
                bytes[1] = 0x7D; //size, 125
                Buffer.BlockCopy(array, 0, bytes, 2, array.Length);
            }

            return bytes;
        }
        

        private string DecodeMessage(byte[] message)
        {
            if(message.Length > 1)
            {
                int msglength = Convert.ToInt32(message[1] & 127);
                int firstMaskIndex = 2;
                if (msglength == 126)
                {
                    firstMaskIndex = 4;
                }
                else if (msglength == 127)
                {
                    firstMaskIndex = 10;
                }

                byte[] mask = new byte[4];

                for(int i = 0; i < mask.Length; i++)
                {
                    mask[i] = message[i + firstMaskIndex];
                }

                int indexFirstDataByte = firstMaskIndex + 4;

                byte[] decoded = new byte[128];
               
                int ic = indexFirstDataByte;
                int j = 0;
                while(message[ic] != 0)
                {
                    int maskInt = j % 4;
                    try
                    {
                        decoded[ic - indexFirstDataByte] = (Byte)(message[ic] ^ mask[maskInt]);
                    }
                    catch
                    {
                        Console.WriteLine("index out of range");
                        break;
                    }
                    
                    ic++;
                    j++;
                }
                return Encoding.UTF8.GetString(decoded);
            } else
            {
                return "No message";
            }
        }

        private void pingPong()
        {
            while (!shouldDisconnect)
            {

                SendMessage("ping~ping~ping");
                PongReceived = false;
                Thread.Sleep(5000);
                int timeoutcounter = 0;
                while (!PongReceived)
                {
                    if(timeoutcounter > 10)
                    {
                        shouldDisconnect = true;
                        break;
                    }
                    timeoutcounter++;
                    Thread.Sleep(1000);
                }
            }
        }
        private void ServerThreadProgram()
        {
            try
            {

                server = new TcpListener(IPAddress.Any, port);

                server.Start();
                while (true)
                {


                    try
                    {
                        MessageReceivedCallback("CONNECTING");
                    }
                    catch
                    {
                        //this is bad but im tired
                    }

                    Console.WriteLine("WS: WS Server has started on " + IPAddress.Any.ToString() + ":" + port.ToString() + ".{0}Waiting for a connection...", Environment.NewLine);

                    client = server.AcceptTcpClient();


                    Console.WriteLine("WS: A client connected.");
                    stream = client.GetStream();

                    Thread.Sleep(1);
                    Thread pinger = new Thread(new ThreadStart(() => pingPong()));
                    
                    while (client.Connected && !shouldDisconnect)
                    {
                        Thread.Sleep(1);
                        if (stream.CanRead)
                        {
                            byte[] myReadBuffer = new byte[1024];
                            StringBuilder myCompleteMessage = new StringBuilder();
                            int numberOfBytesRead = 0;

                            // Incoming message may be larger than the buffer size.
                            do
                            {
                                numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                                myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));

                            }
                            while (stream.DataAvailable);

                            string receive = myCompleteMessage.ToString();
                            if (receive.Contains("Sec-WebSocket-Key"))
                            {

                                Console.WriteLine("WS: Parsing key");
                                Regex regex1 = new Regex(@"(?<=Sec-WebSocket-Key: )(.*\n?)");
                                Match matches1 = regex1.Match(receive);

                                if (matches1.Success)
                                {
                                    string key = matches1.Value.Trim();
                                    Console.WriteLine("WS: KEY: " + key);
                                    Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                                                                                + "Upgrade: websocket" + Environment.NewLine
                                                                                + "Connection: Upgrade" + Environment.NewLine
                                                                                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                                                                                    SHA1.Create().ComputeHash(
                                                                                        Encoding.UTF8.GetBytes(
                                                                                            key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                                                                        )
                                                                                    )
                                                                                ) + Environment.NewLine
                                                                                + Environment.NewLine);
                                    stream.Write(response, 0, response.Length);

                                    byte[] msg = EncodeMessage("HELLOOOO :3");
                                    try
                                    {

                                        stream.Write(msg, 0, msg.Length);
                                        stream.Flush();
                                        pinger.Start();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {

                                string msgrc = DecodeMessage(myReadBuffer).Trim().Replace("\0", string.Empty);
                                if (msgrc == "disconnect")
                                {
                                    stream.Close();
                                    client.Close();
                                    pinger.Abort();
                                    break;
                                }
                                else if (msgrc != "")
                                {

                                    Console.WriteLine("WS: RAW WS MSG: " + msgrc);
                                    if (msgrc.Contains("pong"))
                                    {
                                        PongReceived = true;
                                    } else {
                                        if (msgrc.Contains(":msgisok"))
                                        {
                                            MessageReceivedCallback(msgrc.Replace(":msgisok", ""));
                                        }
                                        else
                                        {
                                            SendMessage("received:" + msgrc);
                                        }
                                    }                                    
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("WS: Sorry.  You cannot read from this NetworkStream.");
                        }
                    }

                    try
                    {
                        stream.Close();
                        client.Close();
                        pinger.Abort();
                    } catch(Exception e)
                    {
                        Console.WriteLine("WS: COULD NOT CLOSE WS SERVER, PROBABLY ALREADY CLOSED");
                    }
                }
            } catch (SocketException se)
            {
                Console.WriteLine("WS: Socket closed: " + se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("WS: Other errors :X: " + e.ToString());
            }


        }
    }
}
