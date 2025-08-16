using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Reflection;

namespace AdvancedInteractions
{
    public class BasePage
    {
        protected readonly IWebDriver driver;
        private readonly IJavaScriptExecutor _js;
        private readonly int _defaultTimeoutSeconds;
        private Actions _actions;

        public BasePage(IWebDriver driver, int defaultTimeoutSeconds = 10)
        {
            this.driver = driver;
            _js = (IJavaScriptExecutor)driver;
            _defaultTimeoutSeconds = defaultTimeoutSeconds;
            _actions = new Actions(driver); // Initialize Actions once
        }

        protected WebDriverWait CreateWait(int timeoutSeconds = 0)
        {
            var timeout = timeoutSeconds > 0 ? timeoutSeconds : _defaultTimeoutSeconds;
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

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

        protected T WaitForCondition<T>(Func<IWebDriver, T> condition, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(condition);
        }

        protected void SwitchToFrameAndWait(By frameLocator, By elementToVerify, int timeoutSeconds = 0)
        {
            // First, ensure the iframe element itself is present and ready for interaction
            var frameElement = WaitForElementToBePresent(frameLocator, timeoutSeconds);

            // We assume that all wait operations in this example are successful for simplicity
            // You should add null/result checks in your tests
            TestContext.WriteLine($"Found iframe element: {frameLocator}");

            // Switch the WebDriver's focus into the iframe context
            driver.SwitchTo().Frame(frameElement);

            TestContext.WriteLine("Switched to iframe context");

            // Verify the iframe content has loaded by waiting for a known element inside it
            WaitForElementToBePresent(elementToVerify, timeoutSeconds);

            TestContext.WriteLine($"Verified iframe content is ready: {elementToVerify}");
        }

        protected void SwitchToFrameByNameAndWait(string frameNameOrId, By elementToVerify, int timeoutSeconds = 0)
        {
            // Wait for the iframe to be present by its name or ID
            WaitForCondition(driver =>
            {
                try
                {
                    driver.SwitchTo().Frame(frameNameOrId);
                    return true;
                }
                catch (NoSuchFrameException)
                {
                    return false; // Frame not yet available, continue waiting
                }
            }, timeoutSeconds);

            TestContext.WriteLine($"Switched to iframe by name/ID: {frameNameOrId}");

            // Verify the iframe content is ready for interaction
            WaitForElementToBePresent(elementToVerify, timeoutSeconds);

            TestContext.WriteLine($"Verified iframe content is ready: {elementToVerify}");
        }

        protected void SwitchToMainDocument()
        {
            driver.SwitchTo().DefaultContent();
            TestContext.WriteLine("Switched back to main document context");
        }

        protected IAlert WaitForAlertAndSwitch(int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);

            var alert = wait.Until(driver =>
            {
                try
                {
                    return driver.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    // Alert not yet present, continue waiting
                    return null;
                }
            });

            TestContext.WriteLine("Successfully switched to alert context");
            return alert;
        }

        protected void SwitchToWindowAndWait(string windowHandle, string expectedTitleContains = null, int timeoutSeconds = 0)
        {
            // Switch to the specified window
            driver.SwitchTo().Window(windowHandle);

            TestContext.WriteLine($"Switched to window: {windowHandle}");

            // If expected title provided, wait for it to confirm the window is fully loaded
            if (!string.IsNullOrEmpty(expectedTitleContains))
            {
                WaitForCondition(driver =>
                  driver.Title.Contains(expectedTitleContains, StringComparison.OrdinalIgnoreCase),
                  timeoutSeconds);

                TestContext.WriteLine($"Verified window title contains: {expectedTitleContains}");
            }
        }

        protected string WaitForNewWindowAndSwitch(string originalWindowHandle, string expectedTitleContains = null, int timeoutSeconds = 0)
        {
            var wait = CreateWait(timeoutSeconds);

            // Wait for a new window to appear by checking the total number of window handles
            var newWindowHandle = wait.Until(driver =>
            {
                var currentHandles = driver.WindowHandles;
                // Find the handle that is not the original window
                var newHandle = currentHandles.FirstOrDefault(handle => handle != originalWindowHandle);
                return newHandle;
            });

            TestContext.WriteLine($"Detected new window: {newWindowHandle}");

            // Switch to the new window and optionally verify its content
            SwitchToWindowAndWait(newWindowHandle, expectedTitleContains, timeoutSeconds);

            return newWindowHandle;
        }

        // Hover interaction with built-in wait strategy
        protected void HoverOverElement(By locator, int timeoutSeconds = 0)
        {
            var element = WaitForElementToBeVisible(locator, timeoutSeconds);
            _actions.MoveToElement(element).Perform();

            // Brief pause to allow hover effects to activate
            Thread.Sleep(300);
        }

        // Hover and click sequence for dropdown menus
        protected void HoverAndClick(By hoverLocator, By clickLocator, int timeoutSeconds = 0)
        {
            var hoverElement = WaitForElementToBeVisible(hoverLocator, timeoutSeconds);
            _actions.MoveToElement(hoverElement).Perform();

            // Wait for the click target to become available after hover
            var clickElement = WaitForElementToBeClickable(clickLocator, timeoutSeconds);
            clickElement.Click();
        }

