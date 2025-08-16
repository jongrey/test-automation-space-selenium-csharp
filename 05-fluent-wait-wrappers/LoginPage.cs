using OpenQA.Selenium;

namespace FluentWaitWrappers
{
    public class LoginPage : BasePage
    {
        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        // Locators remain the same
        private readonly By _usernameLocator = By.Id("user-name");
        private readonly By _passwordLocator = By.Id("password");
        private readonly By _loginButtonLocator = By.Id("login-button");
        private readonly By _errorMessageLocator = By.CssSelector("[data-test='error']");
        private readonly By _inventoryContainerLocator = By.Id("inventory_container");

        // Properties now use appropriate wait strategies
        public IWebElement UsernameInput => WaitForElementToBeVisible(_usernameLocator);
        public IWebElement PasswordInput => WaitForElementToBeVisible(_passwordLocator);
        public IWebElement LoginButton => WaitForElementToBeClickable(_loginButtonLocator);

        // Navigation method with clear intent
        public void NavigateTo()
        {
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            // Wait for the page to load by checking for username field
            WaitForElementToBeVisible(_usernameLocator);
        }

        // Successful login with appropriate waits
        public ProductsPage LoginSuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();

            // Wait for navigation to complete by checking for products page element
            WaitForElementToBePresent(_inventoryContainerLocator);
            return new ProductsPage(driver);
        }

        // Unsuccessful login with error handling
        public LoginPage LoginUnsuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();

            // Wait specifically for error message to appear
            WaitForElementToBeVisible(_errorMessageLocator);
            return this;
        }

        // Verification methods with improved reliability
        public bool IsErrorMessageDisplayed()
        {
            try
            {
                return WaitForElementToBeVisible(_errorMessageLocator, 2).Displayed;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public string GetErrorMessageText()
        {
            return WaitForElementToBeVisible(_errorMessageLocator).Text;
        }

        public bool IsOnLoginPage()
        {
            try
            {
                return driver.Url.Contains("saucedemo.com")
                    && WaitForElementToBePresent(_usernameLocator, 2) != null
                    && WaitForElementToBePresent(_passwordLocator, 2) != null
                    && WaitForElementToBePresent(_loginButtonLocator, 2) != null;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}