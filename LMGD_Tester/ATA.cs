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
        public BrowserExt BrowserHelper = new BrowserExt();

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
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error on ATA homepage, ref: {e.ToString()}");
                return ATAInfo = "Couldn't get into ATA"; 
            }
            
            //Gather DHCP table info.. 
            try
            {
                var DHCP_Table = browser.FindElementById("PageList");
                DHCP_Table = DHCP_Table.FindElement(By.TagName("table"));
                var DHCP_Rows = DHCP_Table.FindElements(By.TagName("tr"));
                int DHCP_Count = DHCP_Rows.Count;
                string DHCP_Info = $"{DHCP_Count.ToString()} Devices were found in DHCP Table:\n";

                //Iterate over each row, grab all DHCP items listed there 
                foreach (var Row in DHCP_Rows)
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
                    DHCP_Info += $"DCHP Item# {counter.ToString()} MAC: {info[0].Text} IP: {info[1].Text} Host Name: {info[5].Text} DHCP Status: {info[6].Text}\n";
                    counter++;
                }
                ATAInfo += DHCP_Info;
            }
            catch (NotFoundException NoDCHP)
            {
                Console.WriteLine($"Nothing found in DHCP Table {NoDCHP.ToString()}");
                ATAInfo += "Nothing found in DHCP Table";
                //throw;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Some other DHCP error occured... {e.ToString()}");
                ATAInfo += "Nothing found in DHCP table";
            }
            

            //Should be all done with ATA, go ahead and reboot dat hoe!
            browser.FindElementById("loginReboot").Click();
            
            //Combine information
            
            ATAInfo += "Rebooted / Rebuilt ATA";
            try
            {
                var handleAlert = browser.SwitchTo().Alert();
                handleAlert.Accept();
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't handle Alert, was not able to reboot ATA");
                Console.WriteLine(e.StackTrace);
            }
            return ATAInfo;
        }
        public string Spa122(ChromeDriver browser)
        {
            string ATAInfo = "";
            try
            {
                browser.FindElementByName("user").SendKeys(userName);
                browser.FindElementByName("pwd").SendKeys(passWord);
                //login button? no unique ID for it so trying this way, hopefully I can count 
                browser.FindElements(By.TagName("input"))[5].Click();
            }
            catch (ElementNotVisibleException e)
            {
                Console.WriteLine($"Ran into error: {e.ToString()} \nAttempting refresh of page.");
                //sometimes page wouldn't load for spa122, will attempt a refresh, but this could go on endlessly soooo
                //nvm just re run old code vs callin whole method. 
                browser.Navigate().Refresh();
                browser.FindElementByName("user").SendKeys(userName);
                browser.FindElementByName("pwd").SendKeys(passWord);
                //login button? no unique ID for it so trying this way, hopefully I can count 
                browser.FindElements(By.TagName("input"))[5].Click();
               // throw;
            }
            catch (Exception e)
            {
                return ATAInfo = $"Some error occured logging into ATA, ref: {e.ToString()}";
            }

            try
            {
                //Go to network setup page. 
                browser.FindElementById("trt_Network_Service.asp").Click();
                var networkSettings = browser.FindElementById("d_4");
                networkSettings.FindElement(By.TagName("a")).Click();
            }
            catch (Exception e)
            {

                return ATAInfo = $"Couldn't get into ATA {e.ToString()}";
            }
            
            //Show DHCP reservations. 
            browser.FindElementById("t3").Click();
            //id's could be numbered like an array? need to find an spa122 with mulitples to test that, too bad they're becoming rare...or is it that bad? lolol
            try
            {
                var DHCP_Name = browser.FindElementByName("dhcp_select_name_0");
                var DHCP_IP = browser.FindElementByName("dhcp_select_ip_0");
                var DHCP_MAC = browser.FindElementByName("dhcp_select_mac_0");
                ATAInfo += $"DCHP item: \nName:{DHCP_Name.Text}\nLAN IP: {DHCP_IP.Text}\nMAC: {DHCP_MAC.Text}";
            }
            catch (NotFoundException NotHere)
            {
                Console.WriteLine($"Nothing found in DHCP Table {NotHere.ToString()}");
                ATAInfo += "Nothing found in DCHP table\n";
                //throw;
            }
            
            //Considering there's only one ethernet port on the SPA122, will assume there's one item in DHCP, not always 100%
            //accurate as not every customer lets SPA122 run DHCP, but should be enough to go off of as far as equipment functionality
            //ATAInfo += $"Number of items found in DHCP: {DHCP_IP.Count.ToString()}";
            //Console.WriteLine($"Number of items found in DHCP: {DHCP_IP.Count.ToString()}");
            //for (int dhcp_list = 0; dhcp_list >= DHCP_IP.Count; dhcp_list++)
            //{
            //    ATAInfo += $"DHCP client name: {DHCP_Name[dhcp_list].Text} IP: {DHCP_IP[dhcp_list].Text} MAC: {DHCP_MAC[dhcp_list].Text}";
            //    Console.WriteLine($"DHCP client name: {DHCP_Name[dhcp_list].Text} IP: {DHCP_IP[dhcp_list].Text} MAC: {DHCP_MAC[dhcp_list].Text}");
            //}

            
            try
            {
                //Check phones
                browser.FindElementById("trt_voice.asp").Click();
                //this page is a mess, hope we really REALLY can count...you really don't wanna see this DOM man. but if you do uncomment this next line, ye be warned
                //Console.WriteLine(browser.PageSource.ToString());

                //Data I want is stored in iframe will have to switch in and out of it.
                browser.SwitchTo().Frame(browser.FindElementById("iframe"));

                //Should be leading DIV containing ALL the info related to voice/uptime. 

                var infoDiv = browser.FindElementById("Information");
                var infoTable = infoDiv.FindElements(By.TagName("tr"))[8];
                infoTable = infoTable.FindElements(By.TagName("td"))[3];
                ATAInfo += $"ATA up time: {infoTable.FindElement(By.TagName("font")).Text}";
                //Line 1: 
                infoTable = infoDiv.FindElements(By.TagName("tr"))[16];
                ATAInfo += $"Line 1 is {infoTable.FindElements(By.TagName("font"))[0].Text} hook and {infoTable.FindElements(By.TagName("font"))[1].Text} to SIP Server";

                //line 2
                infoTable = infoDiv.FindElements(By.TagName("tr"))[43]; //yeah that many rows in this table
                ATAInfo += $"Line 2 is {infoTable.FindElements(By.TagName("font"))[0].Text} hook and {infoTable.FindElements(By.TagName("font"))[1].Text} to SIP Server";
            }
            catch (Exception NoIframe)
            {
                ATAInfo += $"Couldn't get voice info, no Iframe found or trouble switching to Iframe, ref: {NoIframe.ToString()}";
                Console.WriteLine($"Couldn't get voice info, no Iframe found or trouble switching to Iframe, ref: {NoIframe.ToString()}");
                throw;
            }
            
            Console.WriteLine("Going to reboot ATA...");
            //Getting out of iframe, back into orginal DOM
            browser.SwitchTo().DefaultContent();
            //move to reboot ATA
            browser.FindElementById("trt_Management.asp").Click();
            var adminPage = browser.FindElementById("d_20");
            //should be reboot button. 
            adminPage.FindElement(By.TagName("a")).Click();
            browser.FindElementById("t4").Click();
            //handle JS alert 
            try
            {
                BrowserHelper.HandleAlerts(browser);
            }
            catch (NoAlertPresentException BadAlert)
            {
                Console.WriteLine($"No alert found when attempting reboot, are lines in use? ref: {BadAlert.ToString()}");
                ATAInfo += "No alert found when attempting reboot, are lines in use?"; 
               // throw;
            }
            
            return ATAInfo;
        }

        public string Spa2102(ChromeDriver browser)
        {
            string ATAInfo = "Found SPA2102, just create a ticket a this point, this thing is an\nancient piece of junk that needs to be replaced. ";
            BrowserHelper.CreateBrowser(browser.Url);
            return ATAInfo;
        }

        
    }
}
