using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PageObjects
{
    public class ProductsPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ProductsPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        // Locators for products page elements
        private readonly By _inventoryContainerLocator = By.Id("inventory_container");
        private readonly By _productItemsLocator = By.CssSelector("[data-test='inventory-item']");

        // Properties for page elements with explicit waits
        public IWebElement InventoryContainer => _wait.Until(d => d.FindElement(_inventoryContainerLocator));
        public IList<IWebElement> ProductItems => _driver.FindElements(_productItemsLocator);

        // Validation methods
        public bool IsOnProductsPage()
        {
            try
            {
                return _driver.Url.Contains("inventory.html")
                    && InventoryContainer.Displayed
                    && ProductItems.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetProductCount()
        {
            return ProductItems.Count;
        }
    }
}