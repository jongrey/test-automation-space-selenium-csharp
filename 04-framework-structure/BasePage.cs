using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FrameworkStructure
{
    public class BasePage
    {
        // Use "protected" to allow child classes to access the driver and wait objects
        protected readonly IWebDriver driver;
        protected readonly WebDriverWait wait;

        // The constructor that all child Page Objects will call
        public BasePage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(10));
        }
    }
}
