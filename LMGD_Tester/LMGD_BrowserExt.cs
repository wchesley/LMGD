using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;


namespace LMGD_Tester
{
    class LMGD_BrowserExt
    {
        /// <summary>
        /// Creates new Chrome Browser
        /// TODO: Dynamic User profile, store in json/xml file or prompt end user for it's location. 
        /// </summary>
        /// <returns></returns>
        public static ChromeDriver LMGD_BrowserBuilder()
        {
            //Browser runtime options
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default"); //@ home = 1 Default, work = 2 \Default

            var browser = new ChromeDriver(chromeOptions);

            //Set wait time when looking for item in webpage. 
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return browser;
        }

        /// <summary>
        /// Injects, clicks, then goes to user specified link. Link is injected via JavaScript into webpage. 
        /// As of 6/21/2018 this is untested. 
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="url"></param>
        public static void OpenUrlInNewTab(ChromeDriver browser, string url)
        {
            //Define JS script to inject link into DOM of current webpage
            string script = "var d=document,a=d.createElement('a');a.target='_blank';a.href='" + url + "';a.innerHTML='.';d.body.appendChild(a);return a;";
            // Execute the script 
            ChromeWebElement element = (ChromeWebElement)browser.ExecuteScript(script);
            //Click the newly created link
            element.Click();
            //switch to the link. 
            browser.SwitchTo().Window(browser.WindowHandles.Last());
        }
    }
}
