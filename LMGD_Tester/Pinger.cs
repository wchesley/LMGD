using System;
using System.Net.NetworkInformation;
using System.Text;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace LMGD_Tester
{
    class Pinger
    {
        /// <summary>
        /// Test to see if equipment is responding via ping. To get specific ATA, must specify what type it is.
        /// Example call for ATA is PingBuilder(IP, "ATA Cambium");
        /// If pinging radio, only need to specify "radio" as the equipType Parameter. 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="equipType"></param>
        /// <returns></returns>
        public string PingBuilder(ChromeDriver browser, string equipType, string IP)
        {
            ATA GetATA = new ATA();
            Radio GetRadio = new Radio();
            string PingReplies = string.Empty;
            Ping Pinger = new Ping();
            PingOptions opt = new PingOptions();
            opt.DontFragment = true;
            int successPacket = 0;
            int failedPacket = 0; 
            //create packet as per https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=netframework-4.7.2
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 10000;
            Console.WriteLine($"Passed to RegEx: {IP}");
           
            //Tested this regex string in calculator and appears to work on IP's with stuffs before and after actuall address.
            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            Match result = ip.Match(IP);
            Console.WriteLine($"{equipType} IP: {result}");
                try
                {
                for (int counter = 1; counter <= 10; counter++)
                {
                    PingReply reply = Pinger.Send(result.Value, timeout, buffer, opt);
                    if (reply.Status == IPStatus.Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}ms");
                        successPacket++;
                    }
                    if (reply.Status == IPStatus.Success && counter == 10)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}ms");
                        PingReplies += $"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}ms \n";
                        Console.ResetColor();
                        successPacket++;
                        switch (equipType)
                        {
                            case "ATA Cambium":
                                Console.WriteLine("Launching Cambium logic...");
                                PingReplies += GetATA.Cambium(browser);
                                break;
                            case "ATA SPA122":
                                Console.WriteLine("Launching SPA122 logic...");
                                PingReplies += GetATA.Spa122(browser);
                                break;
                            case "ATA SPA2102":
                                Console.WriteLine("Launching SPA2102 logic...");
                                PingReplies += GetATA.Spa2102(browser);
                                break;
                            case "radio":
                                Console.WriteLine("Launching Radio logic...");
                                PingReplies += GetRadio.GetRadioType(browser);
                                break;
                            default:
                                PingReplies += "Unable to determine Equipment type";
                                break;
                        }
                    }
                    //if ping fails then RoundtripTime will be 0
                    else if (reply.RoundtripTime == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to receive reply from {result.Value}");
                        Console.ResetColor();
                        //return PingReplies += $"Failed to receive reply from {result.Value}";

                        failedPacket++;
                    }
                }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ping Error: {e.ToString()}");
                    return PingReplies;
                }
            Console.WriteLine($"Received: {successPacket.ToString()}\nFailed Responses: {failedPacket.ToString()}");
            return PingReplies; 
        }
    }
}