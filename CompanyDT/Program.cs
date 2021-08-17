using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Drawing;
using Console = Colorful.Console;
using SimpleArgLib;
using System.Threading;
using System.IO;
using System.Data.SQLite;

namespace CompanyDT
{
    class Program
    {
        static int companyCount = 0;
        static void Main(string[] args)
        {
            Console.WriteAscii("CompanyDT",Color.DarkSeaGreen);
            Console.WriteLine();

            Console.Write("Select Category: ");
            string category = Console.ReadLine();
            
            Console.Write("Use ChromeWebDriver? n/y: ");
            bool useChromeDriver = (Console.ReadLine() == "y") ? true : false;

            Console.Write("Export to SQL DATABASE file? y/n: ");
            bool exportToFile = (Console.ReadLine() == "y") ? true : false;
            Console.Clear();

            Console.Write("Downloading Category Info.. ",Color.Yellow);
            CategoryInfo categoryInfo = new CategoryInfo(CreateAdress(category,0),category);
            Console.Clear();

            Console.WriteLine("Found Companies ("+ categoryInfo.CompaniesCount + ") in ("+categoryInfo.PageCount+") Pages.",Color.Yellow);

            string dbFileName = category + ".db";
            if (exportToFile)
            {
                if (File.Exists(dbFileName)) 
                { 
                    File.Delete(dbFileName); SQLiteConnection.CreateFile(dbFileName);

                    SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                    connection.Open();

                    string sql = "CREATE TABLE companies (" +
                        "name varchar(64), " +
                        "page varchar(64)," +
                        "category varchar(64)," +
                        "adress varchar(64)," +
                        "mail varchar(64)," +
                        "phone varchar(64))";

                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
            }

            for (int i =0;i < categoryInfo.PageCount ;i++)
            {
                CategoryInfo info = new CategoryInfo(CreateAdress(category, i), category);
                Console.WriteLine("\nPreparing Page (" + (i + 1) + ")", Color.Yellow);
                Console.WriteLine("Stored company data ("+companyCount+")", Color.Yellow);
                
                CompanyListPage companyListPage = new CompanyListPage(info.adress);
                companyListPage.NewCompany += CompanyListPage_NewCompany;

                Company[] companies = companyListPage.GetCompanyList(useChromeDriver);

                if (exportToFile)
                {
                    SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                    connection.Open();

                    foreach (Company company in companies)
                    {
                        string sql = "INSERT INTO companies (name,page,category,adress,mail,phone) VALUES" +
                            "('"+company.name+"','"+company.page+"','"+company.category+"','"+company.adress+"','"+company.mail+"','"+company.phoneNumber+"')";

                        using (SQLiteCommand cmd = new SQLiteCommand(sql, connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
            }
        }

        private static void CompanyListPage_NewCompany(Company company)
        {
            Console.WriteLine(company.ToString(),Color.Tomato);
            companyCount++;
        }

        static string CreateAdress(string category,int page)
        {
            if(page == 0)
            {
                return "https://panoramafirm.pl/" + category.Replace(' ', '_') + "/";
            }
            else
            {
                return "https://panoramafirm.pl/" + category.Replace(' ', '_') + "/firmy," + page.ToString() + ".html";
            }
        }
    }
}
