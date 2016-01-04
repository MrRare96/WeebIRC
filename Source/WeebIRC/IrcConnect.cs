using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace WeebIRC
{
    class IrcConnect
    {

        //Member Variables

        private int newPort;
        private string newPassword;
        private static Form1 form;


        //Accessable stuff
        public static StreamReader reader;
        public static StreamWriter writer;
        public static NetworkStream stream;
        public static TcpClient irc;

        public static string newIP;
        public static string newChannel;
        public static string newUsername;

        //Default Constructor - null state
        public IrcConnect()
        {
            newIP = "";
            newPort = 0;
            newUsername = "";
            newPassword = "";
            newChannel = "";
        }

        //Overload Constructor - safe way to get variables
        public IrcConnect(string IP, int Port, string Username, string Password, string Channel, Form1 formIn)
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


        //main program for class

        //connects to irc server, gives a boolean back on succesfull connect etc
        public bool Connect()
        {
            try
            {
                irc = new TcpClient(newIP, newPort);
                stream = irc.GetStream();

                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                Pinger ping = new Pinger(form);
                ping.Start();

                writeIrc("USER " + newUsername + " 8 * : Testing RareAMVS C# irc client");
                writeIrc("NICK " + newUsername);
                writeIrc("JOIN " + newChannel);

                return true;
            }
            catch
            {
                return false;
            }
        }

        //function to write to the irc server, bit easier to use and better looking
        public static void writeIrc(string input)
        {
            try
            {
                writer.WriteLine(input);
                writer.Flush();
            } catch (Exception Ee)
            {
            }
            
        }

     
        
        
    }
}
