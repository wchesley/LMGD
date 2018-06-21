using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
 

//Testing app for new features/Ideas/Groundwork for LMGD app

namespace LMGD_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //init chrome browser
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            string FOPS_Login = "https://fops.amatechtel.com/login.asp";
            browser.Navigate().GoToUrl(FOPS_Login);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys("");
            pswd.SendKeys("");
            login.Submit();
            
            
            browser.Navigate().GoToUrl("https://fops.amatechtel.com/tools/ataprovisioning/");
            Console.WriteLine(browser.Url);
            


            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));

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


            var AccountNumber = browser.FindElementById("search_foreign_id");
            var searchATA = browser.FindElementById("voip_search_submit_button");
            AccountNumber.SendKeys("802059");
            searchATA.Click();

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



            OpenQA.Selenium.Support.UI.WebDriverWait waitTable = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));
            string ataID = string.Empty;
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

            //string script = @"$('#search_results_div').on('click', '.table_row', function() { ata_id=$(this).attr('ata_id'); window.open('http://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id=' + ata_id); } );";

            //browser.ExecuteScript(script);
            //will need to set implicit wait here. 
            //Console.WriteLine("pause for js");
            //Console.ReadKey();
            
            string whereAmI = browser.Url;
            Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
            Console.WriteLine($"Browser Location, after 'searching' ata... {whereAmI}");
            Console.ReadKey();
           
            //returns data: ...
            
            //Console.ReadKey();
            //potential solution: 
            //browser.FindElementByXPath("//*[@id='search_results_div']/div/form/fieldset/table/tbody/tr[2]").Click();

            //await ATA IP address... 
            Func<IWebDriver, bool> WaitForAtaIP = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting ATA page...");
                
                //ATA Div Xpath: //*[@id="modify_ata_form"]/fieldset/div[1]

                // stuck here wiht finding customer ATA div with IP address. would be easier if someone left an ID tag on it but... 
                var pageTitle = webDriver.FindElement(By.ClassName(" info "));
                if (pageTitle.Displayed  == true)
                {
                    pageTitle.Click();
                    return true;
                }
                return false;
            });
            try
            {
                wait.Until(WaitForAtaIP);
            }
            catch
            {
                Console.WriteLine("Element: ATA ID not found");
            }
            
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

