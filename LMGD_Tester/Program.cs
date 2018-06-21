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
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default\Default");
            var browser = new ChromeDriver(chromeOptions);
            string FOPS_Login = "https://fops.amatechtel.com/login.asp";
            browser.Navigate().GoToUrl(FOPS_Login);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys("wchesley");
            pswd.SendKeys("fuimdrunk1");
            login.Submit();
            Console.ReadKey();
            //var CookieOne = new Cookie("ASPSESSIONIDSACSRSTR","CKAMGCHAEBPNIKGPNBOLFJMP");
            //var CookieTwo = new Cookie("ASPSESSIONIDQCAQSQTR","ANCGFODBINMGEOHIEGNGLBFN");
            //browser.Manage().Cookies.AddCookie(CookieOne);
            //browser.Manage().Cookies.AddCookie(CookieTwo);
            
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
            //Stuck here now, trying to handle new window opening and actually selecting ATA's... 
            Func<IWebDriver, bool> WaitForSearch = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Searching for ATA...");
                var pageTitle = webDriver.FindElement(By.Id("search_results_div"));
                if (pageTitle.Displayed == true)
                {
                    //pageTitle.Click();
                    
                    Console.WriteLine(browser.Url);

                    //pageTitle = browser.FindElementByClassName("table_row");
                    //pageTitle.Click();
                    //var popup = browser.WindowHandles[1];
                    //browser.SwitchTo().Window(browser.WindowHandles[1]);
                    return true;
                }
                return false;
            });
            wait.Until(WaitForSearch);
            //try
            //{
            //    IJavaScriptExecutor jsExecutor = browser as IJavaScriptExecutor;

            //    //expirament...call JS directly from Selenium browser... 
            //    jsExecutor.ExecuteScript("$('#search_results_div).on('click','table_row ," +
            //        "function() {" +
            //        "ata_id = $(this).attr('ata_id')" +
            //        "window.open('modify.asp?ata_id=' + ata_id);" +
            //        "}" +
            //        "));");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            
            
            string Ata_Id = "";
            Func<IWebDriver, bool> WaitForATA = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting ATA page...");
                var pageTitle = webDriver.FindElement(By.CssSelector("tr[ata_id]"));
                if (pageTitle.Displayed == true)
                {
                    return true;
                }
                return false;
            });
            wait.Until(WaitForATA);
            var ata_id = browser.FindElement(By.CssSelector("tr[ata_id]"));
            var ataResults = browser.FindElement(By.Id("search_results_div"));
            Ata_Id = ata_id.GetAttribute("ata_id");
            Console.WriteLine(Ata_Id);
            string script = @"$('#search_results_div').on('click', '.table_row', function() { ata_id=$(this).attr('ata_id'); window.location.href('modify.asp?ata_id=' + ata_id); } );";

            browser.ExecuteScript(script);
            // I Think this redirect is giving me issues..need to see about handeling sessions in selenium. 
            //browser.Navigate().GoToUrl($"http://fops.amatechtel.com/tools/ataprovisioning/modify.asp?ata_id={Ata_Id}");
            
            string whereAmI = browser.Url;
            Console.WriteLine($"Browser Location, after 'searchign' ata... {whereAmI}");
            Console.ReadKey();
            //potential solution: 
            //browser.FindElementByXPath("//*[@id='search_results_div']/div/form/fieldset/table/tbody/tr[2]").Click();

            //await ATA IP address... 
            Func<IWebDriver, bool> WaitForAtaIP = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            {
                Console.WriteLine("Awaiting ATA page...");
                
                //ATA Div Xpath: //*[@id="modify_ata_form"]/fieldset/div[1]

                // stuck here wiht finding customer ATA div with IP address. would be easier if someone left an ID tag on it but... 
                var pageTitle = webDriver.FindElement(By.XPath("//div[/='http://']"));
                if (pageTitle.Displayed  == true)
                {
                    return true;
                }
                return false;
            });
            wait.Until(WaitForAtaIP);
            
            Console.WriteLine("Success?");

            Console.WriteLine("End...");
            Console.ReadKey();
        }
    
    }
}

