using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BasicScript
{
    public class LoginTests
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            // This single line initializes the Chrome browser.
            // Selenium Manager runs in the background to handle the driver.
            _driver = new ChromeDriver();
        }

        [Test]
        public void SuccessfulLoginTest()
        {
            // 1. Navigate to the website
            _driver.Navigate().GoToUrl("https://www.saucedemo.com/");

            // 2. Find the username element by its ID and type in the username
            IWebElement usernameInput = _driver.FindElement(By.Id("user-name"));
            usernameInput.SendKeys("standard_user");

            // 3. Find the password element by its ID and type in the password
            IWebElement passwordInput = _driver.FindElement(By.Id("password"));
            passwordInput.SendKeys("secret_sauce");

            // 4. Find the login button by its ID and click it
            IWebElement loginButton = _driver.FindElement(By.Id("login-button"));
            loginButton.Click();

            // 5. Assert that the login was successful by finding a known element on the next page
            IWebElement inventoryContainer = _driver.FindElement(By.Id("inventory_container"));
            Assert.IsTrue(inventoryContainer.Displayed, "Login was not successful, inventory page not found.");
        }

        [TearDown]
        public void TearDown()
        {
            // This is crucial to close the browser and end the session cleanly.
            _driver.Quit();
        }
    }
}