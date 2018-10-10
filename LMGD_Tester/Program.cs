using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading; 
 

//Testing app for new features/Ideas/Groundwork for LMGD app

namespace LMGD_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //declare variables. 
            string FOPS = "https://fops.amatechtel.com";
            string Login = "/login.asp";
            string AtaProvisioning = "/tools/ataprovisioning/";
            string SuConfig = "/tools/su_config/default.asp";
            string username;
            string password; 

            // attempt to load sensitive info from local xml file. 
          
            XElement LMGD_Doc = XElement.Load(@"C:\Users\Walker\Documents\LMGD_Data.xml");
            username = LMGD_Doc.Element("username").Value;
            password = LMGD_Doc.Element("password").Value;


            //Console.WriteLine("pulled from xml: " + username);
            //Console.WriteLine("pulled from xml: " + password);
            //Console.ReadKey(); 

            //init chrome browser
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless", "whitelisted-ips=''", @"C:\Users\Walker\AppData\Local\Google\Chrome\User Data\Default\"); //@ home = 1 Default, work = 2 \Default
            var browser = new ChromeDriver(chromeOptions);
            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

            //Testing on: 172.28.70.184
            browser.Navigate().GoToUrl("http://172.28.80.69/");
            var userId = browser.FindElementByName("username");
            var pwd = browser.FindElementByName("password");
            var login = browser.FindElementById("loginBtn");
            var JS_Builder = "document.getElementById('login1').removeAttribute('class','readonly'); ";

            //JS_Builder += "document.getElementById('loginBtn').click();";
            //((IJavaScriptExecutor)browser).ExecuteScript(JS_Builder);
            JS_Builder += "arguments[0].removeAttribute('disabled');";
            //JS_Builder = "document.getElementById('login1').removeAttribute('class','readonly'); ";
            JS_Builder += "arguments[0].innerHTML = 'admin';";
            
            //JS_Builder += "document.getElementsByName('password').arguments[0].removeAttribute('disabled');";
            ((IJavaScriptExecutor)browser).ExecuteScript(JS_Builder,userId);
            JS_Builder = "document.getElementById('login1').removeAttribute('class','readonly'); ";
            JS_Builder += "arguments[0].removeAttribute('type','password');";
            JS_Builder += "arguments[0].removeAttribute('disabled');";
            JS_Builder += "arguments[0].innerHTML = 'amatech1';";
            
            ((IJavaScriptExecutor)browser).ExecuteScript(JS_Builder, pwd);
            //userId.SendKeys("admin");// well it's not sendign username and pwd for 1...waduhek fam //readonly property via headless?
            //pwd.SendKeys("amatech1");
            login.Click();
            
            Console.WriteLine(browser.PageSource);
            Console.WriteLine("UserID AND PWD SENT: " + userId.Text + " " + pwd.Text);
            Console.ReadKey();
            Thread.Sleep(100);
            //Cannont locate this element by ID?...might not be logged in lawl, should check page source from chromedriver <- thar she blows sonnnn
            var ePMPRssi = browser.FindElementById("dl_rssi").GetAttribute("title");
            
            var ePMPSNR = browser.FindElementByClassName("dl_snr").GetAttribute("title");
            //var ePMP_EthernetStatus = browser.FindElementsById("alert-success").GetAttribute("title");
            var ePMPUptime = browser.FindElementById("sys_uptime").GetAttribute("title");
            var ePMP_DlMod = browser.FindElementById("dl_mcs_mode").GetAttribute("title");
            var ePMP_ULMod = browser.FindElementById("ul_mcs_mode").GetAttribute("title");

            // reboot req handling popup
            // ref stackoverflow: https://stackoverflow.com/questions/12744576/selenium-c-sharp-accept-confirm-box
            string pageSrc = browser.PageSource;
            //Console.WriteLine(pageSrc);
            //Console.ReadKey();
            //find & click reboot button, I hope?
            browser.FindElementById("reboot_device").Click(); 
            //((IJavaScriptExecutor)browser).ExecuteScript("arguments[0].click();", browser.FindElementById("reboot_device"));
            

            string JSAlertError = null;
            string RadioStats = null;
            //Handle pop up asking us if we're sure we want to reboot, yes we are. 
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

            //Title's in ePMP radio are "preformated", will concantenate strings together and display. 
            RadioStats = $"{ePMPUptime.ToString()}\n";
            RadioStats += $"{ePMPRssi.ToString()}\n";
            RadioStats += $"{ePMPSNR.ToString()}\n";
            //RadioStats += $"{ePMP_EthernetStatus.ToString()}\n";
            RadioStats += $"{ePMP_DlMod.ToString()}\n";
            RadioStats += $"{ePMP_ULMod.ToString()}\n";

            Console.WriteLine(RadioStats);
            Console.WriteLine("End...");
            Console.ReadKey();
        }
    
    }
}

