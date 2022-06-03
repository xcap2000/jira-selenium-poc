using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JSP
{
    public static class Program
    {
        public static void Main()
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless");
            // options.LeaveBrowserRunning = true;
            // options.AddArgument("--user-data-dir=/home/carlos/.config/google-chrome/Default/Person 1");
            // options.AddArgument("--user-data-dir=/home/carlos/.config/google-chrome/Default");
            // options.AddArgument("--profile-directory=Person 1");

            options.AddArgument("--user-data-dir=/home/carlos/.config/google-chrome/");
            options.AddArgument("--profile-directory=Default");

            using IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("https://jira.jnj.com/browse/AHFY-34224");

            // driver.FindElement(By.Id("username")).SendKeys("cportes");
            // driver.FindElement(By.Id("password")).SendKeys("J3#4$5%7&8*9(");
            // driver.FindElement(By.Id("signOnButton")).Click();

            Thread.Sleep(10000);

            var buttonText = driver.FindElements(By.ClassName("button")).First().Text;
            System.Console.WriteLine(buttonText);

            Thread.Sleep(10000);

            // options.addArguments("--user-data-dir=C:\\Users\\user\\AppData\\Local\\Google\\Chrome\\User Data");
            // options.addArguments("--profile-directory=Profile 2");

            //driver.FindElement
            Thread.Sleep(20000);

            //username
            //password
            //signOnButton
            //class button Change Device
        }
    }
}