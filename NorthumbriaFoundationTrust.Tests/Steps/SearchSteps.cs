using Microsoft.Playwright;
using NorthumbriaFoundationTrust.Tests.Pages;
using NorthumbriaFoundationTrust.Tests.Support;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace NorthumbriaFoundationTrust.Tests.Steps
{
    [Binding]
    public class SearchSteps
    {
        private readonly UiContext _ctx;
        private HomePage? _home;
        private SearchResultsPage? _results;
        private QualitySafetyPage? _quality;
        private string? _searchTerm;

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
            _searchTerm = term;
            await _home!.EnterSearchAsync(term);
        }

        [When(@"the user clicks the search button")]
        public async Task WhenTheUserClicksTheSearchButton()
        {
            await _home!.SubmitSearchByButtonAsync();
            _results = new SearchResultsPage(_ctx.Page);
            await _results.WaitForResultsAsync(_searchTerm ?? UiContext.Texts.SearchTerm);
        }

        [When(@"the user presses Enter to submit the search")]
        public async Task WhenTheUserPressesEnterToSubmitTheSearch()
        {
            await _home!.SubmitSearchByEnterAsync();
            _results = new SearchResultsPage(_ctx.Page);
            await _results.WaitForResultsAsync(_searchTerm ?? UiContext.Texts.SearchTerm);
        }

        [Then(@"results relevant to ""(.*)"" are displayed")]
        public async Task ThenResultsRelevantToAreDisplayed(string term)
        {
            _results ??= new SearchResultsPage(_ctx.Page);
            var hit = _ctx.Page.Locator("#search-results a", new() { HasText = term }).First;
            await hit.WaitForAsync();
            Assert.That(await hit.IsVisibleAsync(), Is.True, $"Expected at least one result containing '{term}'.");
        }

        [Then(@"the user selects the ""(.*)"" link on the results page")]
        public async Task ThenTheUserSelectsTheLinkOnTheResultsPage(string linkText)
        {
            _results ??= new SearchResultsPage(_ctx.Page);
            await _results.OpenResultAsync(linkText);
        }

        [Then(@"the user navigates to the ""(.*)"" page")]
        public async Task ThenTheUserNavigatesToThePage(string linkText)
        {
            _quality ??= new QualitySafetyPage(_ctx.Page);
            await _quality.OpenTileAsync(linkText);
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
