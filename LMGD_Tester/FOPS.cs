using System;
using System.Xml.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace LMGD_Tester
{
    class FOPS
    {
        //URL Building Strings: 
        private const string FOPS_HomeUrl = "https://fops.amatechtel.com";
        private const string FOPS_LoginUrl = "/login.asp";
        private const string FOPS_ATAUrl = "/tools/ataprovisioning/default.asp";
        private const string FOPS_RadioUrl = "/tools/su_config/default.asp";
        private static string username = FOPS_LoginUser();
        private static string password = FOPS_LoginPass();
        public Pinger PingTest = new Pinger();


        // attempt to load sensitive info from local xml file.
        //TODO: Consolidate calls to disk into one method. 
        private static string FOPS_LoginUser()
        {
            string error = "unreachable, but here to avoid compile error"; 
            try
            {
                XElement LMGD_Doc = XElement.Load(@"C:\LMGD_Data.xml");
                string username = LMGD_Doc.Element("username").Value;
                //string password = LMGD_Doc.Element("password").Value;
                return username;
                //log into FOPS
                //FOPSPage.FOPS_Login(username, password, browser, FOPS + AtaProvisioning);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Error Loading data file from disk: {e.ToString()}\nPres any key to quit... ");
                Console.ReadKey();
                Environment.Exit(0);
                return error;
            }
            catch (Exception e2)
            {
                Console.WriteLine($"Some other Error occured, please review: {e2.ToString()}\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
                return error;
            }
            
        }
        // attempt to load sensitive info from local xml file.
        private static string FOPS_LoginPass()
        {
            string error = "unreachable, but here to avoid compile error";
            try
            {
                XElement LMGD_Doc = XElement.Load(@"C:\LMGD_Data.xml");
                string password = LMGD_Doc.Element("password").Value;
                return password;
                //log into FOPS
                //FOPSPage.FOPS_Login(username, password, browser, FOPS + AtaProvisioning);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Error Loading data file from disk: {e.ToString()}\nPres any key to quit... ");
                Console.ReadKey();
                Environment.Exit(0);
                return error;
            }
            catch (Exception e2)
            {
                Console.WriteLine($"Some other Error occured, please review: {e2.ToString()}\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
                return error;
            }

        }
        /// <summary>
        /// Redirects browser to FOPS login page, and logs in under specified user. Waits 100ms for redirect following login.
        /// idea: redirect browser back to previously viewed page?
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="password"></param>
        /// <param name="browser"></param>
        public ChromeDriver FOPS_Login(ChromeDriver browser, string PrevURL)
        {
            try
            {
                browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_LoginUrl);
                var userID = browser.FindElementById("username");
                var pswd = browser.FindElementById("password");
                var login = browser.FindElementById("login_form");
                userID.SendKeys(username);
                pswd.SendKeys(password);
                login.Submit();
                browser.Navigate().GoToUrl(PrevURL);
                return browser;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error logging into FOPS: \n{e.ToString()}");
                throw;
            }
            
        }

        /// <summary>
        /// Using current browser session, takes Customer account number and attempts to find ATA IP Address in FOPS. 
        /// Then transfers browser control to specific ATA method as described in ATA's config page. 
        /// Reboots and "Saves Changes" for ATA's. 
        /// Returns: String with ATA's information. 
        /// TODO: Error handling
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="AccountNumber"></param>
        
        public string GetAtaIp(ChromeDriver browser, string AccountNumber)
        { 
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 
            //logic to search url and verify not on login page. 
            //Console.WriteLine(browser.Url);
            
            string ATAError = "ATA not found in FOPS...try again or use different account number (External ID maybe?)";
            string ATA_Info = "";
            var GetATA = new ATA();
            //Verify Logged into FOPS, then Search for ATA
            try
            {
                browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_ATAUrl);
                if (browser.Url.ToString().Contains(FOPS_LoginUrl) == true)
                {
                    FOPS_Login(browser, FOPS_HomeUrl + FOPS_ATAUrl);
                }
            }
            catch (TimeoutException)
            {
                return ATAError += " Timeout Error on FOPS page, try again..."; 
            }
            catch(StaleElementReferenceException e)
            {
                return ATAError += $"Stale element ref exception, was not able to get to ATA page, please try again. ref code: {e.ToString()} "; 
            }
            catch(Exception e)
            {
                return ATAError += " " + e.ToString();
            }
            try
            {
                browser.FindElementById("search_foreign_id").SendKeys(AccountNumber);
                browser.FindElementById("voip_search_submit_button").Click();

                //TODO: Handle Multiple ATA's found. Need good way of verifiying we've selected the right customer
                string ataType = string.Empty;
                var ATA_Table = browser.FindElementsByClassName("table_row");
                //wowee have to call explicit wait to get this crap site to 'accept' my click, lol headless browser be too quick son!
                Thread.Sleep(100);
                //assume first row has our ATA
                for (int count = 0; count == ATA_Table.Count; count++)
                {
                    var ATA_Rows = ATA_Table[count].FindElements(By.TagName("td"));
                    if(GetATA.CorrectATA(ATA_Rows[0].Text,AccountNumber) == true)
                    {
                        ATA_Info = $"{ATA_Rows[0].Text}\nATA Type: {ATA_Rows[2].Text}";
                        ataType = ATA_Rows[2].Text;
                        ATA_Table[count].Click();
                        count = ATA_Table.Count;
                    }
                }
                

                //ATA config page opens in new tab. cannot call URL dirctly. returns error
                browser.SwitchTo().Window(browser.WindowHandles[1]);
                string whereAmI = browser.Url;
                Console.WriteLine($"Number of tabs: {browser.WindowHandles.Count}");
                Console.WriteLine($"Browser Location, after searching for ATA... {whereAmI}");

                //Finding ATA's last known IP and rebuild it's config. 
                var AtaIP = browser.FindElementsByClassName("small_pad");
                //Should save changes...
                Console.WriteLine($"Saved changes clicked...{AtaIP[1].Text}");
                AtaIP[1].Click();
                //Should have ATA IP
                Console.WriteLine($"Selecting ATA...{AtaIP[0].Text}");
                string ATA_IP = AtaIP[0].Text;
                AtaIP[0].Click();
                //page[0] holds FOPS search, 1 has ATA config, 2 should be ATA
                browser.SwitchTo().Window(browser.WindowHandles[2]);
                Console.WriteLine($"If ata found/ip addr clicked number of tabs is now: {browser.WindowHandles.Count}");
            
                switch (ataType)
                {
                    //Call proper ATA method here...switch statement? will call proper method depending on what ATA type is specified in DOM. 

                    case "Cambium 200P":
                    case "Cambium R201P":
                        Console.WriteLine("Found Cambium ATA, Attempting Login/Reboot...");
                        //Call Cambium logic return to ATA_Info;
                        ATA_Info += PingTest.PingBuilder(browser, "ATA Cambium", ATA_IP);
                        break;
                    case "Linksys SPA122":
                        Console.WriteLine("Found SPA122, Attempting Login/Reboot...");
                        //Call Cisco SPA122 logic
                        ATA_Info += PingTest.PingBuilder(browser, "ATA SPA122", ATA_IP);
                        break;
                    case "Linksys SPA2102":
                        Console.WriteLine("Found SPA2102, Attempting Login/Reboot but you might be fucked anyway lol it's a POS...");
                        //Call SPA 2102 logic
                        ATA_Info += PingTest.PingBuilder(browser, "ATA SPA2102", ATA_IP);
                        break;
                    default:
                        Console.WriteLine(ATAError);
                        ATA_Info = ATAError;   
                        break;
                }

            }
            catch (Exception e)
            {
                ATA_Info = $"Some other error: {e.ToString()}";
                throw;
            }

            Console.WriteLine("End ATA...");
            
            return ATA_Info;
        }

        /// <summary>
        /// Navigates browser to Radio search page. Verifies FOPS login via URL check. 
        /// Attmepts login if not already there. Redirects back to Radio page if not logged in.
        /// </summary>
        /// <param name="browser"></param>
        /// <returns type='string'></returns>
        public string GetRadioIP(ChromeDriver browser, string customerNumber)
        {
            Console.WriteLine("Searching for Radio...");
            var GetRadio = new Radio();
            string Radio_Info = "";
            try
            {
                browser.Navigate().GoToUrl(FOPS_HomeUrl + FOPS_RadioUrl);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine($"Timed out getting to SU Config...ref:\n{e.ToString()}");
                return Radio_Info; 
            }
            catch(Exception e)
            {
                return Radio_Info = $"Some other error: {e.ToString()}";
            }
            if(browser.Url.ToString().Contains(FOPS_LoginUrl)==true)
            {
                FOPS_Login(browser,FOPS_HomeUrl+FOPS_RadioUrl);
            }
            try
            {
                var custNumber = browser.FindElementByName("customer_number");
                var RadioForm = browser.FindElementsByName("B1");
                //var RadioForm = browser.FindElementByXPath(@"//*[@id='div_3_contents']/form");
                custNumber.SendKeys(customerNumber);
                RadioForm[2].Submit();

                Console.WriteLine(browser.Url);


                //Radio IP should always be first item in td for given table...will need to handle if multiple radios are presented. 
                //again need to test via ping if radio is up or not. 
                //need to determine radio type via webpage DOM 
                var RadioTable = browser.FindElementByTagName("td");
                Console.WriteLine($"Found Radio IP: {RadioTable.Text}");
                //created to avoid stale refrence exception on later call. 
                string radioIP = RadioTable.Text;
                //Uri RadioIP = RadioTable.Text; 
                //logic to ping radio goes here. 

                if (RadioTable.Text.Contains("Nothing"))
                {
                    return Radio_Info += "No IP found for radio";
                }
                browser.Navigate().GoToUrl($"http://{RadioTable.Text}");
                Console.WriteLine(browser.Url);
                //go to ping/crawl radio. 
                Radio_Info += PingTest.PingBuilder(browser, "radio", radioIP);


                return Radio_Info;
            }
            catch (Exception e)
            {
                return Radio_Info = $"error: {e.ToString()}";
            }
            
        }
    }
}
