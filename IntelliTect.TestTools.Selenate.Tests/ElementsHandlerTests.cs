namespace IntelliTect.TestTools.Selenate.Tests;

public class ElementsHandlerTests
{
    [Fact]
    public void GetTextReturnsExpectedWhenFound()
    {
        Assert.True(SetupMockedData().ContainsText("Testing1"));
    }

    [Fact]
    public void GetTextReturnsFalseWhenUnableToFindElementWithText()
    {
        Assert.False(SetupMockedData().ContainsText("TestingA"));
    }

    [Fact]
    public void GetSpecificExistingElementReturnsFoundElements()
    {
        Assert.NotNull(
            SetupMockedData()
            .GetSingleWebElement(x => 
                x.Displayed));
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenNoElementsMatch()
    {
        Assert.Throws<WebDriverTimeoutException>(() => 
            SetupMockedData()
            .GetSingleWebElement(x => 
                x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetElementsThrowsWhenNoElementsMatch()
    {
        Assert.Throws<WebDriverTimeoutException>(() =>
            SetupMockedData()
            .GetAllWebElements(x =>
                x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenMultipleElementsMatch()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SetupMockedData()
            .GetSingleWebElement(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetElementsReturnsWhenMultipleElementsMatch()
    {
        Assert.Equal(2,
            SetupMockedData()
            .GetAllWebElements(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase))
            .Count);
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenNoElementsAreFound()
    {
        Assert.Throws<WebDriverTimeoutException>(() =>
            SetupMockedData()
            .SetLocator(By.Id("blarg"))
            .GetSingleWebElement(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
    }

    [Theory]
    [InlineData("id")]
    [InlineData("class")]
    [InlineData("css")]
    [InlineData("xpath")]
    [InlineData("name")]
    [InlineData("tagname")]
    [InlineData("link")]
    [InlineData("partiallink")]
    public void GetElementHandlersWorksWithAllSelectorTypes(string selectorType)
    {
        List<By> bys = new();
        for (int i = 1; i < 4; i++)
        {

        }

        By by = selectorType switch
        {
            "id" => By.Id("test"),
            "class" => By.ClassName("test"),
            "css" => By.CssSelector("div[id='test']"),
            "xpath" => By.XPath("//div[@id='test']"),
            "name" => By.Name("test"),
            "tagname" => By.TagName("test"),
            "link" => By.LinkText("test"),
            "partiallink" => By.PartialLinkText("test"),
            _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
        };

        //var mockElement1 = new Mock<IWebElement>();
        //mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
        //mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);

        //var mockElement2 = new Mock<IWebElement>();
        //mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
        //mockElement2.SetupGet(e2 => e2.Displayed).Returns(true);

        //var mockElement3 = new Mock<IWebElement>();
        //mockElement3.SetupGet(e2 => e2.Text).Returns("Testing3");
        //mockElement3.SetupGet(e2 => e2.Displayed).Returns(false);

        //var mockDriver = new Mock<IWebDriver>();
        //mockDriver.Setup
        //    (f => f.FindElement(by))
        //    .Returns(mockElement1.Object);

        //mockDriver.Setup
        //    (f => f.FindElements(By.Id("blarg")))
        //    .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>()));

        //return new ElementsHandler(mockDriver.Object, By.Id("test"))
        //    .SetTimeout(TimeSpan.FromMilliseconds(20))
        //    .SetPollingIntervalMilliseconds(10);
    }

    private static ElementsHandler SetupMockedData()
    {
        var mockElement1 = new Mock<IWebElement>();
        mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
        mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);
        
        var mockElement2 = new Mock<IWebElement>();
        mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
        mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);
        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup
            (f => f.FindElements(By.Id("test")))
            .Returns(
                new ReadOnlyCollection<IWebElement>(
                    new List<IWebElement> { mockElement1.Object, mockElement2.Object }));

        mockDriver.Setup
            (f => f.FindElements(By.Id("blarg")))
            .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>()));

        return new ElementsHandler(mockDriver.Object, By.Id("test"))
            .SetTimeout(TimeSpan.FromMilliseconds(20))
            .SetPollingIntervalMilliseconds(10);
    }
}