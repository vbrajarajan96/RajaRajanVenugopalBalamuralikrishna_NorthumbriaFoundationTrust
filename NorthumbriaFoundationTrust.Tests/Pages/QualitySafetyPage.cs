using Microsoft.Playwright;
using System.Threading.Tasks;

namespace NorthumbriaFoundationTrust.Tests.Pages
{
    /// <summary>
    /// Page object for navigating Quality & Safety sections on the Northumbria NHS site.
    /// </summary>
    public class QualitySafetyPage
    {
        private readonly IPage _page;
        public QualitySafetyPage(IPage page) => _page = page;

        /// <summary>
        /// Opens the specified tile (e.g., "Continually improving services") by accessible name with fallback.
        /// </summary>
        public async Task OpenTileAsync(string linkText)
        {
            var locator = _page.GetByRole(AriaRole.Link, new() { Name = linkText });
            if (await locator.CountAsync() == 0)
                locator = _page.Locator($"a[title='{linkText}']");

            await Assertions.Expect(locator).ToBeVisibleAsync();
            await locator.ClickAsync();
        }
    }
}
