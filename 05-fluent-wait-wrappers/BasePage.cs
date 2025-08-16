using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FluentWaitWrappers
{
    public class BasePage
    {
        protected readonly IWebDriver driver;
        private readonly int _defaultTimeoutSeconds;

        public BasePage(IWebDriver driver, int defaultTimeoutSeconds = 10)
        {
            this.driver = driver;
            _defaultTimeoutSeconds = defaultTimeoutSeconds;
        }

        // Create a WebDriverWait instance with custom or default timeout
        protected WebDriverWait CreateWait(int timeoutSeconds = 0)
        {
            var timeout = timeoutSeconds > 0 ? timeoutSeconds : _defaultTimeoutSeconds;
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

        // Wait for element to be present in DOM - using our proven pattern
        protected IWebElement WaitForElementToBePresent(By locator, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    // Simply find and return the element if it exists
                    return driver.FindElement(locator);
                }
                catch (NoSuchElementException)
                {
                    // Element not in DOM yet, continue waiting
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    // Element became stale, continue waiting for fresh element
                    return null;
                }
            });
        }

        // Wait for element to be visible - building on our robust pattern
        protected IWebElement WaitForElementToBeVisible(By locator, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    // Return element only if it's both found and displayed
                    return element.Displayed ? element : null;
                }
                catch (NoSuchElementException)
                {
                    // Element not yet in DOM, continue waiting
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    // Element was found but became stale, try again
                    return null;
                }
            });
        }

        // Wait for element to be clickable - our most comprehensive check
        protected IWebElement WaitForElementToBeClickable(By locator, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    // Element must be displayed AND enabled to be truly clickable
                    if (element.Displayed && element.Enabled)
                    {
                        return element;
                    }
                    return null;
                }
                catch (NoSuchElementException)
                {
                    // Element not in DOM yet, continue waiting
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    // Element became stale during check, try again
                    return null;
                }
            });
        }

        // Wait for multiple elements to be present - extends our single element pattern
        protected IList<IWebElement> WaitForElementsToBePresent(By locator, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var elements = driver.FindElements(locator);
                    // Return the list only if we found at least one element
                    return elements.Count > 0 ? elements : null;
                }
                catch (StaleElementReferenceException)
                {
                    // Some elements became stale, try the search again
                    return null;
                }
            });
        }

        // Wait for specific text to appear in an element - applies our text waiting pattern
        protected IWebElement WaitForTextToBePresent(By locator, string expectedText, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    // Use case-insensitive comparison and trim whitespace for robustness
                    if (element.Text.Trim().Contains(expectedText, StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                    return null;
                }
                catch (NoSuchElementException)
                {
                    // Element not found yet, continue waiting
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    // Element became stale while checking text, try again
                    return null;
                }
            });
        }

        // Wait for text to change from a specific value - common in status updates
        protected IWebElement WaitForTextToChange(By locator, string oldText, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    var currentText = element.Text.Trim();
                    // Return element when text is different from the old text
                    if (!string.Equals(currentText, oldText, StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                    return null;
                }
                catch (NoSuchElementException)
                {
                    // Element not found, continue waiting
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    // Element became stale, try again
                    return null;
                }
            });
        }

        // Wait for element to disappear - using our proven disappearance pattern
        protected bool WaitForElementToDisappear(By locator, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    // If element is found and visible, keep waiting
                    return !element.Displayed;
                }
                catch (NoSuchElementException)
                {
                    // Element no longer in DOM - this is what we want
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    // Element became stale (likely removed) - this is also success
                    return true;
                }
            });
        }

        // Generic wait method for completely custom conditions
        protected T WaitForCondition<T>(Func<IWebDriver, T> condition, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(condition);
        }
    }
}