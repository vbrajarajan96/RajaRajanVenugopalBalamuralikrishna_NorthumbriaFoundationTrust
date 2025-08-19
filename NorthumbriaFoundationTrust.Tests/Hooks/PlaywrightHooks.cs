using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using TechTalk.SpecFlow;
using NorthumbriaFoundationTrust.Tests.Support;

namespace NorthumbriaFoundationTrust.Tests.Hooks
{
    [Binding]
    public sealed class PlaywrightHooks
    {
        private readonly UiContext _ctx;
        private bool _traceEnabled;
        private string? _tracePath;

        public PlaywrightHooks(UiContext ctx) => _ctx = ctx;

        [BeforeScenario]
        public async Task BeforeScenario(ScenarioContext scenario)
        {
            // Initialise Playwright and browser
            _ctx.Playwright = await Playwright.CreateAsync();

            // Launch browser based on environment setting
            Console.WriteLine($"[DEBUG] Launching browser: {_ctx.BrowserName}");
            _ctx.Browser = _ctx.BrowserName switch
            {
                "firefox" => await _ctx.Playwright.Firefox.LaunchAsync(new() { Headless = false }),
                "webkit" => await _ctx.Playwright.Webkit.LaunchAsync(new() { Headless = false }),
                _ => await _ctx.Playwright.Chromium.LaunchAsync(new() { Headless = false })
            };

            // Create browser context and page
            _ctx.BrowserContext = await _ctx.Browser.NewContextAsync(new()
            {
                IgnoreHTTPSErrors = true,
                ViewportSize = new() { Width = 1280, Height = 900 }
            });
            _ctx.Page = await _ctx.BrowserContext.NewPageAsync();
            _ctx.Page.SetDefaultTimeout(10_000);
            _ctx.Page.SetDefaultNavigationTimeout(15_000);

            // Enable tracing if specified
            _traceEnabled = IsEnabled(Environment.GetEnvironmentVariable("TRACE"));
            if (_traceEnabled)
            {
                await _ctx.BrowserContext.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
                var safeName = SafeFileSegment(scenario.ScenarioInfo.Title);
                _tracePath = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    $"trace_{safeName}.zip");
            }
        }

        [AfterScenario]
        public async Task AfterScenario(ScenarioContext scenario)
        {
            try
            {
                // Attach trace file if enabled
                if (_traceEnabled && _ctx.BrowserContext is not null && !string.IsNullOrWhiteSpace(_tracePath))
                {
                    await _ctx.BrowserContext.Tracing.StopAsync(new() { Path = _tracePath });
                    if (File.Exists(_tracePath))
                        TestContext.AddTestAttachment(_tracePath);
                }

                // Attach screenshot on test failure
                if (scenario.TestError is not null && _ctx.Page is not null)
                {
                    var file = Path.Combine(
                        TestContext.CurrentContext.WorkDirectory,
                        $"Failure_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png");
                    await _ctx.Page.ScreenshotAsync(new() { Path = file, FullPage = true });
                    if (File.Exists(file))
                        TestContext.AddTestAttachment(file);
                }
            }
            finally
            {
                // Clean up resources
                if (_ctx.Page is not null) await _ctx.Page.CloseAsync();
                if (_ctx.BrowserContext is not null) await _ctx.BrowserContext.CloseAsync();
                if (_ctx.Browser is not null) await _ctx.Browser.CloseAsync();
                _ctx.Playwright?.Dispose();
            }
        }

        private static bool IsEnabled(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var v = value.Trim().ToLowerInvariant();
            return v is "1" or "true" or "yes" or "on";
        }

        private static string SafeFileSegment(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(input.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
            return string.IsNullOrWhiteSpace(cleaned) ? "scenario" : cleaned;
        }
    }
}
