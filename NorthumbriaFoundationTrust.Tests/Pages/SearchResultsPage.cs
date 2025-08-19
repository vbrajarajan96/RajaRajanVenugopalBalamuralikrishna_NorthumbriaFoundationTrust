using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace NorthumbriaFoundationTrust.Tests.Pages
{
    /// <summary>
    /// Page object for handling search results on the Northumbria NHS website.
    /// </summary>
    public class SearchResultsPage
    {
        private readonly IPage _page;
        public SearchResultsPage(IPage page) => _page = page;

        // Main search results content area
        private ILocator ResultsContainer => _page.Locator("#page-results");

        /// <summary>
        /// Waits for the results page to load and for any result item containing the search term within results container.
        /// </summary>
        public async Task WaitForResultsAsync(string term)
        {
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var anyHit = ResultsContainer.GetByText(term, new() { Exact = false }).First;
            await anyHit.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = 60000
            });
        }

        /// <summary>
        /// Returns a locator for a result link by matching text, scoped inside results container.
        /// </summary>
        public ILocator ResultLink(string text) =>
            ResultsContainer.GetByRole(AriaRole.Link, new() { NameRegex = new Regex(text, RegexOptions.IgnoreCase) });

        /// <summary>
        /// Opens the first matching result link.
        /// </summary>
        public Task OpenResultAsync(string linkText) =>
            ResultLink(linkText).First.ClickAsync();
    }
}
