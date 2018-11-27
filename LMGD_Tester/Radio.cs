using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium; 

namespace LMGD_Tester
{
    class Radio
    {
        public BrowserExt BrowserHelper = new BrowserExt();
        /// <summary>
        /// Check webpage for specific elements related to each radio then transfers control to 
        /// the proper method. If nothing is found
        /// </summary>
        /// <param name="browser"></param>
        public string GetRadioType(ChromeDriver browser)
        {
            string radio = "Nothing found..."; 
            BrowserExt TestRadio = new BrowserExt();
            if(TestRadio.IsElemenentPresent(By.Id("CanopyUsername"), browser) == true)
            {
                Console.WriteLine("Found 450 Radio...attempting login/reboot");
                return radio = ScrapeFourFifty(browser);

            }
            else if(TestRadio.IsElemenentPresent(By.Id("loginBtn"), browser) == true)
            {
                Console.WriteLine("Found ePMP radio...attempting login/reboot");
                return radio = Scrape_ePMP(browser);
            }
            else if(TestRadio.IsElemenentPresent(By.Id("login_form"), browser) == true)
            {
                Console.WriteLine("Found Wimax radio...attempting login/reboot");
               return radio = ScrapeWimax(browser);
            }
            else
            {
                Console.WriteLine("Couldn't determine radio type...exiting app");
                browser.Quit();
                return radio;
            }
        }
        //450 Radio scraping logic
        public string ScrapeFourFifty(ChromeDriver browser)
        {


            /*Shit list:  
           #1 finding/Navigating to correct page...might try via <a> element array
           #2 Rebooting the radio, button does not load in headless chrome browser .
           #3 who knows, but I'm sure I'll find something. 
           */

            /*
            IDEA FOR NAVIGATING 450 Radio
            for whatever freaking reason, 450's combine this data as it's webpages
            order: /main.cgi?mac_esn=0a003e433075 &catindex=1& pageindex=0& ession=1481765933
            alt method of navigation would be to build the url's each time, just change the values to garuntee we get where we are wanting to go. 
            <input type="hidden" name="mac_esn" value="0a003e41ad1e" id="mac_esn">
            <input type="hidden" name="catindex" value="1" id="catindex">
            <input type="hidden" name="pageindex" value="0" id="pageindex">
            <input type="hidden" name="Session" value="1191391529" id="Session">
            */

            //Login to Radio
            var userId = browser.FindElementById("CanopyUsername");
            var Radiopassword = browser.FindElementById("CanopyPassword");
            var login = browser.FindElementById("loginbutton");
            userId.SendKeys("admin");
            Radiopassword.SendKeys("amatech1");
            login.Submit();
            Console.WriteLine("Logged into 450");
            //Gather Stats
            var FourFifty_Uptime = browser.FindElementById("UpTime").Text;
            var FourFifty_EthernetStats = browser.FindElementById("LinkStatusMain").Text;
            var FourFifty_Rssi = browser.FindElementById("PowerLevelOFDM").Text;
            var FourFifty_Snr = browser.FindElementById("SignalToNoiseRatioSM").Text;

            //Tested and working code to find & reboot 450 Radio
            browser.FindElementsByClassName("menu")[1].Click();
            Thread.Sleep(100);
            var rebootTestForm = browser.FindElementById("reboot");
            rebootTestForm.Click();

            //Return Stats
            var FourFifty_Stats = $"Uptime: {FourFifty_Uptime}\n";
            FourFifty_Stats += $"RSSI: {FourFifty_Rssi}\n";
            FourFifty_Stats += $"SNR: {FourFifty_Snr}\n";
            FourFifty_Stats += $"Ethernet Status: {FourFifty_EthernetStats}";
            return FourFifty_Stats;
	    }
	    

	 // ePMP Radio Scraping Logic: 
	    
        public string Scrape_ePMP(ChromeDriver browser)
        {
            string RadioStats = null;
            var userId = browser.FindElementByName("username");
            var pwd = browser.FindElementByName("password");
            var login = browser.FindElementById("loginBtn");
            
            userId.SendKeys("admin");
            pwd.SendKeys("amatech1");
            login.Click();
            /******************************************************************************************
             * Note for Error handling: 
             * Max # of users: <span class="error-text">Maximum number of users reached.</span>
             * Wrong Username/Password: <span class="error-text">Wrong username or password</span>
            ******************************************************************************************/
            


            //Have to set explicit waits for ePMP DOM to load...wireless connections be slow
            Thread.Sleep(500);
            try
            {
                var ePMPRssi = browser.FindElementById("dl_rssi").GetAttribute("title");
                var ePMPSNR = browser.FindElementById("dl_snr").GetAttribute("title");
                //var ePMP_EthernetStatus = browser.FindElementsById("alert-success").GetAttribute("title");
                var ePMPUptime = browser.FindElementById("sys_uptime").GetAttribute("title");
                var ePMP_DlMod = browser.FindElementById("dl_mcs_mode").GetAttribute("title");
                var ePMP_ULMod = browser.FindElementById("ul_mcs_mode").GetAttribute("title");
                RadioStats = $"{ePMPUptime.ToString()}\n";
                RadioStats += $"{ePMPRssi.ToString()}\n";
                RadioStats += $"{ePMPSNR.ToString()}\n";
                RadioStats += $"{ePMP_DlMod.ToString()}\n";
                RadioStats += $"{ePMP_ULMod.ToString()}\n";
            }
            catch (NoSuchElementException NoElement)
            {
                Console.WriteLine($"ePMP elements not found ref: {NoElement.ToString()}");
                //throw;
            }
            

            // reboot req handling popup
            // ref stackoverflow: https://stackoverflow.com/questions/12744576/selenium-c-sharp-accept-confirm-box
            //string pageSrc = browser.PageSource;
            
            //find & click reboot button, currently not visible in DOM 
            browser.FindElementByClassName("navbar-toggle").Click();
            Thread.Sleep(500);
            browser.FindElementById("reboot_device").Click();
            

            
            
            //Handle pop up asking us if we're sure we want to reboot, yes we are. 
            BrowserHelper.HandleAlerts(browser);

            //Title's in ePMP radio are "preformated", will concantenate strings together and display. 
            

            Console.WriteLine(RadioStats);
            Console.WriteLine("ePMP Complete...");
            return RadioStats; 
            

             //  return scrapedData;
        }
        public string ScrapeWimax(ChromeDriver browser)
        {
            /* WIMAX RADIOS
             * cannot get stats headlessly, will boot into graphical browser instead... 
	        test IP: 172.28.151.210
	        Shit list: 
	        Rebooting: can call via Js on ajaxReboot();
            slow transfer speeds - wireless connection, might need explicit waits might have insanly high ping too
	        */

            var login = browser.FindElementByName("login_form");
            var usrID = browser.FindElementByName("username");
            var pwd = browser.FindElementByName("passwd");
            usrID.SendKeys("administrator");
            pwd.SendKeys("ama@dmin");
            login.Submit();
            ((IJavaScriptExecutor)browser).ExecuteScript("ajaxReboot();");
            var WimaxRadio = "Wimax Rebooted \nEnd...";
            Console.ReadKey();
            return WimaxRadio; 
        }
    }
}