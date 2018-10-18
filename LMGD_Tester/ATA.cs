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
    class ATA
    {
        private const string userName = "admin";
        private const string passWord = ".amaph0n3";
        public string Cambium(ChromeDriver browser)
        {
            //Browser should be looking at ATA page by now
            string ATAInfo = "";
            browser.FindElementById("user_name").SendKeys(userName);
            browser.FindElementById("password").SendKeys(passWord);
            var loginBtn = browser.FindElementById("login");
            if(loginBtn.Displayed == true)
            {
                loginBtn.Click();
            }
            else
            {
                //If login button is not present we'll create it ourselves same way DOM originally does it. 
                ((IJavaScriptExecutor)browser).ExecuteScript("document.write('<input id='login' type='submit' onclick='OnSubmit(this.form);');");
            }
            //Get phone lines and their status, format and add to ATAInfo string. 
            var line1 = browser.FindElementById("sipStatus_1").Text;
            var line1_hook = browser.FindElementById("hookStatus_1").Text;
            var line1_Status = browser.FindElementById("useStatus_1").Text;
            var line2 = browser.FindElementById("sipStatus_2").Text;
            var line2_hook = browser.FindElementById("hookStatus_2").Text;
            var line2_Status = browser.FindElementById("useStatus_2").Text;
            ATAInfo = $"Phone lines in ATA:\nLine 1 is {line1} and is {line1_hook} hook and currently {line1_Status}\n";
            ATAInfo += $"Phone line 2 is {line2} and is {line2_hook} hook and currently {line2_Status}\n";


            return ATAInfo;
        }
        public string Spa122(ChromeDriver browser)
        {
            string ATAInfo = "";
            return ATAInfo;
        }
        public string Spa2102(ChromeDriver browser)
        {
            string ATAInfo = "";
            return ATAInfo;
        }

        
    }
}
