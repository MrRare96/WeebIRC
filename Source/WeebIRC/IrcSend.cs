using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WeebIRC
{
    class IrcSend
    {

        //program function
        public static string packNumber;
        public static string bot;
        //this is where different kind of messages can be send to the server
        public static void sendMsg(string Input, Form1 formIn)
        {
            if (Input.ToLower().Contains("xdcc send"))
            {
                string[] xdccparts = Input.Split(' ');
                formIn.AppendDebug("xdcc string received: " + Input);
                int bCounter = 0;
                while(xdccparts[bCounter].Contains("xdcc") || xdccparts[bCounter].Contains("send") || xdccparts[bCounter].Contains("/msg") || xdccparts[bCounter].Contains("#"))
                {
                    bCounter++;
                    if(bCounter == 10)
                    {
                        formIn.AppendDebug("no bot found");
                        break;
                    }
                }

                int pCounter = 0;
                while (!xdccparts[pCounter].Contains("#"))
                {
                    pCounter++;
                    if (pCounter == 10)
                    {
                        formIn.AppendDebug("no pack num found");
                        break;
                    }
                }
                bot = xdccparts[bCounter];
                packNumber = xdccparts[pCounter];

                string xdccdl = "PRIVMSG " + bot + " :XDCC SEND " + packNumber;
                formIn.AppendIrcOutput("RareIRC: XDDC request : " + xdccdl);
                IrcConnect.writeIrc(xdccdl);
            }
            else if (Input.ToLower().Contains("xdcc cancel"))
            {
                string[] xdccparts = Input.Split(' ');
                string xdcccl = "PRIVMSG " + xdccparts[1] + " :XDCC CANCEL";
                formIn.AppendIrcOutput("RareIRC: XDDC CANCEL : " + xdcccl);
                IrcConnect.writeIrc(xdcccl);
            }
            else if (Input.ToLower().Contains("xdcc remove"))
            {
                string[] xdccparts = Input.Split(' ');
                string xdccdl = "PRIVMSG " + xdccparts[1] + " :XDCC REMOVE " + xdccparts[4];
                formIn.AppendIrcOutput("RareIRC: XDDC remove : " + xdccdl);
                IrcConnect.writeIrc(xdccdl);
            }
            else if (Input.Contains("/quit"))
            {
                IrcConnect.writeIrc("QUIT");
            }
            else
            {
                formIn.AppendIrcOutput(IrcConnect.newUsername + " : " + Input);
                IrcConnect.writeIrc("PRIVMSG " + IrcConnect.newChannel + " :" + Input);
            }
            
        }
        //actually sends it to the server with a function defined in IrcConnect
        public static void sendInput(string Input)
        {
            IrcConnect.writeIrc(Input);
        }
       
    }
}
