using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace CMDIRC
{
    class Setup
    {
        static void Main(string[] args)
        {
            //setup vars
            string ip;
            int port;
            string username;
            string password;
            string channel;

            //setup screen:
            Console.WriteLine("Server IP(default is : 54.229.0.87(irc.rizon.net)) = ");
            if ((ip = Console.ReadLine()) == "")
            {
                ip = "54.229.0.87";
            }

            Console.WriteLine("Server Port(default is : 6667) = ");
            if (Console.ReadLine() != "")
            {
                port = Convert.ToInt32(Console.ReadLine());
            } 
            else
            {
                port = 6667; 
            }

            Console.WriteLine("Username(default is : RareIRC_Client) = ");
            if ((username = Console.ReadLine()) == "")
            {
                username = "RareIRC_Client";
            }

            Console.WriteLine("Password(not working yet, default is : ) = ");
            if ((password = Console.ReadLine()) == "")
            {
                password = "";
            }

            Console.WriteLine("Channel(default is : #RareIRC) = ");
            if ((channel = Console.ReadLine()) == "")
            {
                channel = "#RareIRC";
            }

            //irc client executing:
            IrcClient irc = new IrcClient(ip, port, username, password, channel);
            irc.IrcClientRun();

            Console.ReadLine();
        }
    }
}
