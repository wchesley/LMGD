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

            browser.Navigate().GoToUrl("http://172.16.98.161");
            string scrapedData = "Nothing found";
            var userId = browser.FindElementById("CanopyUsername");
            var Radiopassword = browser.FindElementById("CanopyPassword");
            var login = browser.FindElementById("loginbutton");
            userId.SendKeys("admin");
            Radiopassword.SendKeys("amatech1");
            login.Submit();
            string upTime = browser.FindElementById("UpTime").Text;
            string RSSI = browser.FindElementById("PowerLevelOFDM").Text;
            string SNR = browser.FindElementById("SignalToNoiseRatioSM").Text;

            // go to reboot radio
            // xpath: //*[@id="menu"]/a[2]
            Thread.Sleep(100);
            
            //browser.FindElementByXPath("*[@id='menu']/a[2]").Click();
            browser.FindElementsByClassName("menu")[2].Click();   
            Thread.Sleep(100);

            var wtf = browser.PageSource;
            Console.WriteLine(wtf.ToString());


            //"reboot" button doesn't appear headlessly, will build manullly via JS and inject into webpage. 
            // 
            //browser.FindElementByName("reboot");
            string JavaScriptInjection = "var d=document, a=d.createElement("; 
            //build string of Radio info prior to reboot. 
            scrapedData = $"Uptime: {upTime}\n";
            scrapedData += $"RSSI: {RSSI}\n";
            scrapedData += $"SNR: {SNR}";

            Console.WriteLine(scrapedData);

            Console.WriteLine("End...");
            Console.ReadKey();
        }
    
    }
}

