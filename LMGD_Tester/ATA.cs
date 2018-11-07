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
        //Const for logging in. 
        private const string userName = "admin";
        private const string passWord = ".amaph0n3";


        public string Cambium(ChromeDriver browser)
        {
            BrowserExt ElementCheck = new BrowserExt();
            //Browser should be looking at ATA page by now
            string ATAInfo = "";
            browser.FindElementById("user_name").SendKeys(userName);
            browser.FindElementById("password").SendKeys(passWord);
           
            if(ElementCheck.isElemenentPresent(By.Id("login"),browser) == true)
            {
                browser.FindElementById("login").Click();
                //loginBtn.Click();
            }
            else
            {
                //If login button is not present we'll create it ourselves same way DOM originally does it. 
                ((IJavaScriptExecutor)browser).ExecuteScript("document.write('<input id='login' type='submit' onclick='OnSubmit(this.form);');");
                browser.FindElementById("login").Click();
            }
            //Get phone lines and their status, format and add to ATAInfo string. 
            Console.WriteLine($"Cambium DOM: {browser.PageSource}");
            Console.ReadKey();
            var line1 = browser.FindElementById("sipStatus_1").Text;
            var line1_hook = browser.FindElementById("hookStatus_1").Text;
            var line1_Status = browser.FindElementById("useStatus_1").Text;
            var line2 = browser.FindElementById("sipStatus_2").Text;
            var line2_hook = browser.FindElementById("hookStatus_2").Text;
            var line2_Status = browser.FindElementById("useStatus_2").Text;
            ATAInfo = $"Phone lines in ATA:\nLine 1 is {line1} and is {line1_hook} hook and currently {line1_Status}\n";
            ATAInfo += $"Phone line 2 is {line2} and is {line2_hook} hook and currently {line2_Status}\n";
            //Find DHCP table
            var LAN_HostBtn = browser.FindElementById("menuSubList");
            LAN_HostBtn.FindElements(By.TagName("li"))[1].Click(); 
            //Gather DHCP table info.. 
            var DHCP_Table = browser.FindElementById("PageList");
            DHCP_Table = DHCP_Table.FindElement(By.TagName("table"));
            var DHCP_Rows = DHCP_Table.FindElements(By.TagName("tr"));
            int DHCP_Count = DHCP_Rows.Count;
            string DHCP_Info = $"{DHCP_Count.ToString()} Devices were found in DHCP Table:\n";
            
            //Iterate over each row, grab all DHCP items listed there 
            foreach(var Row in DHCP_Rows)
            {
                int counter = 1;
                var info = Row.FindElements(By.TagName("td"));
                /*Should be 7 items per Entry in DHCP table
                 * 0 - MAC address
                 * 1 - LAN IP
                 * 2 - Interface Type
                 * 3 - Address Source
                 * 4 - Expires in
                 * 5 - Device Name
                 * 6 - DHCP Status
                 */
                DHCP_Info += $"#{counter.ToString()} MAC: {info[0].ToString()} IP: {info[1].ToString()} Host Name: {info[5].ToString()} DHCP Status: {info[6].ToString()}\n";
                counter++;
            }

            //Should be all done with ATA, go ahead and reboot dat hoe!
            browser.FindElementById("loginReboot").Click();
            
            //Combine information
            ATAInfo += DHCP_Info;
            ATAInfo += "Rebooted / Rebuilt ATA";
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
            return ATAInfo;
        }
        public string Spa122(ChromeDriver browser)
        {
            string ATAInfo = "";
            return ATAInfo;
        }

        public string Spa2102(ChromeDriver browser)
        {
            string ATAInfo = "Found SPA2102, just create a ticket a this point, this thing is an\nancient piece of junk that needs to be replaced. ";
            return ATAInfo;
        }

        
    }
}
