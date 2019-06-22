using System;
using System.Diagnostics; 
//Testing app for new features/Ideas/Groundwork for LMGD app

namespace LMGD_Tester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //declare variables. 
            string FOPS = "https://fops.amatechtel.com";
            string AtaProvisioning = "/tools/ataprovisioning/";
            string AccountNumber = "";
            FOPS FOPSPage = new FOPS();
            Stopwatch timer = new Stopwatch();
            Console.ForegroundColor = ConsoleColor.Green;
            //init headless chrome browser
            var browser = new BrowserExt().CreateHeadlessBrowser(FOPS);

            //init GUI chrome browser, mostly just for testings sake. 
            //var browser = new BrowserExt().CreateBrowser(FOPS);

            FOPSPage.FOPS_Login(browser, FOPS + AtaProvisioning);

            
            //Get customer account number from end user...
            Console.WriteLine("Enter Account number: ");
            AccountNumber = Console.ReadLine();
            timer.Start();



            //get ATA Info first
            var ReturnedATA = FOPSPage.GetAtaIp(browser, AccountNumber);
            Console.WriteLine(ReturnedATA.ToString());
            //Now Grab Radio info
            var ReturnedRadio = FOPSPage.GetRadioIP(browser, AccountNumber);
            timer.Stop();
            TimeSpan timeSpan = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Console.WriteLine($"Results: \n{ReturnedRadio}\n{ReturnedATA}\nEND>>>");
            Console.WriteLine($"RunTime: {elapsedTime}");
            System.Windows.Forms.Clipboard.SetText($"Radio:\n{ReturnedRadio}\nATA:\n{ReturnedATA}");
            Console.WriteLine("Copied data to clipboard, press any key to exit...");
            //Clean up. 
            browser.Quit();
            Console.ReadKey();
            Environment.Exit(0);
        }
    
    }
}