using Microsoft.Playwright;

namespace NorthumbriaFoundationTrust.Tests.Support
{
    /// <summary>
    /// Shared UI context and configuration for Playwright tests.
    /// Centralizes Playwright objects and environment-driven settings.
    /// </summary>
    public class UiContext
    {
        // Playwright runtime objects
        public IPlaywright Playwright { get; set; } = default!;
        public IBrowser Browser { get; set; } = default!;
        public IBrowserContext BrowserContext { get; set; } = default!;
        public IPage Page { get; set; } = default!;

        // Settings resolved from environment variables (with safe defaults)
        public string BaseUrl { get; } =
            Environment.GetEnvironmentVariable("BASE_URL")?.TrimEnd('/') ??
            "https://www.northumbria.nhs.uk";

        public string BrowserName { get; } =
            Environment.GetEnvironmentVariable("BROWSER")?.ToLower() switch
            {
                "firefox" => "firefox",
                "webkit" => "webkit",
                _ => "chromium"
            };

        public bool Headless { get; } =
            !string.Equals(Environment.GetEnvironmentVariable("HEADLESS"), "false", StringComparison.OrdinalIgnoreCase);

        public static class Texts
        {
            public const string SearchTerm = "Quality and safety";
            public const string QualityAndSafetyLink = "Quality and safety";
            public const string ContinuallyImprovingLink = "Continually improving services";
            public const string ContinuallyImprovingContentSnippet = "15 Steps";
        }
    }
}
