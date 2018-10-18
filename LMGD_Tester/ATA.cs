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
       
        public string Cambium(ChromeDriver browser, string FoundATA)
        {
            //Browser should be looking at ATA page by now
            string ATAInfo = "";
            browser.FindElementById("user_name").SendKeys("admin");
            browser.FindElementById("password").SendKeys(".amaph0n3");
            return ATAInfo;
        }
        public string Spa122(ChromeDriver browser, string FoundATA)
        {
            string ATAInfo = "";
            return ATAInfo;
        }
        public string SPA2102(ChromeDriver browser, string FoundATA)
        {
            string ATAInfo = "";
            return ATAInfo;
        }

        
    }
}
