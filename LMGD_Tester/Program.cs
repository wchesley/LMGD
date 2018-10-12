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

            // attempt to load sensitive info from local xml file. 
          
            XElement LMGD_Doc = XElement.Load(@"C:\Users\Walker\Documents\LMGD_Data.xml");
            username = LMGD_Doc.Element("username").Value;
            password = LMGD_Doc.Element("password").Value;


            //Console.WriteLine("pulled from xml: " + username);
            //Console.WriteLine("pulled from xml: " + password);
            //Console.ReadKey(); 

            //init chrome browser
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless","whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default\"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/ataprovisioning/");
            Console.WriteLine(browser.Url);
            string AccountNumber = "";
            string SelectedAtaType; 
            ////Verify we're on ATA config page. 

            ////Console.WriteLine("Awaiting Login...");
            ////    var pageTitle = browser.FindElement(By.Id("main_nav_tools_link"));


            ////Search for ATA: 
            //var AccountNumberTxtBox = browser.FindElementById("search_foreign_id");
            //var searchATA = browser.FindElementById("voip_search_submit_button");
            //AccountNumberTxtBox.SendKeys(AccountNumber);
            //searchATA.Click();

            //// Need to handle: No ata found, multiple ata's found, and no connection to FOPS. 

            ////TODO: 
            ///* 
            // * Grab All ATA's as list, if only one is found then assume it's the right one and use it. 
            // * Get ATA type from config page. then navigate control to the proper method. 
            // * Grab exact match on account number, select that ATA 
            // */
            //Console.WriteLine("Searching for ATA...");
            //var ATA = browser.FindElement(By.Id("search_results_div"));

            ////cust specific ata page is found by unique ata_id


            ////strange error searching this element. implicit/explicit waits won't work, attempting to resolve by sleeping the thread.
            ////Thread.Sleep(150);

            //Console.WriteLine("Awaiting ATA page...");
            //var AtaTable = browser.FindElementsByClassName("table_row");
            //var AtaSearchedDiv = browser.FindElementById("search_results_div");
            //Console.WriteLine($"Searching for ATA: {AtaTable}\n Found: {AtaTable.Count}");

            //foreach (var Ata in AtaTable)
            //{
            //    if (ATA.GetAttribute("ata_id") == AccountNumber)
            //    {

            //    }
            //}


            //call search url directly as no 'clickable' link in FOPS (lame af Jason)
            browser.Navigate().GoToUrl($"http://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id={AccountNumber}");


            string whereAmI = browser.Url;
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after 'searching' ata... {whereAmI}");
            Console.ReadKey();

            //await ATA IP address... 
            Console.WriteLine("Awaiting ATA page...");

            var AtaIP = browser.FindElementByClassName("button tiny expand info radius small_pad pointer");
            AtaIP.Click();
            var AtaType = browser.FindElementsByTagName("value");
            foreach(var ata in AtaType)
            {
                if(isAttributePresent((ChromeWebElement)ata, "selected") == true)
                {
                    SelectedAtaType = ata.Text; 
                    //Call proper ATA method here...switch statement?
                }
            }
            //Currntly working up to here, need to hangle migrating to ATA's webpage, ID'ing ATA type, getting information and rebooting ATA, also need to click "save Changes" on ata config page. 

            Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");
            Console.ReadKey();
            browser.SwitchTo().Window(browser.WindowHandles[3]); //page[1] holds webpage, page [2] is Data; (?idk what this is really), page [3..n] are tabs we open. 
            Console.WriteLine("Success?");
            //TODO: log into ATA, get stats based on ATA Type and reboot ATA (Save config too). or what you wrote above dipshit :P ^^^ just 6 lines
            Console.WriteLine($"{browser.Url}");
            Console.WriteLine("End...");
            Console.ReadKey();

           
            
        }
        //Helper method to see if attribute is present on desired element. 
        public static Boolean isAttributePresent(ChromeWebElement element, string Attribute)
        {
            Boolean isPresent = false;
            try
            {
                string value = element.GetAttribute(Attribute);
                if(value != null)
                {
                    isPresent = true;
                }
            }
            catch(Exception e){}
            return isPresent; 
        }
    
    }
}

