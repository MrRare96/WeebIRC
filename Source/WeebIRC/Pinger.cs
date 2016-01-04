using System;
using System.Threading;


namespace WeebIRC
{
    class Pinger
    {
        //vars needed to ping pong with the server
        private string ping = "PING :";
        private static Thread pingSender;
        private Form1 form;
        //creates a thread for the pinger
        public Pinger(Form1 formIn)
        {
            pingSender = new Thread(new ThreadStart(this.Run));
            form = formIn;
        }
        //starts the ping thread
        public void Start()
        {
            pingSender.Start();
        }

        public static void Stop()
        {
            try
            {
                pingSender.Abort();
            } catch (Exception e)
            {
                //do nothing
            }
        }
        //function that runs in the ping thread, used to keep the connection with the irc server alive
        public void Run()
        {
            while (!form.StopProgram)
            {
                IrcConnect.writeIrc(ping + IrcConnect.newIP); 
                Thread.Sleep(15000);
            }            
        }
    }
}
