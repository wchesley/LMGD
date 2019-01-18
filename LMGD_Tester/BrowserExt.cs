using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace LMGD_Tester
{
    class BrowserExt
    {
        public ChromeDriver CreateHeadlessBrowser(string URL)
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless", "whitelisted-ips='' ", "remote-debugging-port=8000", @"user-data-directory=C:\Users\wchesley\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default
                var browser = new ChromeDriver(chromeOptions);
                browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
                browser.Navigate().GoToUrl(URL);
                return browser;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR! Couldn't start chrome session\nWould you like to try the GUI browser instead?\nY/N:");
                Console.ResetColor(); 
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "y":
                    case "Y":
                    case "yes":
                    case "Yes":
                        Console.WriteLine("Attempting to start GUI Chrome, Please note this method is much slower than headless...");
                        var browser = CreateBrowser(URL);
                        return browser; 
                    default:
                        Console.WriteLine($"ERROR: {e.ToString()}\nExiting app...");
                        Environment.Exit(0);
                        break;
                }
                throw;
            }
            
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


        public static bool IsAttributePresent(ChromeWebElement element, string Attribute)
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
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            return isPresent;
        }
        //Logic if Element is present. 
        public bool IsElemenentPresent(By by, ChromeDriver browser)
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
