using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace LMGD_Tester
{
    class Ping
    {
        public string PingBuilder(string IP, string equipType)
        {
            string PingReplies;
            Ping Pinger = new Ping();
            PingOptions opt = new PingOptions();
            opt.DontFragment = true; 
            int successPacket = 0;
            int failedPacket = 0; 
            //create packet as per https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=netframework-4.7.2
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 250; 
            //Considering option to just launch CLI and endlessly ping desired/found IP's, 
            //this would need to be closed with each new run 
            for(int i=0; i<=10; i++)
            {
                PingReply reply = Pinger.
                if(reply == IPStatus.Success)
                {
                    Console.WriteLine($"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}");
                    PingReplies += $"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}";
                    successPacket++;
                }
                //if ping fales then RoundtripTime will be 0
                else if (reply.RoundtripTime = 0)
                {
                    Console.WriteLine($"Failed to receive reply from {IP}");
                    PingReplies += $"Failed to receive reply from {IP}"; 
                    failedPacket++;
                }
            } 
            return PingReplies(); 
        }
    }
}