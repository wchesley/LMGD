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
        public static void GetRadioIP(ChromeDriver browser, string customerNumber)
        {
            var custNumber = browser.FindElementByName("customer_number");
            var RadioForm = browser.FindElementsByName("B1");
            //var RadioForm = browser.FindElementByXPath(@"//*[@id='div_3_contents']/form");
            custNumber.SendKeys(customerNumber);
            RadioForm[2].Submit();
            //Console.ReadKey();
            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));
            Thread.Sleep(150);
            Console.WriteLine(browser.Url);
            Console.ReadKey();


            //await table to load...
            Thread.Sleep(150);
            //Radio IP should always be first item in td for given table...will need to handle if multiple radios are presented. 
            //again need to test via ping if radio is up or not. 
            //need to determine radio type via webpage DOM 
            var RadioTable = browser.FindElementByTagName("td");
            Console.WriteLine(RadioTable.Text);

            //logic to ping radio


            browser.Navigate().GoToUrl(RadioTable.Text.ToString());
            Console.WriteLine(browser.Url);

            // logic to determine Radio type / transfer browser along proper channel here: 
            //ePMP test radio: 172.20.70.174
            //450 test radio: 172.16.98.161
            //WiMax test radio: 172.22.94.16
            //VL ? debating...

            string RadioFourFiftey = "quickform";
            string RadioWimax = "img_bg";
            string Radio_ePMP = "top-level-menu";
            if (RadioType(RadioFourFiftey, browser) == true)
            {
                ScrapeFourFifty(browser);

            }
            else if (RadioType(Radio_ePMP, browser) == true)
            {
                Scrape_ePMP(browser);
            }
            else if (RadioType(RadioWimax, browser) == true)
            {
                ScrapeWimax(browser);
            }
            else
            {
                //If going for VL, here would be the place to attempt telnet, but 450 & ePMP will accept telnet connection too. 
                Console.WriteLine("Radio was not found or is not a 450, ePMP or 320. Try again or search manually");
            }

        }
        public static bool RadioType(string id, ChromeDriver browser)
        {
            try
            {
                browser.FindElementById(id);
            }
            catch (NoSuchElementException e)
            {
                //Console.WriteLine($"Radio was not found or is not a 450, ePMP or 320 \nError code: {e.ToString()}");
                return false; 
            }
            return true; 
        }
        public static string ScrapeFourFifty(ChromeDriver browser)
        {
            string scrapedData = "Nothing found";



            return scrapedData; 
        }
        public static string Scrape_ePMP(ChromeDriver browser)
        {
            string scrapedData = "Nothing found";
            var userId = browser.FindElementById("CanopyUsername");
            var password = browser.FindElementById("CanopyPassword");
            var login = browser.FindElementById("loginbutton");
            userId.SendKeys("admin");
            password.SendKeys("amatech1");
            login.Submit();
            string upTime = browser.FindElementById("UpTime").Text;
            string RSSI = browser.FindElementById("PowerLevelOFDM").Text;
            string SNR = browser.FindElementById("SignalToNoiseRatioSM").Text;

            // go to reboot radio

            browser.FindElementByXPath("//*[@id='menu']/a[2]").Submit();
            browser.FindElementByName("reboot").Submit();
            
            //build string of Radio info prior to reboot. 
            scrapedData = $"Uptime: {upTime}\n";
            scrapedData += $"RSSI: {RSSI}\n";
            scrapedData += $"SNR: {SNR}";

            return scrapedData;
        }
        public static string ScrapeWimax(ChromeDriver browser)
        {
            string scrapedData = "Nothing found";



            return scrapedData;
        }
    }
}
