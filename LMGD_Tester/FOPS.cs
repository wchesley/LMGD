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
    class FOPS
    {
        //URL Building Strings: 
        private const string FOPS_HomeUrl = "https://fops.amatechtel.com";
        private const string FOPS_LoginUrl = "/login.asp";
        private const string FOPS_ATAUrl = "/tools/ataprovisioning/default.asp";
        private const string FOPS_RadioUrl = "/tools/su_config/default.asp";
        public Pinger PingTest = new Pinger(); 


        /// <summary>
        /// Redirects browser to FOPS login page, and logs in under specified user. Waits 100ms for redirect following login.
        /// idea: redirect browser back to previously viewed page?
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="password"></param>
        /// <param name="browser"></param>
        public ChromeDriver FOPS_Login(string UserID, string password, ChromeDriver browser, string PrevURL)
        {
            
            browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_LoginUrl);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys(UserID);
            pswd.SendKeys(password);
            login.Submit();
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
                FOPS_Login("wchesley","fuimdrunk1",browser,FOPS_HomeUrl+FOPS_ATAUrl);
            }
            browser.FindElementById("search_foreign_id").SendKeys(AccountNumber);
            browser.FindElementById("voip_search_submit_button").Click();

            //TODO: Handle Multiple ATA's found. Need good way of verifiying we've selected the right customer
            //Selects First ATA found in table
            var ATA_Table = browser.FindElementsByClassName("table_row");
            //assume first row has our ATA
            var ATA_Rows = ATA_Table[0].FindElements(By.TagName("td"));
            //to get ATA Type
            Console.WriteLine($"Found ATA Type: {ATA_Rows[2].Text}");
            
            string ataType = ATA_Rows[2].Text;
            var ataSuspended = ATA_Rows[9]; //checks if ata is suspended. should be yes or no. 
            //wowee have to call explicit wait to get this crap site to 'accept' my click, lol headless browser be too quick son!
            Thread.Sleep(100);
            //Stuck here, feel like this isn't the first time either, will need to mitigate this. 
            ATA_Table[0].Click();


            //ATA config page opens in new tab. cannot call URL dirctly. returns error
            browser.SwitchTo().Window(browser.WindowHandles[1]);
            string whereAmI = browser.Url;
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after searching for ATA... {whereAmI}");

            //Finding ATA's last known IP and rebuild it's config. 
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
            switch (ataType)
            {
                //Call proper ATA method here...switch statement? will call proper method depending on what ATA type is specified in DOM. 

                case "Cambium 200P":
                case "Cambium R201P":
                    Console.WriteLine("Found Cambium ATA, Attempting Login/Reboot...");
                    //Call Cambium logic return to ATA_Info;
                    ATA_Info += PingTest.PingBuilder(browser, "ATA Cambium");
                    break;
                case "Linksys SPA122":
                    Console.WriteLine("Found SPA122, Attempting Login/Reboot...");
                    //Call Cisco SPA122 logic
                    ATA_Info += PingTest.PingBuilder(browser, "ATA SPA122");
                    break;
                case "Linksys SPA2102":
                    Console.WriteLine("Found SPA2102, Attempting Login/Reboot but you might be fucked anyway lol it's a POS...");
                    //Call SPA 2102 logic
                    ATA_Info += PingTest.PingBuilder(browser, "ATA SPA2102");
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
        /// <returns type='string'></returns>
        public string GetRadioIP(ChromeDriver browser, string customerNumber)
        {
            Console.WriteLine("Searching for Radio...");
            var GetRadio = new Radio();
            string Radio_Info = "";
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/su_config/default.asp");
            if(browser.Url.ToString().Contains(FOPS_LoginUrl)==true)
            {
                FOPS_Login("wchesley","fuimdrunk1",browser,FOPS_HomeUrl+FOPS_RadioUrl);
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
            //go to ping/crawl radio. 
            Radio_Info += PingTest.PingBuilder(browser, "radio");
            
            
            return Radio_Info;
        }
    }
}
