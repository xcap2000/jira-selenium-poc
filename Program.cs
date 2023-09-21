using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
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
            // const string path = "Copy of TUS Linkage File - 8.21.xlsx";
            const string path = "Copy of TUS Linkage File - 8.21-2.xlsx";
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

            for (int row = 2; row <= 1_000_000; row++)
            {
                var currentBus = worksheet.Cells["B" + row].Text;

                var currentBps = worksheet.Cells["C" + row].Text;

                if (string.IsNullOrWhiteSpace(currentBus) && string.IsNullOrWhiteSpace(currentBps))
                {
                    // Save
                    var saveElement = driver.FindElementSafely(By.Name("Link"));
                    saveElement.Click();

                    break;
                }

                if (currentBps != previousBps)
                {
                    if (previousBps != null)
                    {
                        // Save
                        var saveElement = driver.FindElementSafely(By.Name("Link"));
                        saveElement.Click();
                    }

                    Thread.Sleep(1000);

                    // Go to the next BPS
                    driver.Navigate().GoToUrl($"https://jira.jnj.com/browse/{currentBps}");

                    var moreElement = driver.FindElementsSafely(By.Id("opsbar-operations_more"));

                    // while ((moreElement = driver.FindElements(By.Id("opsbar-operations_more")).SingleOrDefault()) == null)
                    // {
                    //     // Wait for the user to login (if needed) and be redirected to the correct link,
                    //     // in other words make sure the element id is available.
                    //     Thread.Sleep(1000);
                    // }

                    moreElement.Click();

                    Thread.Sleep(1000);

                    var linkIssueElement = driver.FindElementsSafely(By.Id("link-issue"));

                    // while ((linkIssueElement = driver.FindElements(By.Id("link-issue")).SingleOrDefault()) == null)
                    // {
                    //     // Wait for the user to login (if needed) and be redirected to the correct link,
                    //     // in other words make sure the element id is available.
                    //     Thread.Sleep(2000);
                    // }

                    linkIssueElement.Click();

                    Thread.Sleep(1000);

                    var selectElement = driver.FindElementsSafely(By.Id("link-type"));

                    //var selectElement = driver.FindElement(By.Id("link-type"));

                    // while ((selectElement = driver.FindElements(By.Id("link-type")).SingleOrDefault()) == null)
                    // {
                    //     // Wait for the user to login (if needed) and be redirected to the correct link,
                    //     // in other words make sure the element id is available.
                    //     Thread.Sleep(1000);
                    // }

                    var selectObject = new SelectElement(selectElement);
                    selectObject.SelectByValue("is child task of");
                }

                // TODO - If you find the following div then you have to redo the process below and increase the sleep time
                // for each iteration
                // TODO - div class error text: You must specify a Jira issue to link to.

                IWebElement? errorMessageDiv = null;

                var count = 1;

                do
                {
                    var element = driver.FindElementSafely(By.Id("jira-issue-keys-textarea"));
                    element.SendKeys(currentBus);
                    if (count > 1)
                    {
                        Debugger.Break();
                    }
                    Thread.Sleep(count++ * 1000);
                    element.SendKeys(Keys.Tab);

                    errorMessageDiv = driver.FindElements(By.ClassName("error")).FirstOrDefault(e => e.Text == "You must specify a Jira issue to link to.");
                }
                while (errorMessageDiv != null);

                previousBps = currentBps;
            }

            Thread.Sleep(30000);
        }

        public static IWebElement FindElementSafely(this IWebDriver driver, By by)
        {
            IWebElement? element = null;

            while ((element = driver.FindElement(by)) == null)
            {
                // Wait for the user to login (if needed) and be redirected to the correct link,
                // in other words make sure the element id is available.
                Thread.Sleep(1000);
            }

            return element;
        }

        public static IWebElement FindElementsSafely(this IWebDriver driver, By by)
        {
            IWebElement? element = null;

            while ((element = driver.FindElements(by).SingleOrDefault()) == null)
            {
                // Wait for the user to login (if needed) and be redirected to the correct link,
                // in other words make sure the element id is available.
                Thread.Sleep(1000);
            }

            return element;
        }
    }
}