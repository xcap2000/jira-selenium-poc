﻿using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JSP
{
    public static class Program
    {
        public static void Main()
        {
            // const string path = "BUS Mapping FI-01-00.xlsx";
            // const string path = "TUS Linkage File - 5.1.23.xlsx";
            const string path = "Copy of TUS Linkage File - 8.21.xlsx";
            // const string path = "/home/carlos/Downloads/Import Jira/BUS Mapping INB-07-00.xlsx";
            // const string path = "/home/carlos/Downloads/Import Jira/BUS Mapping OUTB-06-00.xlsx";
            // const string path = "/home/carlos/Downloads/Import Jira/BUS Mapping OUTB-06-03.xlsx";
            // const string path = "/home/carlos/Downloads/RE__BPS_to_BUS_Mass_Upload/BUS Mapping COMM-04-00.xlsx";
            // const string path = "/home/carlos/Downloads/RE__BPS_to_BUS_Mass_Upload/BUS Mapping E2E-28-00.xlsx";

            using var file = new FileStream(path, FileMode.Open, FileAccess.Read);

            using var excelPackage = new ExcelPackage(file);

            var worksheet = excelPackage.Workbook.Worksheets[0];

            var options = new ChromeOptions();
            // options.AddArgument("--headless");
            // options.LeaveBrowserRunning = true;

            // Create profile manually, login in the first time, after that it'll log in automatically.
            // options.AddArgument("--user-data-dir=/home/carlos/.config/google-chrome/");
            options.AddArgument("--user-data-dir=C:\\Users\\cportes\\AppData\\Local\\Google\\Chrome\\User Data");
            options.AddArgument("--profile-directory=Profile 1");

            using IWebDriver driver = new ChromeDriver(options);

            string? previousBps = null;

            // for (int row = 2; row <= 1_000_000; row++)
            for (int row = 368; row <= 1_000_000; row++)
            {
                var currentBus = worksheet.Cells["B" + row].Text;

                var currentBps = worksheet.Cells["C" + row].Text;

                if (string.IsNullOrWhiteSpace(currentBus) && string.IsNullOrWhiteSpace(currentBps))
                {
                    // Save
                    // var saveElement = driver.FindElement(By.PartialLinkText("Cancel"));
                    var saveElement = driver.FindElement(By.Name("Link"));
                    saveElement.Click();

                    break;
                }

                if (currentBps != previousBps)
                {
                    if (previousBps != null)
                    {
                        // Save
                        // var saveElement = driver.FindElement(By.PartialLinkText("Cancel"));
                        var saveElement = driver.FindElement(By.Name("Link"));
                        saveElement.Click();
                    }

                    // Go to the next BPS
                    driver.Navigate().GoToUrl($"https://jira.jnj.com/browse/{currentBps}");

                    IWebElement? moreElement = null;

                    while ((moreElement = driver.FindElements(By.Id("opsbar-operations_more")).SingleOrDefault()) == null)
                    {
                        // Wait for the user to login (if needed) and be redirected to the correct link,
                        // in other words make sure the element id is available.
                        Thread.Sleep(1000);
                    }

                    moreElement.Click();

                    Thread.Sleep(1000);

                    IWebElement? linkIssueElement = null;

                    while ((linkIssueElement = driver.FindElements(By.Id("link-issue")).SingleOrDefault()) == null)
                    {
                        // Wait for the user to login (if needed) and be redirected to the correct link,
                        // in other words make sure the element id is available.
                        Thread.Sleep(2000);
                    }

                    linkIssueElement.Click();

                    Thread.Sleep(1000);

                    IWebElement? selectElement = null;

                    //var selectElement = driver.FindElement(By.Id("link-type"));

                    while ((selectElement = driver.FindElements(By.Id("link-type")).SingleOrDefault()) == null)
                    {
                        // Wait for the user to login (if needed) and be redirected to the correct link,
                        // in other words make sure the element id is available.
                        Thread.Sleep(1000);
                    }

                    var selectObject = new SelectElement(selectElement);
                    selectObject.SelectByValue("is child task of");
                }

                var element = driver.FindElement(By.Id("jira-issue-keys-textarea"));
                element.SendKeys(currentBus);
                element.SendKeys(Keys.Tab);

                previousBps = currentBps;
            }

            Thread.Sleep(30000);
        }
    }
}