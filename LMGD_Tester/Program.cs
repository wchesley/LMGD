using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;


//Testing app for new features/Ideas/Groundwork for LMGD app

namespace LMGD_Tester
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //declare variables. 
            string FOPS = "https://fops.amatechtel.com";
            string Login = "/login.asp";
            string AtaProvisioning = "/tools/ataprovisioning/";
            string SuConfig = "/tools/su_config/default.asp";
            string username;
            string password;
            string AccountNumber = "";
            string ATAError = "ATA Error occured... ";
            FOPS FOPSPage = new FOPS();

            // attempt to load sensitive info from local xml file. 
            XElement LMGD_Doc = XElement.Load(@"C:\LMGD_Data.xml");
            username = LMGD_Doc.Element("username").Value;
            password = LMGD_Doc.Element("password").Value;

            //init headless chrome browser
            var browser = new BrowserExt().CreateHeadlessBrowser(FOPS);
            //init GUI chrome browser, mostly just for testings sake. 
            //var browser = new BrowserExt().CreateBrowser(FOPS);
            //log into FOPS
            FOPSPage.FOPS_Login(username, password, browser, FOPS + AtaProvisioning);
            //Get customer account number from end user...
            Console.WriteLine("Enter Account number: ");
            AccountNumber = Console.ReadLine();
            //Still need to ping equip first. 
            //get ATA Info first
            var ReturnedATA = FOPSPage.GetAtaIp(browser, AccountNumber);
            Console.WriteLine(ReturnedATA.ToString());
            var ReturnedRadio = FOPSPage.GetRadioIP(browser, AccountNumber);
            Console.WriteLine(ReturnedRadio.ToString());
            browser.Quit();
            Console.WriteLine($"Results: \n{ReturnedRadio}\n{ReturnedATA}\nEND>>>");
            System.Windows.Forms.Clipboard.SetText($"Radio:\n{ReturnedRadio}\nATA:\n{ReturnedATA}");
            Console.ReadKey();
        }
    
    }
}