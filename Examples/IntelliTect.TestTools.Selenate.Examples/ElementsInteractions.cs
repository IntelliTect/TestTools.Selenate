namespace IntelliTect.TestTools.Selenate.Examples;

public class ElementsInteractions : TestBase
{
    public ElementsInteractions()
    {
        _ChallengingDomPage = new ChallengingDomPage(WebDriver);
    }

    private ChallengingDomPage _ChallengingDomPage;

    [Fact]
    public void GetASingleElementFromCollection()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/challenging_dom");
        string textToFind = "Iuvaret0";
        IWebElement foundElem = _ChallengingDomPage.FirstRow.GetSingleWebElement(x => x.Text == textToFind);
        // Make sure GetSingleElement actually returned the expected element.
        Assert.Equal(
            textToFind,
            foundElem.Text);
    }

    [Fact]
    public void GetAListOfElementsFromCollection()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/challenging_dom");
        int foundCount = _ChallengingDomPage.Headers.GetAllWebElements(x => x.Displayed).Count;
        Assert.Equal(
            7,
            foundCount);
    }

    [Fact]
    public void GetAnEnumerableOfElementHandlers()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/challenging_dom");
        // We expect exactly 7 headers here.
        // However, depending on specific scenarios, E.G. dynamic content,
        //   may make sense to use a not do a ToList() and Assert.Count and instead
        //   use a lower .Take() and go straight to iterating
        IList<ElementHandler> foundHandlers = _ChallengingDomPage.Headers.GetElementHandlers().Take(7).ToList();

        Assert.True(foundHandlers.Count is 7);

        foreach (ElementHandler handler in foundHandlers)
        {
            Assert.True(handler.WaitForDisplayed());
        }
    }
}
