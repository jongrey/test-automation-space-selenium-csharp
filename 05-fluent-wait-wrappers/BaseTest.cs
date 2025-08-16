using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FluentWaitWrappers
{
    public class BaseTest
    {
        protected IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                // Capture final state before cleanup
                if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    TakeScreenshot();
                }
            }
            finally
            {
                _driver?.Quit();
            }
        }

        protected void TakeScreenshot()
        {
            try
            {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                var testName = TestContext.CurrentContext.Test.Name;
                var fileName = $"{testName}_{timestamp}.png";

                var screenshotDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "screenshots");
                Directory.CreateDirectory(screenshotDir);

                var filePath = Path.Combine(screenshotDir, fileName);
                screenshot.SaveAsFile(filePath);

                TestContext.WriteLine($"Screenshot saved: {filePath}");

                // Attach to test results if running in CI/CD
                TestContext.AddTestAttachment(filePath, $"Screenshot - test failure");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
        }
    }
}
