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
        /// <summary>
        /// Redirects browser to FOPS login page, and logs in under specified user. Waits 100ms for redirect following login. 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="password"></param>
        /// <param name="browser"></param>
        public static void FOPS_Login(string UserID, string password, ChromeDriver browser)
        {
            string FOPS_Login = "https://fops.amatechtel.com/login.asp";
            browser.Navigate().GoToUrl(FOPS_Login);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys(UserID);
            pswd.SendKeys(password);
            login.Submit();
            Thread.Sleep(100);
        }

        /// <summary>
        /// Using current browser session, takes Customer account number and attempts to find ATA IP Address in FOPS. 
        /// TODO: Error handling
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="AccountNumber"></param>
        public static void GetAtaIp(ChromeDriver browser, string AccountNumber)
        {
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/ataprovisioning/");
            Console.WriteLine(browser.Url);



            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));
            //Verifies we're on ATA config page. 
            Func<IWebDriver, bool> waitForLogin = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting Login...");
                var pageTitle = webDriver.FindElement(By.Id("main_nav_tools_link"));
                if (pageTitle.Displayed == true)
                {
                    return true;
                }
                return false;
            });
            wait.Until(waitForLogin);

            //input customer information. 
            var AccountNumberTxtBox = browser.FindElementById("search_foreign_id");
            var searchATA = browser.FindElementById("voip_search_submit_button");
            AccountNumberTxtBox.SendKeys(AccountNumber);
            searchATA.Click();

            // Need to handle: No ata found, multiple ata's found, and no connection to FOPS. 
            Func<IWebDriver, bool> WaitForSearch = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Searching for ATA...");
                var pageTitle = webDriver.FindElement(By.Id("search_results_div"));
                if (pageTitle.Displayed)
                {
                    return true;
                }
                return false;
            });
            wait.Until(WaitForSearch);



            
            //cust specific ata page is found by uniquie ata_id
            string ataID = string.Empty;

            //strange error searching this element. implicit/explicit waits won't work, attempting to resolve by sleeping the thread.
            Thread.Sleep(150);

            OpenQA.Selenium.Support.UI.WebDriverWait waitTable = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));
            Func<IWebDriver, bool> WaitForATA = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting ATA page...");
                var pageTitle = webDriver.FindElement(By.ClassName("table_row"));
                ataID = pageTitle.GetAttribute("ata_id");
                if (pageTitle.Displayed == true)
                {
                    pageTitle.Click();
                    return true;
                }
                return false;
            });
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            try
            {
                waitTable.Until(WaitForATA);
            }
            catch
            {
                Console.WriteLine("Error finding Table_Row");
            }

            Console.WriteLine($"Searching for ATA: {ataID}");
            browser.Navigate().GoToUrl($"http://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id={ataID}");


            string whereAmI = browser.Url;
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after 'searching' ata... {whereAmI}");
            Console.ReadKey();
            
            //await ATA IP address... 
            Func<IWebDriver, bool> WaitForAtaIP = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting ATA page...");

                var pageTitle = webDriver.FindElement(By.ClassName(" info "));
                if (pageTitle.Displayed == true)
                {
                    pageTitle.Click();
                    return true;
                }
                return false;
            });
            wait.Until(WaitForAtaIP);
            //Currntly working up to here, need to hangle migrating to ATA's webpage, ID'ing ATA type, getting information and rebooting ATA, also need to click "save Changes" on ata config page. 

            Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");
            Console.ReadKey();
            browser.SwitchTo().Window(browser.WindowHandles[3]);
            Console.WriteLine("Success?");
            Console.WriteLine($"{browser.Url}");
            Console.WriteLine("End...");
            Console.ReadKey();
        }
    }
}
