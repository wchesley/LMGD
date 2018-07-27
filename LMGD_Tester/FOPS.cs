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
        //URL Building Strings: 
        string FOPS_HomeUrl = "https://fops.amatechtel.com";
        string FOPS_LoginUrl = "/login.asp";
        string FOPS_ATAUrl = "/tools/ataprovisioning/default.asp";
        string FOPS_RadioUrl = "/tools/su_config/default.asp"; 

        /// <summary>
        /// Redirects browser to FOPS login page, and logs in under specified user. Waits 100ms for redirect following login.
        /// idea: redirect browser back to previously viewed page?
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="password"></param>
        /// <param name="browser"></param>
        public ChromeDriver FOPS_Login(string UserID, string password, ChromeDriver browser, string PrevURL)
        {
            
            browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_LoginUrl);
            var userID = browser.FindElementById("username");
            var pswd = browser.FindElementById("password");
            var login = browser.FindElementById("login_form");
            userID.SendKeys(UserID);
            pswd.SendKeys(password);
            login.Submit();
            Thread.Sleep(100);
            browser.Navigate().GoToUrl(PrevURL);
            return browser;
        }

        /// <summary>
        /// Navigates Browser to ATA search page. Verifies FOPS login via URL check. 
        /// Attempts login if not already there. Redirects back to ATA page if not logged in.
        /// </summary>
        /// <param name="browser"></param>
        public ChromeDriver FOPS_ATA(ChromeDriver browser)
        {
            if (browser.Url == FOPS_HomeUrl + FOPS_LoginUrl)
            {
                FOPS_Login("", "", browser, FOPS_HomeUrl+FOPS_RadioUrl);
            }
            else
            {
               browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_RadioUrl);
            }
            Thread.Sleep(100);
            return browser; 
        }

        /// <summary>
        /// Navigates browser to Radio search page. Verifies FOPS login via URL check. 
        /// Attmepts login if not already there. Redirects back to Radio page if not logged in.
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        public ChromeDriver FOPS_Radio(ChromeDriver browser)
        {
            if (browser.Url == FOPS_HomeUrl + FOPS_LoginUrl)
            {
                FOPS_Login("", "", browser, FOPS_HomeUrl + FOPS_RadioUrl);
            }
            else
            {
                browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_RadioUrl);
            }
            Thread.Sleep(100);
            return browser;
        }
    }
}
