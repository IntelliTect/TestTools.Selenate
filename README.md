Selenate
==========
Provides wrapper functions to leverage commonly used WebDriverWaits. Note that this does not provide exhaustive functionality, and customer WebDriverWaits will still be needed for complex or unique scenarios.

Simple Usage
-----
To instantiate a class that manages simple WebDriver interactions:
```C#
// WebDriverFactory is not required. You can pass in your own IWebDriver to DriverHandler instead.
IWebDriver driver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
DriverHandler handler = new(driver);
handler.NavigateToPage("http://www.some-page.com/");
```

To instantiate a class that manages simple interactions with a single IWebElement (note: this uses a Page Object Model approach):
```C#
public class SomePageUnderTest
{
    public SomePageUnderTest(IWebDriver driver)
    {
        _Driver = driver;
    }

    // Note that this will not save any settings (e.g. StartButton.SetTimeoutSeconds(30)) made after object instantiation.
    // Use an explicit getter with a private backing field for that.
    public ElementHandler SomeButton => new (_Driver, By.CssSelector("div[id='someId']"));

    private IWebDriver _Driver;
}

public class SomeTest
{
    public SomeTest()
    {
        IWebDriver driver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
        _Handler = new(driver);
    }

    private DriverHandler _Handler;

    // Also works with other runners like MSTest and NUnit
    [Fact]
    public void Testing()
    {
        // Arrange
        SomePageUnderTest put = new(_Handler.WrappedDriver);
        _Handler.NavigateToPage("http://www.some-page.com/");

        // Act
        // Below automatically waits for the element to exist and be clickable before executing.
        put.SomeButton.Click();

        // Assert (pretend the button disappears after clicking it.)
        // Default timeout is 15s. Override for this call.
        Assert.True(put.SomeButton.SetTimeoutSeconds(5).WaitForNotDisplayed(),
            "The button did not disappear within 5s after clicking it.");
    }
}

```

Complex Use Cases
-----
Using the same structure as above, we can create complex use cases

```
public void SomeTest
{
    // Same setup as above.

    // Also works with other runners like MSTest and NUnit
    [Fact]
    public void SomeOtherTest()
    {
        // Arrange
        // Note: to reduce code duplication, this can be abstracted out into the Page Object for small projects,
        // Or into an explicit class of grouped actions for large projects.
        SomePageUnderTest put = new(_Handler.WrappedDriver);
        _Handler.NavigateToPage("http://www.some-page.com/");

        // Act and Assert
        // Sometimes the site is slow to load, and the initial click doesn't do anything.
        // For whatever reason, dev team won't fix.

        WebDriverWait wait = new(_Handler.WrappedDriver, TimeSpan.FromSeconds(30));
        wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(InvalidElementStateException),
            typeof(ElementNotVisibleException),
            typeof(StaleElementReferenceException),
            typeof(ElementClickInterceptedException));
        
        // If the button continues to be displayed once the 30s timeout is reached,
        // This will throw a WebDriverTimeout exception.
        // Note: if you want a friendly assert message,
        //   you can wrap just this block of code in a try/catch(WebDriverException) and selectively assign a variable true or false / assert on the result.
        // Best practice in that scenario is to abstract out to a harness page object or group action object as soon as a second instance of this pattern is used.
        wait.Until(x => x
        {
            IWebElement someButton = x.FindElement(put.SomeButton.Locator);
            someButton.Click();
            return !someButton.Displayed;
        })
    }
}
```