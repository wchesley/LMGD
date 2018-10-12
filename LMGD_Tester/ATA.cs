using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LMGD_Tester
{
    class ATA
    {
       

        /// <summary>
        /// Using current browser session, takes Customer account number and attempts to find ATA IP Address in FOPS. 
        /// TODO: Error handling
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="AccountNumber"></param>
        public static void GetAtaIp(ChromeDriver browser, string AccountNumber)
        {
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/ataprovisioning/");
            Console.WriteLine(browser.Url);

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

           var AtaIP = browser.FindElement(By.ClassName(" info "));
                
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
    }
}
