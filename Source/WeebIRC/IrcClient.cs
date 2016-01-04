using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace WeebIRC
{
    class IrcClient
    {


        //Member vars

        private Form1 form;
        private string newIP;
        private int newPort;
        private string newUsername;
        private string newPassword;
        private string newChannel;
        public bool conCheck;


        //default construct
        public IrcClient()
        {
            newIP = "";
            newPort = 0;
            newUsername = "";
            newPassword = "";
            newChannel = "";
        }

        //overload construct
        public IrcClient(string IP, int Port, string Username, string Password, string Channel, Form1 formIn)
        {
            newIP = IP;
            newPort = Port;
            newUsername = Username;
            newPassword = Password;
            newChannel = Channel;
            form = formIn;
        }

        //Accessor Functions
        public string getIP()
        {
            return newIP;
        }

        public int getPort()
        {
            return newPort;
        }

        public string getUsername()
        {
            return newUsername;
        }

        public string getPassword()
        {
            return newPassword;
        }

        public string getChannel()
        {
            return newChannel;
        }

        //Mutator Functions
        public void setIP(string IP)
        {
            newIP = IP;
        }

        public void setPort(int Port)
        {
            newPort = Port;
        }

        public void setUsername(string Username)
        {
            newUsername = Username;
        }

        public void setPassword(string Password)
        {
            newPassword = Password;
        }

        public void setChannel(string Channel)
        {
            newChannel = Channel;
        }

        //program function

        //its the heart, here it outputs the server responses, here you can type to the server, here is where it responses to the server etc
        public void IrcClientRun()
        {
            IrcConnect con = new IrcConnect(newIP, newPort, newUsername, newPassword, newChannel, form);
            conCheck = con.Connect();
            form.AppendIrcOutput("IP: " + con.getIP());
            form.AppendIrcOutput("Port: " + con.getPort());
            form.AppendIrcOutput("Username: " + con.getUsername());

            if (conCheck)
            {
                form.AppendIrcOutput("succesful connected to the irc server!");
                string ircData = "";
                var worker = new Thread(() =>
                {
                    while ((ircData = IrcConnect.reader.ReadLine()) != null && !form.StopProgram)
                    {
                        if (ircData.Contains("PONG"))
                        {

                        }
                        else
                        {
                            if (ircData.ToLower().Contains("privmsg " + IrcConnect.newChannel.ToLower() + " :"))
                            {
                                string[] messageParts = ircData.Split(':');
                                string[] nicknameParts = messageParts[1].Split('!');
                                form.AppendIrcOutput(nicknameParts[0] + " : " + messageParts[messageParts.Length - 1]);

                            } else
                            {
                                form.AppendIrcOutput(ircData);
                            }
                        }

                        if (ircData.Contains("DCC SEND") && ircData.Contains(newUsername))
                        {
                            DCCClient dcc = new DCCClient(ircData, form);
                            dcc.getDccParts();
                            dcc.setDccValues();

                            form.AppendIrcOutput("FILENAME: " + dcc.getFileName());
                            form.AppendIrcOutput("FILESIZE: " + dcc.getFileSize());
                            form.AppendIrcOutput("IP: " + dcc.getIp());
                            form.AppendIrcOutput("PORT: " + dcc.getPortNum());

                            dcc.Downloader();
                        }
                    }
                });

                worker.Start();
                
            }
            else
            {
                form.AppendIrcOutput("Something went wrong, try again!");
            }
        }

    }
}
