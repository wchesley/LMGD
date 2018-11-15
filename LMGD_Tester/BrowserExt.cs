using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LMGD_Tester
{
    class BrowserExt
    {
        public ChromeDriver CreateHeadlessBrowser(string URL)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"user-data-directory=C:\Users\wchesley\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
            browser.Navigate().GoToUrl(URL);
            return browser;
        }
        public ChromeDriver CreateBrowser(string URL)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("whitelisted-ips=''", @"user-data-directory=C:\Users\wchesley\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
            browser.Navigate().GoToUrl(URL);
            return browser;
        }


        public static bool isAttributePresent(ChromeWebElement element, string Attribute)
        {
            bool isPresent = false;
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
        //Logic if Element is present. 
        public bool isElemenentPresent(By by, ChromeDriver browser)
        {
            bool isPresent = false;
            try
            {
                browser.FindElement(by);
                isPresent = true;
            }
            catch (NoSuchElementException)
            {
                isPresent = false;
            }
            catch(TimeoutException e)
            {
                Console.WriteLine($"Timed out looking for element on page, ref: {e.ToString()}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Some other error occured looking for element: {e.ToString()}");
            }

            return isPresent; 
        }
        public void HandleAlerts(ChromeDriver browser)
        {
            string JSAlertError = null;
            try
            {
                var handleAlert = browser.SwitchTo().Alert();

                handleAlert.Accept();
            }
            catch (Exception e)
            {
                JSAlertError = e.ToString();
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
