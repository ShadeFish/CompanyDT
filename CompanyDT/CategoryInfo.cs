using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SimpleArgLib;

namespace CompanyDT
{
    class CategoryInfo
    {
        public string adress;
        public string category;
        public int CompaniesCount;
        public int PageCount;
        public HtmlDocument document;

        public CategoryInfo(string adress,string category) 
        {
            this.adress = adress;
            this.category = category;

            /* DOWNLOAD PAGE */
            HtmlWeb web = new HtmlWeb();
            document = web.Load(adress);

            /* COMPANIES COUNT */
            HtmlNode node = document.DocumentNode.SelectSingleNode("//*[@id=\"listing-search-filters\"]/div[1]/h1/text()[2]");
            CompaniesCount = Convert.ToInt32(node.InnerText.Trim().Split(' ')[1]);

            /* PAGE COUNT */
            PageCount = Convert.ToInt32(document.DocumentNode.SelectSingleNode("//*[@id=\"company-list-paginator\"]/nav/ul/li[7]/a").Attributes["data-paginatorpage"].Value);
        }
    }
}
