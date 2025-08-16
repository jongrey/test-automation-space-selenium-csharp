using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace RobustInteractions
{
    public class LoginTests
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            // Initialize the wait with a 10-second timeout.
            // This can be configured based on application needs.
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void SuccessfulLoginTest()
        {
            _driver.Navigate().GoToUrl("https://www.saucedemo.com/");

            // Wait for the username input to be visible before interacting
            IWebElement usernameInput = _wait.Until(d => d.FindElement(By.Id("user-name")));
            usernameInput.SendKeys("standard_user");

            // No wait needed here as the password field loads with the username field
            IWebElement passwordInput = _driver.FindElement(By.Id("password"));
            passwordInput.SendKeys("secret_sauce");

            IWebElement loginButton = _driver.FindElement(By.Id("login-button"));
            loginButton.Click();

            // After login, wait for the inventory container to be visible on the next page
            IWebElement inventoryContainer = _wait.Until(d => d.FindElement(By.Id("inventory_container")));
            Assert.IsTrue(inventoryContainer.Displayed, "Login was not successful, inventory page not found.");
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}
