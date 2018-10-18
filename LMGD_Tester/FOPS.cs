﻿using System;
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
    class FOPS
    {
        //URL Building Strings: 
        public const string FOPS_HomeUrl = "https://fops.amatechtel.com";
        public const string FOPS_LoginUrl = "/login.asp";
        public const string FOPS_ATAUrl = "/tools/ataprovisioning/default.asp";
        public const string FOPS_RadioUrl = "/tools/su_config/default.asp";
        


        /// <summary>
        /// Redirects browser to FOPS login page, and logs in under specified user. Waits 100ms for redirect following login.
        /// idea: redirect browser back to previously viewed page?
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="password"></param>
        /// <param name="browser"></param>
        public ChromeDriver FOPS_Login(ChromeDriver browser, string PrevURL)
        {
            string UserID = "wchesley";
            string password = "fuimdrunk1";
            browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_LoginUrl);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys(UserID);
            pswd.SendKeys(password);
            login.Submit();
            Thread.Sleep(100);
            browser.Navigate().GoToUrl(PrevURL);
            return browser;
        }

        /// <summary>
        /// Using current browser session, takes Customer account number and attempts to find ATA IP Address in FOPS. 
        /// Then transfers browser control to specific ATA method as described in ATA's config page. 
        /// Reboots and "Saves Changes" for ATA's. 
        /// Returns: String with ATA's information. 
        /// TODO: Error handling
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="AccountNumber"></param>
        
        public string GetAtaIp(ChromeDriver browser, string AccountNumber)
        {
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 
            //logic to search url and verify not on login page. 
            //Console.WriteLine(browser.Url);
            string ATAError = "ATA not found in FOPS...try again or use different account number (External ID maybe?)";
            string ATA_Info = "";
            var GetATA = new ATA(); 
            //Verify Logged into FOPS, then Search for ATA
            browser.Navigate().GoToUrl($"https://fops.amatechtel.com/tools/ataprovisioning");
            if(browser.Url.ToString().Contains(FOPS_LoginUrl)==true)
            {
                FOPS_Login(browser,FOPS_HomeUrl+FOPS_ATAUrl);
            }
            browser.FindElementById("search_foreign_id").SendKeys(AccountNumber);
            browser.FindElementById("voip_search_submit_button").Click();

            //TODO: Handle Multiple ATA's found. Need good way of verifiying we've selected the right customer
            //Selects First ATA found in table
            var ATA_Table = browser.FindElementByClassName("table_row");
            var ATA_Rows = ATA_Table.FindElements(By.TagName("td"));
            Console.WriteLine($"Found ATA Type: {ATA_Rows[2].Text}");
            ATA_Table.Click();
            
            //ATA config page opens in new tab. cannot call URL dirctly. returns error
            browser.SwitchTo().Window(browser.WindowHandles[1]);
            string whereAmI = browser.Url;
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after 'searching' ata... {whereAmI}");

            //Finding ATA's last known IP and Resetting it's config. 
            var AtaIP = browser.FindElementsByClassName("small_pad");
            //Should save changes...
            Console.WriteLine($"Saved changes clicked...{AtaIP[1].Text}");
            AtaIP[1].Click();
            //Should have ATA IP
            Console.WriteLine($"Selecting ATA...{AtaIP[0].Text}");
            AtaIP[0].Click();
            //page[0] holds FOPS search, 1 has ATA config, 2 should be ATA
            browser.SwitchTo().Window(browser.WindowHandles[2]);
            Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");
            whereAmI = browser.Url;
            switch (ATA_Rows[2].Text)
            {
                //Call proper ATA method here...switch statement? will call proper method depending on what ATA type is selected in DOM. 

                case "Cambium 200P":
                case "Cambium R201P":
                    Console.WriteLine("Found Cambium ATA, Attempting Login/Reboot...");
                    //Call Cambium logic return to ATA_Info;
                    ATA_Info = GetATA.Cambium(browser);
                    break;
                case "Linksys SPA122":
                    Console.WriteLine("Found SPA122, Attempting Login/Reboot...");
                    //Call Cisco SPA122 logic
                    ATA_Info = GetATA.Spa122(browser);
                    break;
                case "Linksys SPA2102":
                    Console.WriteLine("Found SPA2102, Attempting Login/Reboot but you might be fucked anyway lol it's a POS...");
                    //Call SPA 2102 logic
                    ATA_Info = GetATA.Spa2102(browser);
                    break;
                default:
                    Console.WriteLine(ATAError);
                    break;
            }
            
            
            
            Console.WriteLine("End ATA...");
            
            return ATA_Info;
        }

        /// <summary>
        /// Navigates browser to Radio search page. Verifies FOPS login via URL check. 
        /// Attmepts login if not already there. Redirects back to Radio page if not logged in.
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        public string GetRadioIP(ChromeDriver browser, string customerNumber)
        {
            var GetRadio = new Radio();
            string Radio_Info = "";
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/su_config/default.asp");
            if(browser.Url.ToString().Contains(FOPS_LoginUrl)==true)
            {
                FOPS_Login(browser,FOPS_HomeUrl+FOPS_RadioUrl);
            }
            var custNumber = browser.FindElementByName("customer_number");
            var RadioForm = browser.FindElementsByName("B1");
            //var RadioForm = browser.FindElementByXPath(@"//*[@id='div_3_contents']/form");
            custNumber.SendKeys(customerNumber);
            RadioForm[2].Submit();
           
            Console.WriteLine(browser.Url);


            //Radio IP should always be first item in td for given table...will need to handle if multiple radios are presented. 
            //again need to test via ping if radio is up or not. 
            //need to determine radio type via webpage DOM 
            var RadioTable = browser.FindElementByTagName("td");
            Console.WriteLine(RadioTable.Text);

            //logic to ping radio goes here. 


            browser.Navigate().GoToUrl(RadioTable.Text.ToString());
            Console.WriteLine(browser.Url);

            // logic to determine Radio type / transfer browser along proper channel here: 
            //ePMP test radio: 172.20.70.174
            //450 test radio: 172.16.98.161
            //WiMax test radio: 172.22.94.16
            //VL ? debating...HA, no...no way

            string RadioFourFiftey = "quickform";
            string RadioWimax = "img_bg";
            string Radio_ePMP = "top-level-menu";
            //need to rework logic to use IsAttributePresent Method from BrowserExt class. 
            if (RadioType(RadioFourFiftey, browser) == true)
            {
               Radio_Info = GetRadio.ScrapeFourFifty(browser);
                
            }
            else if (RadioType(Radio_ePMP, browser) == true)
            {
                Radio_Info = GetRadio.Scrape_ePMP(browser, browser.Url.ToString());
            }
            else if (RadioType(RadioWimax, browser) == true)
            {
                Radio_Info = GetRadio.ScrapeWimax(browser);
            }
            else
            {
                //If going for VL, here would be the place to attempt telnet, but 450 & ePMP will accept telnet connection too. 
                Radio_Info = "Radio was not found or is not a 450, ePMP or 320(Wimax). Try again or search manually";
            }
            return Radio_Info;
        }
        public static bool RadioType(string id, ChromeDriver browser)
        {
            try
            {
                browser.FindElementById(id);
            }
            catch (NoSuchElementException e)
            {
                Console.WriteLine($"Radio was not found or is not a 450, ePMP or 320 \nError code: {e.ToString()}");
                return false;
            }
            return true;
        }
        public bool IsLoggedInFOPS (ChromeDriver broswer, string prevURL)
        {
            bool loggedIn = false; 
            if(broswer.Url.ToString().Contains(FOPS_LoginUrl)==true)
            {
                FOPS_Login(broswer,prevURL);
                loggedIn = true; 
            }
            return loggedIn; 
        }
        


    }
}
