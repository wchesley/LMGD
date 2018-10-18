﻿using System;
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
            string AccountNumber = "";
            string ATAError = "ATA Error occured... ";
            FOPS FOPSPage = new FOPS();
            // attempt to load sensitive info from local xml file. 

            XElement LMGD_Doc = XElement.Load(@"C:\Users\Walker\Documents\LMGD_Data.xml");
            username = LMGD_Doc.Element("username").Value;
            password = LMGD_Doc.Element("password").Value;


            //Console.WriteLine("pulled from xml: " + username);
            //Console.WriteLine("pulled from xml: " + password);
            //Console.ReadKey(); 

            //init chrome browser
            var browser = new BrowserExt().CreateHeadlessBrowser(FOPS);
            //manually loggin in for testing sake. 
            FOPSPage.FOPS_Login(username, password, browser, FOPS + AtaProvisioning);
            //browser.Navigate().GoToUrl(FOPS);
            //I'm getting redirected to login page? session issues?
            //assumes we're logged into FOPS... SHould be if control was transferred from FOPS browser sesh directly...THis should be handled in FOPS browser. 

            //Console.WriteLine(browser.Url);
            Console.WriteLine("Enter Account number: ");
            AccountNumber = Console.ReadLine();
            var ReturnedRadio = FOPSPage.GetRadioIP(browser, AccountNumber);
            var ReturnedATA = FOPSPage.GetAtaIp(browser, AccountNumber);
            browser.Quit();
            Console.WriteLine($"Results: \n{ReturnedRadio}\n{ReturnedATA}");
        }
    
    }
}