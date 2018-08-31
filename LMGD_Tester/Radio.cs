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


	var 450_Links = FindElementsByTag("a");
	foreach link in 450_Links
	{
		if (link.Text == "Configuration")
		{
			browser.Click();
			Thread.Sleep(100);
			string Reboot450_JS = "var reboot = document.createElement('input'); ";
			Reboot450_JS += "reboot.type = 'submit'; ";
			Reboot450_JS += "reboot.value = 'Reboot'; ";
			Reboot450_JS += "reboot.name = 'reboot'; ";
			Reboot450_JS += "reboot.id = 'MyRoboBooter'";
			Reboot450_JS += "document.body.appendChild(reboot);";
			browser.FindElementById("MyRoboBooter").Click;
		}
	}

	var 450_Stats = $"Uptime: {450_Uptime}\n";
	450_Stats += $"RSSI: {450_Rssi}\n";
	450_Stats += $"SNR: {450_Snr}\n";
	450_Stats += $"Ethernet Status: {450_EthernetStats}"; 
	return 450_Stats;
	

            return scrapedData; 
        }
	    
	 // ePMP Radio Scraping Logic: 
	    
        public static string Scrape_ePMP(ChromeDriver browser)
        {
            //string scrapedData = "Nothing found";
            var userId = browser.FindElementById("CanopyUsername");
            var password = browser.FindElementById("CanopyPassword");
            var login = browser.FindElementById("loginbutton");
            userId.SendKeys("admin");
            password.SendKeys("amatech1");
            login.Submit();
            
            var ePMPRssi = browser.FindElementById("dl_rssi").GetAttribute("title");
            var ePMPSNR = browser.FindElementById("dl_snr").GetAttribute("title");
            var ePMP_EthernetStatus = browser.FindElementsById("alert-success").GetAttribute("title");
            var ePMPUptime = browser.FindElementById("sys_uptime").GetAttribute("title");
            var ePMP_DlMod = browser.FindElementById("dl_mcs_mode").GetAttribute("title");
            var ePMP_ULMod = browser.FindElementById("ul_mcs_mode").GetAttribute("title");

            // reboot req handling popup
            // ref stackoverflow: https://stackoverflow.com/questions/12744576/selenium-c-sharp-accept-confirm-box

            browser.FindElementById("reboot_device").Click();
            bool presentAlert = false; 
            string JSAlertError = null; 
            string RadioStats = null; 
            try {
	            var handleAlert = browser.SwitchTo().handleAlert();
	            presentAlert = true;
	            handleAlert.accept();
                }
            catch (Exception e)
            {
	            string JSAlertError = e.ToString();
	            e.printStackTrace();
            }
            if (JSAlertError == nullorBlankSpace)
            {
            	return JSAlertError;
            }
            else{
	            //Title's in ePMP radio are "preformated", will concantenate strings together and display. 
	            RadioStats = $"{ePMPUptime.ToString()}\n";
	            RadioStats += $"{ePMPRssi.ToString()}\n";
	            RadioStats += $"{ePMPSNR.ToString()}\n";
	            RadioStats += $"{ePMP_EthernetStatus.ToString()}\n";
	            RadioStats += $"{ePMP_DlMod.ToString()}\n";
	            RadioStats += $"{ePMP_ULMod.ToString()}\n";
	            return RadioStats; 
            }

             //  return scrapedData;
        }
        public static string ScrapeWimax(ChromeDriver browser)
        {
            string scrapedData = "Nothing found";



            return scrapedData;
        }
    }
}
