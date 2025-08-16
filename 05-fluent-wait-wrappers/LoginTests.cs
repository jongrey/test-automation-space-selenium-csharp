namespace FluentWaitWrappers
{
    public class LoginTests : BaseTest
    {
        [Test]
        public void SuccessfulLoginTest()
        {
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            var productsPage = loginPage.LoginSuccessfully("standard_user", "secret_sauce");

            Assert.IsTrue(productsPage.IsOnProductsPage(), "Should be on products page after successful login");
        }
    }
}