using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Console = Colorful.Console;
using System.Drawing;


namespace CompanyDT
{
    class Company
    {
        public string name;
        public string page;
        public string category;
        public string adress;
        public string mail;
        public string phoneNumber;

        public Company() { }

        public Company(string name, string category, string adress, string mail,string page, string phoneNumber)
        {
            this.name = name;
            this.category = category;
            this.adress = adress;
            this.mail = mail;
            this.phoneNumber = phoneNumber;
            this.page = page;
        }

        public static Company FromPage(string adress,bool useChromeDriver)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(adress);
            Company company = new Company();

            if (useChromeDriver)
            {
                HtmlNode[] buttons = ParseButtons(doc, "//*[@id=\"to-pdf\"]/div[6]/div/div[3]/div[2]/div/div[1]");

                if(buttons != null)
                {
                    company.phoneNumber = GetPhoneNumber(adress, "#to-pdf > div.container-fluid.px-0.pb-2.bg-white.border-bottom > div > div.row.pt-2 > div.col-lg-4.col-sm-12.py-3.py-lg-0.contact-buttons > div > div.row > div:nth-child(" + (buttons.Length - 1) + ") > button");
                }
            }

            try
            {
                HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id=\"to-pdf\"]/div[6]/div/div[2]/div/h2");
                company.category = GetSingleNodeInner(doc, "//*[@id=\"to-pdf\"]/div[6]/div/div[2]/div/div/div");
            }
            catch
            {
                company.category = GetSingleNodeInner(doc, "//*[@id=\"to-pdf\"]/div[6]/div/div[1]/div/div/div");
            }

            company.name = GetSingleNodeInner(doc, "//*[@id=\"to-pdf\"]/div[6]/div/div[2]/div/h1");
            company.adress = GetSingleNodeInner(doc, "//*[@id=\"to-pdf\"]/div[6]/div/div[3]/div[1]/div/div/div[1]/div/div/div[2]");
            string mail = GetSingleNodeInner(doc, "//*[@id=\"contact\"]/div[3]/div/div[3]/div[2]/a");
            company.mail = IsValidEmail(mail) ? mail : string.Empty;
            company.page = GetSingleNodeInner(doc, "//*[@id=\"contact\"]/div[3]/div/div[1]/div[2]/div/a");

            return (company.name != string.Empty) ? company : null;
        }

        /* PARSE INFO BUTTONS */
        private static HtmlNode[] ParseButtons(HtmlDocument doc,string xPath)
        {
            try
            {
                HtmlNodeCollection buttons = doc.DocumentNode.SelectSingleNode("//*[@id=\"to-pdf\"]/div[6]/div/div[3]/div[2]/div/div[1]").ChildNodes;
                List<HtmlNode> newButtons = new List<HtmlNode>();
                foreach (HtmlNode node in buttons)
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText)) { newButtons.Add(node); }
                }
                return newButtons.ToArray();
            }
            catch
            {
                return null;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static string GetSingleNodeInner(HtmlDocument doc,string XPath)
        {
            try
            {
                return doc.DocumentNode.SelectSingleNode(XPath).InnerText.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetPhoneNumber(string adress,string cssSelector)
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");

            IWebDriver driver = new ChromeDriver(service, options);
            driver.Navigate().GoToUrl(adress);
            IWebElement phonebutton = null;

            while (phonebutton == null)
            {
                try
                {
                    phonebutton = driver.FindElement(By.CssSelector(cssSelector));
                }
                catch { }
            }

            IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
            executor.ExecuteScript("arguments[0].click();", phonebutton);

            string number = string.Empty;
            while (number == string.Empty)
            {
                try
                {
                    number = driver.FindElement(By.CssSelector("#phone-modal > div > div > div.modal-header.text-dark > div > div.col-xs-12.modal-title.w-100.font-weight-bold.text-dark")).Text;
                }
                catch { }
            }
            driver.Close();
            return number;
        }

        public override string ToString()
        {
            return "Name : " + name + "\n" +
                   "Category : " + category + "\n" +
                   "Adress : " + adress + "\n" +
                   "Mail : " + mail + "\n" +
                   "Page : " + page + "\n" +
                   "Phone Number : " + phoneNumber + "\n";
        }
    }
}
