using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
// using OpenQA.Selenium.Support.UI;

namespace JSP
{
    public static class Program
    {
        public static void Main()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless");
            // options.LeaveBrowserRunning = true;

            // Create profile manually, login in the first time, after that it'll log in automatically.
            options.AddArgument("--user-data-dir=/home/carlos/.config/google-chrome/");
            options.AddArgument("--profile-directory=Profile 1");

            using IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("https://jira.jnj.com/browse/AHFY-34224");

            driver.FindElement(By.Id("add-links-link")).Click();

            Thread.Sleep(3000);

            var selectElement = driver.FindElement(By.Id("link-type"));

            var selectObject = new SelectElement(selectElement);
            selectObject.SelectByValue("is parent task of");

            var element = driver.FindElement(By.Id("jira-issue-keys-textarea"));
            element.SendKeys("AHFY-30319");
            element.SendKeys(Keys.Tab);

            Thread.Sleep(10000);
        }
    }
}