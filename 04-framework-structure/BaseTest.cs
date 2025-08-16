using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FrameworkStructure
{
    public class BaseTest
    {
        // Change to protected so child classes can access it
        protected IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            // All setup logic is now centralized here
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            // All teardown logic is centralized here
            _driver?.Quit();
        }
    }
}
