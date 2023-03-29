namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Enum representing all supported browser types of <see cref="WebDriverFactory"/>
    /// </summary>
    public enum BrowserType
    {
        /// <summary>
        /// Use a headed Chrome instance
        /// </summary>
        Chrome,
        /// <summary>
        /// Use a headless Chrome instance
        /// </summary>
        HeadlessChrome,
        /// <summary>
        /// Use a headed Firefox instance
        /// </summary>
        Firefox,
        /// <summary>
        /// Use a headed Edge instance
        /// </summary>
        Edge
    }
}
