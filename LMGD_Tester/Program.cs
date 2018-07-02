using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //init chrome browser
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default\"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

            
            browser.Navigate().GoToUrl(FOPS + Login);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            //dont' forget to reinput this shit :P
            userID.SendKeys("wchesley");
            pswd.SendKeys("fuimdrunk1");
            login.Submit();


            //browser.Navigate().GoToUrl(FOPS + AtaProvisioning); 

            browser.Navigate().GoToUrl(FOPS + SuConfig);

            Console.WriteLine(browser.Url);
            var custNumber = browser.FindElementByName("customer_number");
            var RadioForm = browser.FindElementByXPath(@"//*[@id='div_3_contents']/form");
            custNumber.SendKeys("803689");
            RadioForm.Submit();
            Console.ReadKey();
            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(browser, System.TimeSpan.FromSeconds(10));
            Thread.Sleep(150);
            Console.WriteLine(browser.Url);
            Console.ReadKey(); 

            //Func<IWebDriver, bool> waitForLogin = new Func<IWebDriver, bool>((IWebDriver webDriver) =>
            //{
            //    Console.WriteLine("Awaiting Su Config...");

            //    var pageTitle = webDriver.FindElement(By.Name("customer_number")); // #div_3_contents > form > fieldset > label:nth-child(4) > input[type="number"]
            //    if (pageTitle.Displayed == true)
            //    {
            //        pageTitle.SendKeys("307982");
            //        return true;
            //    }
            //    return false;
            //});
            //wait.Until(waitForLogin);

            
            
            
            //await table to load...
            Thread.Sleep(150);
            var RadioInTable = browser.FindElementByXPath("//*[@id='div_1_contents']/table/tbody/tr[2]/td[1]");
            string RadioIP = RadioInTable.Text;
            browser.Navigate().GoToUrl(RadioIP);
            Console.WriteLine(browser.Url);
            
            // logic to determine Radio type
            Console.WriteLine("End...");
            Console.ReadKey();
        }
    
    }
}

