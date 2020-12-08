﻿using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Atata
{
    /// <summary>
    /// Provides a set of extension methods for <see cref="ISearchContext"/>
    /// that wrap actual methods with log sections.
    /// </summary>
    public static class ISearchContextLoggingExtensions
    {
        /// <summary>
        /// Gets an element within a log section.
        /// </summary>
        /// <param name="searchContext">The search context.</param>
        /// <param name="by">The by.</param>
        /// <returns>Found element.</returns>
        public static IWebElement GetWithLogging(this ISearchContext searchContext, By by)
        {
            ILogManager log = AtataContext.Current?.Log;

            return log != null
                ? log.ExecuteSection(
                    new ElementFindLogSection(searchContext, by),
                    () => searchContext.Get(by))
                : searchContext.Get(by);
        }

        /// <summary>
        /// Gets all elements within a log section.
        /// </summary>
        /// <param name="searchContext">The search context.</param>
        /// <param name="by">The by.</param>
        /// <returns>Found elements.</returns>
        public static ReadOnlyCollection<IWebElement> GetAllWithLogging(this ISearchContext searchContext, By by)
        {
            ILogManager log = AtataContext.Current?.Log;

            return log != null
                ? log.ExecuteSection(
                    new ElementFindLogSection(searchContext, by, multiple: true),
                    () => searchContext.GetAll(by))
                : searchContext.GetAll(by);
        }
    }
}
