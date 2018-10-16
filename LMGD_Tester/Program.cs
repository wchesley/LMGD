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
            FOPS FOPSPage = new FOPS(); 
            // attempt to load sensitive info from local xml file. 
          
            XElement LMGD_Doc = XElement.Load(@"C:\Users\Walker\Documents\LMGD_Data.xml");
            username = LMGD_Doc.Element("username").Value;
            password = LMGD_Doc.Element("password").Value;
            

            //Console.WriteLine("pulled from xml: " + username);
            //Console.WriteLine("pulled from xml: " + password);
            //Console.ReadKey(); 

            //init chrome browser
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("whitelisted-ips=''", @"user-data-directory=C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            //manually loggin in for testing sake. 
            FOPSPage.FOPS_Login(username, password, browser, FOPS+AtaProvisioning);
            //browser.Navigate().GoToUrl(FOPS);
            //I'm getting redirected to login page? session issues?
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 
            
            //Console.WriteLine(browser.Url);
            string AccountNumber = "308666";
            string SelectedAtaType;
            //browser.Navigate().GoToUrl($"https://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id={AccountNumber}");
            browser.FindElementById("search_foreign_id").SendKeys(AccountNumber);
            browser.FindElementById("voip_search_submit_button").Click();



            browser.FindElementByClassName("table_row").Click();

            //Console.ReadKey();
            //call search url directly as no 'clickable' link in FOPS (lame af Jason)

            browser.SwitchTo().Window(browser.WindowHandles[1]);
            string whereAmI = browser.Url;
            string ATAError = "ATA Error occured... ";
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
            // not returning a list like I want... kees finding nothing really. might move ATA detection logic and go a different route, searching DOM instead of FOPS page cause FOPS is gayyyy af fam
            //var AtaType = browser.FindElementsByTagName("value");
            //foreach(var ata in AtaType)
            //{
            //    Console.WriteLine(ata.Text);
            //    if(isAttributePresent((ChromeWebElement)ata, "selected") == true)
            //    {
            //        SelectedAtaType = ata.Text; 
            //        //Call proper ATA method here...switch statement? will call proper method depending on what ATA type is selected in DOM. 
            //        switch(SelectedAtaType)
            //        {
            //            case "Cambium 200P":
            //            case "Cambium R201P":
            //            Console.WriteLine("Found Cambium ATA, Attempting Login/Reboot...");
            //                //Call Cambium logic
            //                break; 
            //            case "Linksys SPA122":
            //                Console.WriteLine("Found SPA122, Attempting Login/Reboot...");
            //                //Call Cisco SPA122 logic
            //                break; 
            //            case "Linksys SPA2102":
            //                Console.WriteLine("Found SPA2102, Attempting Login/Reboot but you might be fucked anyway lol it's a POS...");
            //                //Call SPA 2102 logic
            //                break;
            //            default:
            //            Console.WriteLine(ATAError);
            //                break;
            //        }
            //    }
            //}
            
             

            Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");
           
            browser.SwitchTo().Window(browser.WindowHandles[2]); //page[0] holds FOPS search, 1 has ATA config, 2 should be ATA 
            

           
            
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

