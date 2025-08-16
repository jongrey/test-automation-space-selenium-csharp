using OpenQA.Selenium;

namespace FrameworkStructure
{
    // Notice the ": BasePage" inheritance
    public class LoginPage : BasePage
    {
        // Private fields are gone! It's inherited from BasePage.

        // The constructor now calls the base constructor to pass the driver up
        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        // Locators and methods remain the same...
        private readonly By _usernameLocator = By.Id("user-name");
        private readonly By _passwordLocator = By.Id("password");
        private readonly By _loginButtonLocator = By.Id("login-button");
        private readonly By _errorMessageLocator = By.CssSelector("[data-test='error']");

        public IWebElement UsernameInput => wait.Until(d => d.FindElement(_usernameLocator));
        public IWebElement PasswordInput => driver.FindElement(_passwordLocator);
        public IWebElement LoginButton => driver.FindElement(_loginButtonLocator);
        public IWebElement ErrorMessage => wait.Until(d => d.FindElement(_errorMessageLocator));

        public void NavigateTo()
        {
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            wait.Until(d => d.FindElement(_usernameLocator));
        }

        public ProductsPage LoginSuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();

            wait.Until(driver => driver.FindElement(By.Id("inventory_container")));
            return new ProductsPage(driver);
        }

        public LoginPage LoginUnsuccessfully(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();


            wait.Until(driver => driver.FindElement(_errorMessageLocator));
            return this;
        }

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
            try
            {
                return driver.Url.Contains("saucedemo.com")
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