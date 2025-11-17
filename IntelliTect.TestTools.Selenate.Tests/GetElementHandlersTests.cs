using OpenQA.Selenium.DevTools.V115.FedCm;

namespace IntelliTect.TestTools.Selenate.Tests;

public class GetElementHandlersTests
{
    private const string ByIdCriteria = "#test";
    private const string ByClassNameCriteria = ".test";
    private const string ByNameCriteria = "*[name =\"test\"]";
    private const string ByCssCriteria = "div[id='test']";
    private const string ByXPathCriteria = "//div[@id='test']";
    private const string CssMechanism = "css selector";
    private const string XPathMechanism = "xpath";

    // Contract tests. If Selenium ever changes, these will fail and let us know.
    [Fact]
    public void VerifyByIdDoesNotChangeFormat()
    {
        By by = By.Id("test");

        Assert.Equal(CssMechanism, by.Mechanism);
        Assert.Equal(ByIdCriteria, by.Criteria);
    }

    [Fact]
    public void VerifyByClassNameDoesNotChangeFormat()
    {
        By by = By.ClassName("test");

        Assert.Equal(CssMechanism, by.Mechanism);
        Assert.Equal(ByClassNameCriteria, by.Criteria);
    }

    [Fact]
    public void VerifyByNameDoesNotChangeFormat()
    {
        By by = By.Name("test");

        Assert.Equal(CssMechanism, by.Mechanism);
        Assert.Equal(ByNameCriteria, by.Criteria);
    }

    [Fact]
    public void VerifyByCssDoesNotChangeFormat()
    {
        By by = By.CssSelector(ByCssCriteria);

        Assert.Equal(CssMechanism, by.Mechanism);
        Assert.Equal(ByCssCriteria, by.Criteria);
    }

    [Fact]
    public void VerifyByXPathDoesNotChangeFormat()
    {
        By by = By.XPath(ByXPathCriteria);

        Assert.Equal(XPathMechanism, by.Mechanism);
        Assert.Equal(ByXPathCriteria, by.Criteria);
    }

    // Tests for actual Selenate code.
    [Theory]
    [InlineData("id")]
    [InlineData("class")]
    [InlineData("name")]
    [InlineData("css")]
    [InlineData("xpath")]
    public void GetElementHandlersWorksWithAllSelectorTypes(string selectorType)
    {
        const string cssIndex = ":nth-of-type";
        List<By> convertedBys = new();

        for (int i = 1; i < 3; i++)
        {
            convertedBys.Add(selectorType switch
            {
                "id" => By.CssSelector($"{ByIdCriteria}{cssIndex}({i})"),
                "class" => By.CssSelector($"{ByClassNameCriteria}{cssIndex}({i})"),
                "name" => By.CssSelector($"{ByNameCriteria}{cssIndex}({i})"),
                "css" => By.CssSelector($"{ByCssCriteria}{cssIndex}({i})"),
                "xpath" => By.XPath($"{ByXPathCriteria}[{i}]"),
                _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
            });
        }

        By by = selectorType switch
        {
            "id" => By.Id("test"),
            "class" => By.ClassName("test"),
            "name" => By.Name("test"),
            "css" => By.CssSelector("div[id='test']"),
            "xpath" => By.XPath("//div[@id='test']"),
            _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
        };

        var mockElement1 = new Mock<IWebElement>();
        mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
        mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);

        var mockElement2 = new Mock<IWebElement>();
        mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
        mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);

        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup
            (f => f.FindElement(convertedBys[0]))
            .Returns(mockElement1.Object);

        mockDriver.Setup
            (f => f.FindElement(convertedBys[1]))
            .Returns(mockElement2.Object);

        ElementsHandler handler = new(mockDriver.Object, by);
        IEnumerable<ElementHandler> elementHandlers = handler.SetTimeout(TimeSpan.FromMilliseconds(20)).GetElementHandlers();

        Assert.Single(elementHandlers);
    }

    [Theory]
    [InlineData("css", "div[id='test']>div", "div[id='test']>div:nth-of-type({index})")]
    [InlineData("css", "div[id='test']{index}>div", "div[id='test']:nth-of-type({index})>div")]
    [InlineData("xpath", "//div[@id='test']/div", "//div[@id='test']/div[{index}]")]
    [InlineData("xpath", "//div[@id='test']{index}/div", "//div[@id='test'][{index}]/div")]
    public void GetElementHandlersWorksWithAllSelectorTypesTest(string selectorType, string selectorCriteria, string expectedResult)
    {
        //const string cssIndex = ":nth-of-type";
        List<By> convertedBys = new();

        for (int i = 1; i < 3; i++)
        {
            convertedBys.Add(selectorType switch
            {
                "css" => By.CssSelector(expectedResult.Replace("{index}", i.ToString())),
                "xpath" => By.XPath(expectedResult.Replace("{index}", i.ToString())),
                _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
            });
        }

        By by = selectorType switch
        {
            "css" => By.CssSelector(selectorCriteria),
            "xpath" => By.XPath(selectorCriteria),
            _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
        };

        var mockElement1 = new Mock<IWebElement>();
        mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
        mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);

        var mockElement2 = new Mock<IWebElement>();
        mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
        mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);

        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup
            (f => f.FindElement(convertedBys[0]))
            .Returns(mockElement1.Object);

        mockDriver.Setup
            (f => f.FindElement(convertedBys[1]))
            .Returns(mockElement2.Object);

        ElementsHandler handler = new(mockDriver.Object, By.CssSelector("div#test"));
        IEnumerable<ElementHandler> elementHandlers = handler
            .SetTimeout(TimeSpan.FromMilliseconds(20))
            .GetElementHandlers(by);

        Assert.Single(elementHandlers);
        Assert.Equal(convertedBys[0].Criteria, elementHandlers.First().Locator.Criteria);
    }

    [Theory]
    [InlineData("tagname")]
    [InlineData("link")]
    [InlineData("partiallink")]
    public void UnsupportedSelectorsThrowArgumentException(string selectorType)
    {
        By by = selectorType switch
        {
            "tagname" => By.TagName("test"),
            "link" => By.LinkText("test"),
            "partiallink" => By.PartialLinkText("test"),
            _ => throw new ArgumentException($"Please add support for this selector type: {selectorType}")
        };

        ElementsHandler handler = new(new Mock<IWebDriver>().Object, by);

        Assert.Throws<ArgumentException>(() => handler.GetElementHandlers().ToList());
    }
}
