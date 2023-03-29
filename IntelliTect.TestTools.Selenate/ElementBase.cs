using OpenQA.Selenium;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Base class for ElementHandler and ElementsHandler. <br />
    /// Keeps track of a Locator and SearchContext
    /// </summary>
    public class ElementBase : HandlerBase
    {
        /// <summary>
        /// Instantiate a new ElementBase with a driver and locator
        /// </summary>
        /// <param name="driver">The IWebDriver to wrap</param>
        /// <param name="locator">The By locator to use for finding an element</param>
        public ElementBase(IWebDriver driver, By locator) : base(driver)
        {
            Locator = locator;
            SearchContext = driver;
        }

        /// <summary>
        /// The locator used to find IWebElements in this handler.
        /// </summary>
        public By Locator { get; protected set; }
        /// <summary>
        /// The SearchContext to use when attempting to find an element. <br />
        /// By default, it will be the wrapped IWebDriver
        /// </summary>
        protected ISearchContext SearchContext { get; set; }
    }
}
