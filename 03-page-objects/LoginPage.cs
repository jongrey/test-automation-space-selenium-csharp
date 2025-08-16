using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PageObjects
{
    public class LoginPage
    {
        // 1. Private fields to store WebDriver and WebDriverWait instances
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // 2. Constructor to initialize the WebDriver and WebDriverWait
        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        // 3. Private By locators for all elements on the page
        private readonly By _usernameLocator = By.Id("user-name");
        private readonly By _passwordLocator = By.Id("password");
        private readonly By _loginButtonLocator = By.Id("login-button");
        private readonly By _errorMessageLocator = By.CssSelector("[data-test='error']");

        // 4. Public properties with explicit waits for robust element access
        public IWebElement UsernameInput => _wait.Until(d => d.FindElement(_usernameLocator));
        public IWebElement PasswordInput => _driver.FindElement(_passwordLocator);
        public IWebElement LoginButton => _driver.FindElement(_loginButtonLocator);
        public IWebElement ErrorMessage => _wait.Until(d => d.FindElement(_errorMessageLocator));

        // 5. Navigation method with wait for page load
        public void NavigateTo()
        {
            _driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            // Wait for page to load by ensuring username field is present
            _wait.Until(d => d.FindElement(_usernameLocator));
        }

        // This method returns a ProductsPage, indicating successful navigation
        public ProductsPage LoginSuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();

            // Wait for navigation to complete by checking for inventory container
            _wait.Until(driver => driver.FindElement(By.Id("inventory_container")));

            return new ProductsPage(_driver);
        }

        // This method stays on the same page for failed login attempts
        public LoginPage LoginUnsuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();

            // Wait for error message to appear
            _wait.Until(driver => driver.FindElement(_errorMessageLocator));

            return this; // Return current page object since we stay on login page
        }

        // Validation methods make tests more expressive and reliable
        public bool IsErrorMessageDisplayed()
        {
            try
            {
                return ErrorMessage.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string GetErrorMessageText()
        {
            return ErrorMessage.Text;
        }

        public bool IsOnLoginPage()
        {
            // Check for multiple indicators to ensure we're on the right page
            try
            {
                return _driver.Url.Contains("saucedemo.com")
                    && UsernameInput.Displayed
                    && PasswordInput.Displayed
                    && LoginButton.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}