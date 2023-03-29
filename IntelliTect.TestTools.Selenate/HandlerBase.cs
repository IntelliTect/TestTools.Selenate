using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Base class for handling Selenium interactions.
    /// </summary>
    public class HandlerBase
    {
        /// <summary>
        /// Base class for handling Selenium interactions.
        /// </summary>
        /// <param name="driver">The WebDriver needed to driver all of the Selenium interactions</param>
        public HandlerBase(IWebDriver driver)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            WrappedDriver = driver;
        }

        /// <summary>
        /// The WebDriver this instance is wrapping.
        /// </summary>
        public IWebDriver WrappedDriver { get; }
        /// <summary>
        /// The time it will take for Selenate to stop attempting an action and throw a WebDriverTimeout exception
        /// </summary>
        protected TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);
        /// <summary>
        /// The time to wait in between attempts when invoking Selenium
        /// </summary>
        protected TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        /// <summary>
        /// Basic DefaultWait that can be used in many scenarios when constructing WebDriverWaits
        /// </summary>
        protected DefaultWait<IWebDriver> Wait => 
            new(WrappedDriver) 
            { 
                Timeout = Timeout, 
                PollingInterval = PollingInterval 
            };

        /// <summary>
        /// Sets the maximum time that this instance will retry a specific interaction with Selenium before throwing.
        /// </summary>
        /// <typeparam name="T">The type for the wrapping class</typeparam>
        /// <param name="timeout">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if <paramref name="timeout"/> is less than 1ms</exception>
        protected T SetTimeout<T>(TimeSpan timeout) where T : HandlerBase
        {
            if (timeout.TotalMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Please provide a positive value.");
            }

            Timeout = timeout;
            return (T)this;
        }

        /// <summary>
        /// Sets the amount of time this instance will wait in between retrying a specific interaction.
        /// </summary>
        /// <typeparam name="T">The type for the wrapping class</typeparam>
        /// <param name="pollingInterval">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws if <paramref name="pollingInterval"/> is less than 1ms</exception>
        protected T SetPollingInterval<T>(TimeSpan pollingInterval) where T : HandlerBase
        {
            if (pollingInterval.TotalMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), "Please provide a positive value.");
            }

            PollingInterval = pollingInterval;
            return (T)this;
        }
    }
}
