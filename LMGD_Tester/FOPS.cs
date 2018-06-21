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

        
    }
}
