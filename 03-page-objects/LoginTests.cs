using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PageObjects
{
    public class LoginTests
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void SuccessfulLoginTest()
        {
            // Arrange: Create an instance of the LoginPage
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            // Verify we're on the correct starting page
            Assert.IsTrue(loginPage.IsOnLoginPage(), "Should start on login page");

            // Act: Use the factory pattern for navigation
            var productsPage = loginPage.LoginSuccessfully("standard_user", "secret_sauce");

            // Assert: Use validation methods from ProductsPage
            Assert.IsTrue(productsPage.IsOnProductsPage(), "Should be on products page after successful login");
            Assert.IsTrue(productsPage.GetProductCount() > 0, "Should display products after login");
        }

        [Test]
        public void UnsuccessfulLoginTest()
        {
            // Arrange
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            // Act: Stay on login page for failed attempt
            loginPage = loginPage.LoginUnsuccessfully("invalid_user", "wrong_password");

            // Assert: Verify error handling
            Assert.IsTrue(loginPage.IsErrorMessageDisplayed(), "Should show error message for invalid login");
            Assert.IsTrue(loginPage.GetErrorMessageText().Contains("Username and password do not match"),
                "Error message should be descriptive");
            Assert.IsTrue(loginPage.IsOnLoginPage(), "Should remain on login page after failed attempt");
        }

        [Test]
        public void LoginWithEmptyCredentialsTest()
        {
            // Arrange
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            // Act: Attempt login with empty credentials
            loginPage = loginPage.LoginUnsuccessfully("", "");

            // Assert: Verify appropriate error message
            Assert.IsTrue(loginPage.IsErrorMessageDisplayed(), "Should show error message for empty credentials");
            Assert.IsTrue(loginPage.GetErrorMessageText().Contains("Username is required"),
                "Should show username required error");
        }

        [Test]
        public void LoginWithLockedUserTest()
        {
            // Arrange
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            // Act: Attempt login with locked user
            loginPage = loginPage.LoginUnsuccessfully("locked_out_user", "secret_sauce");

            // Assert: Verify locked user error message
            Assert.IsTrue(loginPage.IsErrorMessageDisplayed(), "Should show error message for locked user");
            Assert.IsTrue(loginPage.GetErrorMessageText().Contains("locked out"),
                "Should show locked out error message");
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Quit();
        }
    }
}