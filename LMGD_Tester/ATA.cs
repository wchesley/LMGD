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

            //Console.WriteLine(browser.Url);
            string ATAError = "ATA not found in FOPS...try again or use different account number (External ID maybe?)";

            //browser.Navigate().GoToUrl($"https://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id={AccountNumber}");
            browser.FindElementById("search_foreign_id").SendKeys(AccountNumber);
            browser.FindElementById("voip_search_submit_button").Click();



            var ATA_Table = browser.FindElementByClassName("table_row");
            var ATA_Rows = ATA_Table.FindElements(By.TagName("td"));
            Console.WriteLine($"Found ATA Type: {ATA_Rows[2].Text}");
            switch (ATA_Rows[2].Text)
            {
                //Call proper ATA method here...switch statement? will call proper method depending on what ATA type is selected in DOM. 

                case "Cambium 200P":
                case "Cambium R201P":
                    Console.WriteLine("Found Cambium ATA, Attempting Login/Reboot...");
                    //Call Cambium logic
                    break;
                case "Linksys SPA122":
                    Console.WriteLine("Found SPA122, Attempting Login/Reboot...");
                    //Call Cisco SPA122 logic
                    break;
                case "Linksys SPA2102":
                    Console.WriteLine("Found SPA2102, Attempting Login/Reboot but you might be fucked anyway lol it's a POS...");
                    //Call SPA 2102 logic
                    break;
                default:
                    Console.WriteLine(ATAError);
                    break;
            }
            //Selecting First ATA found=0
            ATA_Table.Click();
            //Console.ReadKey();
            //call search url directly as no 'clickable' link in FOPS (lame af Jason)

            browser.SwitchTo().Window(browser.WindowHandles[1]);
            string whereAmI = browser.Url;

            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after 'searching' ata... {whereAmI}");


            //await ATA IP address... 
            Console.WriteLine("Awaiting ATA page...");

            var AtaIP = browser.FindElementsByClassName("small_pad");
            //Should save changes...




            Console.WriteLine($"Saved changes clicked...{AtaIP[1].Text}");
            AtaIP[1].Click();
            Console.WriteLine($"Selecting ATA...{AtaIP[0].Text}");
            AtaIP[0].Click();
            Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");

            browser.SwitchTo().Window(browser.WindowHandles[2]); //page[0] holds FOPS search, 1 has ATA config, 2 should be ATA
            Console.WriteLine("End...");
            Console.ReadKey();
        }
        public static Boolean isAttributePresent(ChromeWebElement element, string Attribute)
        {
            Boolean isPresent = false;
            try
            {
                string value = element.GetAttribute(Attribute);
                if (value != null)
                {
                    isPresent = true;
                }
            }
            catch (Exception e) { }
            return isPresent;
        }
    }
}
