using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using OpenQA.Selenium.Chrome;

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
        public string PingBuilder(ChromeDriver browser, string equipType)
        {
            ATA GetATA = new ATA();
            Radio GetRadio = new Radio();
            string PingReplies = string.Empty;
            Ping Pinger = new Ping();
            PingOptions opt = new PingOptions();
            opt.DontFragment = true;
            //int successPacket = 0;
            //int failedPacket = 0; 
            //create packet as per https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=netframework-4.7.2
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 10000;
            //Considering option to just launch CLI and endlessly ping desired/found IP's, 
            //this would need to be closed with each new run 

            //ERROR Thrown: Unknown host, think it has something to do with http://ipaddress/ instead of a normal .com address or blank IP, psure windows cmd throws same error
            //try
            //{
            //    PingReply reply = Pinger.Send(browser.Url, timeout, buffer, opt);
            //    if (reply.Status == IPStatus.Success)
            //    {
            //        Console.WriteLine($"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}");
            //        PingReplies += $"Ping to {reply.Address.ToString()} received in: {reply.RoundtripTime}";
            //        // successPacket++;
            //    }
            //    //if ping fails then RoundtripTime will be 0
            //    else if (reply.RoundtripTime == 0)
            //    {
            //        Console.WriteLine($"Failed to receive reply from {browser.Url}");
            //        return PingReplies += $"Failed to receive reply from {browser.Url}";
            //        //failedPacket++;
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"Ping Error: {e.ToString()}");
            //    throw;
            //}
            
                
                
            
            
                if(equipType == "radio")
                {
                    //Call method to Determine Radio type, xfers control to Radio class.
                    GetRadio.GetRadioType(browser);
                }
                //Calls ATA method based on equipType input. xfers control to ATA class. all scraped info will be added to ping replies and return to Main();
                else if(equipType.Contains("ATA"))
                {
                    switch (equipType)
                    {
                        case "ATA Cambium":
                            PingReplies += GetATA.Cambium(browser);
                            break;
                        case "ATA SPA122":
                            PingReplies += GetATA.Spa122(browser);
                            break;
                        case "ATA SPA2102":
                            PingReplies += GetATA.Spa2102(browser);
                            break;
                        default:
                            PingReplies += "Unable to determine ATA type";
                            break;
                    }
                }
            
            return PingReplies; 
        }
    }
}