using Microsoft.Playwright;
using System.Threading.Tasks;

namespace NorthumbriaFoundationTrust.Tests.Pages
{
    /// <summary>
    /// Page object for the Northumbria NHS homepage search functionality.
    /// </summary>
    public class HomePage
    {
        private readonly IPage _page;

        public HomePage(IPage page) => _page = page;

        public Task OpenAsync(string baseUrl) => _page.GotoAsync(baseUrl);

        // Returns the search input field via placeholder with a reliable fallback
        public async Task<ILocator> GetSearchInputAsync()
        {
            var primary = _page.GetByPlaceholder("What can we help you to find today?");
            if (await primary.CountAsync() > 0)
                return primary;

            return _page.Locator("input.search-field[placeholder*='find today' i]").First;
        }

        // Returns the search button via role and aria-label with fallback
        public async Task<ILocator> GetSearchButtonAsync()
        {
            var primary = _page.GetByRole(AriaRole.Button, new() { Name = "Search" });
            if (await primary.CountAsync() > 0)
                return primary;

            return _page.Locator("button.submit-btn[aria-label='Search']").First;
        }

        public async Task EnterSearchAsync(string term)
        {
            var input = await GetSearchInputAsync();
            await input.FillAsync(term);
        }

        public async Task SubmitSearchByButtonAsync()
        {
            var btn = await GetSearchButtonAsync();
            await btn.ClickAsync();
        }

        public async Task SubmitSearchByEnterAsync()
        {
            var input = await GetSearchInputAsync();
            await input.PressAsync("Enter");
        }
    }
}
