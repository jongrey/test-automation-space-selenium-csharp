using OpenQA.Selenium;

namespace FluentWaitWrappers
{
    public class ProductsPage : BasePage
    {
        public ProductsPage(IWebDriver driver) : base(driver)
        {
        }

        private readonly By _inventoryContainerLocator = By.Id("inventory_container");
        private readonly By _productItemsLocator = By.CssSelector("[data-test='inventory-item']");

        public bool IsOnProductsPage()
        {
            try
            {
                return driver.Url.Contains("inventory.html")
                    && WaitForElementToBeVisible(_inventoryContainerLocator, 3) != null
                    && WaitForElementsToBePresent(_productItemsLocator, 3).Count > 0;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public int GetProductCount()
        {
            return WaitForElementsToBePresent(_productItemsLocator).Count;
        }
    }
}