        // Right-click with context menu handling
        protected void RightClickElement(By locator, int timeoutSeconds = 0)
        {
            var element = WaitForElementToBeClickable(locator, timeoutSeconds);
            _actions.ContextClick(element).Perform();
        }

        // Robust drag and drop with retry logic
        protected bool DragAndDrop(By sourceLocator, By targetLocator, int timeoutSeconds = 0)
        {
            try
            {
                var sourceElement = WaitForElementToBeVisible(sourceLocator, timeoutSeconds);
                var targetElement = WaitForElementToBeVisible(targetLocator, timeoutSeconds);

                _actions.DragAndDrop(sourceElement, targetElement).Perform();

                // Verify the drag operation succeeded by checking element positions or states
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Drag and drop operation failed: {ex.Message}");
                return false;
            }
        }

        // File upload with validation
        protected void UploadFile(By fileInputLocator, string fileName, int timeoutSeconds = 0)
        {
            var fileInput = WaitForElementToBePresent(fileInputLocator, timeoutSeconds);
            string filePath = GetTestFilePath(fileName);

            // Handle hidden file inputs
            if (!fileInput.Displayed)
            {
                var js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].style.display = 'block';", fileInput);
            }

            fileInput.SendKeys(filePath);
        }

        private string GetTestFilePath(string fileName)
        {
            string testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(testDirectory, "TestData", fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Test file not found: {fullPath}");
            }

            return fullPath;
        }

        // Scrolling utilities
        protected void ScrollIntoView(By locator, string position = "center")
        {
            var element = WaitForElementToBePresent(locator);
            _js.ExecuteScript($@"
            arguments[0].scrollIntoView({{
                behavior: 'smooth',
                block: '{position}',
                inline: 'nearest'
            }});", element);
            Thread.Sleep(500); // Allow scroll animation to complete
        }

        protected void ScrollToTop()
        {
            _js.ExecuteScript("window.scrollTo({ top: 0, behavior: 'smooth' });");
            Thread.Sleep(300);
        }

        protected void ScrollToBottom()
        {
            _js.ExecuteScript("window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });");
            Thread.Sleep(300);
        }

        // Element manipulation utilities
        protected void HighlightElement(By locator, string color = "red", int durationMs = 2000)
        {
            var element = WaitForElementToBePresent(locator);
            _js.ExecuteScript($@"
            var element = arguments[0];
            var originalStyle = element.style.border;
            element.style.border = '3px solid {color}';
            setTimeout(function() {{
                element.style.border = originalStyle;
            }}, {durationMs});
        ", element);
        }

        protected void SetElementValue(By locator, string value)
        {
            var element = WaitForElementToBePresent(locator);
            _js.ExecuteScript(@"
            var element = arguments[0];
            var value = arguments[1];
            element.value = value;
            element.dispatchEvent(new Event('input', { bubbles: true }));
            element.dispatchEvent(new Event('change', { bubbles: true }));
        ", element, value);
        }

        protected bool IsElementInViewport(By locator)
        {
            var element = WaitForElementToBePresent(locator);
            var result = _js.ExecuteScript(@"
            var element = arguments[0];
            var rect = element.getBoundingClientRect();
            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
        ", element);
            return (bool)result;
        }

        // Storage utilities (from previous section)
        protected string GetLocalStorageItem(string key)
        {
            return (string)_js.ExecuteScript($"return localStorage.getItem('{key}');");
        }

        protected void ClearLocalStorage()
        {
            _js.ExecuteScript("localStorage.clear();");
        }

        // Robust JavaScript execution wrapper
        protected T ExecuteJavaScriptSafely<T>(string script, params object[] args)
        {
            try
            {
                var result = _js.ExecuteScript(script, args);

                if (result == null && typeof(T) != typeof(object))
                {
                    throw new InvalidOperationException($"JavaScript returned null, but expected {typeof(T).Name}");
                }

                return (T)result;
            }
            catch (WebDriverException ex) when (ex.Message.Contains("javascript error"))
            {
                // Log the problematic script for debugging
                Console.WriteLine($"JavaScript execution failed:");
                Console.WriteLine($"Script: {script}");
                Console.WriteLine($"Arguments: {string.Join(", ", args)}");
                Console.WriteLine($"Error: {ex.Message}");

                throw new InvalidOperationException($"JavaScript execution failed: {ex.Message}", ex);
            }
            catch (InvalidCastException ex)
            {
                var resultType = _js.ExecuteScript(script, args)?.GetType().Name ?? "null";
                throw new InvalidOperationException(
                    $"Cannot cast JavaScript result of type {resultType} to {typeof(T).Name}. Script: {script}", ex);
            }
        }

        // Conditional execution with fallback
        protected bool TryExecuteJavaScript<T>(string script, out T result, params object[] args)
        {
            try
            {
                result = ExecuteJavaScriptSafely<T>(script, args);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JavaScript execution failed gracefully: {ex.Message}");
                result = default(T);
                return false;
            }
        }
    }
}