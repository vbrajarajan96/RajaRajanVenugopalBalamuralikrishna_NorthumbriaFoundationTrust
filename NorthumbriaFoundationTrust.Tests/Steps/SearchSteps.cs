using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Microsoft.Playwright;
using NorthumbriaFoundationTrust.Tests.Pages;
using NorthumbriaFoundationTrust.Tests.Support;

namespace NorthumbriaFoundationTrust.Tests.Steps
{
    [Binding]
    public class SearchSteps
    {
        private readonly UiContext _ctx;
        private HomePage? _home;

        public SearchSteps(UiContext ctx) => _ctx = ctx;

        [Given(@"the user is on the Northumbria NHS homepage")]
        public async Task GivenOnHome()
        {
            _home = new HomePage(_ctx.Page);
            await _home.OpenAsync(_ctx.BaseUrl);
            var searchInput = _ctx.Page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your search" });
            await searchInput.WaitForAsync();
        }

        [When(@"the user enters ""(.*)"" in the search field")]
        public async Task WhenTheUserEntersInTheSearchField(string term)
        {
            await _home!.EnterSearchAsync(term);
        }

        [When(@"the user clicks the search button")]
        public async Task WhenTheUserClicksTheSearchButton()
        {
            await _home!.SubmitSearchByButtonAsync();
            await _ctx.Page.Locator("h2.results-heading").GetByText("Page results").WaitForAsync();
            await _ctx.Page.Locator("#search-results").WaitForAsync();
        }

        [When(@"the user presses Enter to submit the search")]
        public async Task WhenTheUserPressesEnterToSubmitTheSearch()
        {
            await _home!.SubmitSearchByEnterAsync();
            await _ctx.Page.Locator("h2.results-heading").GetByText("Page results").WaitForAsync();
            await _ctx.Page.Locator("#search-results").WaitForAsync();
        }

        [Then(@"results relevant to ""(.*)"" are displayed")]
        public async Task ThenResultsRelevantToAreDisplayed(string term)
        {
            var hit = _ctx.Page.Locator("#search-results a", new() { HasText = term }).First;
            await hit.WaitForAsync();
            Assert.That(await hit.IsVisibleAsync(), Is.True, $"Expected at least one result containing '{term}'.");
        }

        [Then(@"the user selects the ""(.*)"" link on the results page")]
        public async Task ThenTheUserSelectsTheLinkOnTheResultsPage(string linkText)
        {
            var results = _ctx.Page.Locator("#search-results");
            var target = results.Locator("ul.results li.search-result a", new() { HasText = linkText }).First;
            await target.ClickAsync();
        }

        [Then(@"the user navigates to the ""(.*)"" page")]
        public async Task ThenTheUserNavigatesToThePage(string linkText)
        {
            var qsLink = _ctx.Page.GetByRole(AriaRole.Link, new() { Name = linkText });
            var fallback = _ctx.Page.Locator("a:has-text('" + linkText + "')").First;
            if (await qsLink.CountAsync() > 0)
                await qsLink.ClickAsync();
            else
                await fallback.ClickAsync();
            await _ctx.Page.WaitForURLAsync(new Regex(".*/continually-improving-services/?$", RegexOptions.IgnoreCase));
        }

        [Then(@"the page shows the relevant information about this section")]
        public async Task ThenThePageShowsTheRelevantInformationAboutThisSection()
        {
            var snippet = _ctx.Page.GetByText(UiContext.Texts.ContinuallyImprovingContentSnippet, new() { Exact = false }).First;
            await snippet.WaitForAsync();
            Assert.That(await snippet.IsVisibleAsync(), Is.True, "Expected key content about the '15 steps' programme.");
        }
    }
}
