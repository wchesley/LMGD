using System;

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
            //init headless chrome browser
            var browser = new BrowserExt().CreateHeadlessBrowser(FOPS);
            //init GUI chrome browser, mostly just for testings sake. 
            //var browser = new BrowserExt().CreateBrowser(FOPS);
            FOPSPage.FOPS_Login(browser, FOPS + AtaProvisioning);
            
            
            //Get customer account number from end user...
            Console.WriteLine("Enter Account number: ");
            AccountNumber = Console.ReadLine(); 
            //get ATA Info first
            var ReturnedATA = FOPSPage.GetAtaIp(browser, AccountNumber);
            Console.WriteLine(ReturnedATA.ToString());
            var ReturnedRadio = FOPSPage.GetRadioIP(browser, AccountNumber);
            Console.WriteLine(ReturnedRadio.ToString());
            
            Console.WriteLine($"Results: \n{ReturnedRadio}\n{ReturnedATA}\nEND>>>");
            System.Windows.Forms.Clipboard.SetText($"Radio:\n{ReturnedRadio}\nATA:\n{ReturnedATA}");
            browser.Quit();
            Console.ReadKey();
            Environment.Exit(0);
        }
    
    }
}