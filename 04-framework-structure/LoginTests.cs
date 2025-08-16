namespace FrameworkStructure
{
    // Notice the ": BaseTest" inheritance
    public class LoginTests : BaseTest
    {
        // The _driver field, [SetUp], and [TearDown] methods are gone!
        // They are automatically inherited from BaseTest.

        [Test]
        public void SuccessfulLoginTest()
        {
            // We can still access _driver because it's "protected" in the base class
            var loginPage = new LoginPage(_driver);
            loginPage.NavigateTo();

            var productsPage = loginPage.LoginSuccessfully("standard_user", "secret_sauce");

            Assert.IsTrue(productsPage.IsOnProductsPage(), "Should be on products page after successful login");
        }
    }
}