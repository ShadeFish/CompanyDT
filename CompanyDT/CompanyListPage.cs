using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SimpleArgLib;

namespace CompanyDT
{
    class CompanyListPage
    {
        public HtmlDocument document;

        public delegate void newCompanyFoundDelegate(Company company);
        public event newCompanyFoundDelegate NewCompany;

        public CompanyListPage(string adress)
        {
            HtmlWeb web = new HtmlWeb();
            document = web.Load(adress);
        }

        /* GET COMPANY LIST */
        public Company[] GetCompanyList(bool useChromeDriver)
        {
            HtmlNodeCollection htmlNodes = document.DocumentNode.SelectNodes("//*[@id=\"company-list\"]/li/div/div/h2/a");
            List<Company> companies = new List<Company>();

            if (htmlNodes != null)
            {
                foreach (HtmlNode node in htmlNodes)
                {
                    string url = node.Attributes["href"].Value;
                    if (new Uri(url).Host == "panoramafirm.pl")
                    {
                        Company company = Company.FromPage(url, useChromeDriver);

                        if(company != null)
                        {
                            companies.Add(company);
                            NewCompany(company);
                        }
                    }
                }
            }
            return companies.ToArray();
        }
    }
}
