# Northumbria NHS - Search automation (C#, Playwright, SpecFlow, .NET 8)
## Prerequisites
- .NET 8 SDK
- Playwright CLI and browsers
## Clone Repository

To clone this repository locally, run:
```sh
git clone https://github.com/vbrajarajan96/RajaRajanVenugopalBalamuralikrishna_NorthumbriaFoundationTrust.git
```
## Setup
1. Restore packages
```sh
dotnet restore
```
2. Install Playwright browsers (run either command below)
```sh
playwright install
```
*or (PowerShell)*
```powershell
pwsh -NoProfile -Command "playwright install"
```
## Run
By default, tests launch in **Chromium (headed mode)** so you can see the browser actions.
Run all tests (Chromium headed):
```sh
dotnet test
```
### Firefox
Tests can be configured to run in Firefox by using the `firefox.runsettings` file:
1. In Visual Studio:  
   - Go to **Test → Configure Run Settings → Select Solution Wide Run Settings File...**  
   - Choose `firefox.runsettings`.  
   - This sets the environment variable `BROWSER=firefox`.
2. Now simply run:
```sh
dotnet test
```
### Headless / Headed mode
Tests are configured to always run in **headed mode** by default, so you can see the browser actions clearly.  
This is intentional for interview demonstration and transparency.  

(Advanced: If you wanted to support headless mode, framework can be extended to honour the HEADLESS variable, but this has not been enabled for this submission.)

## Structure
```
NorthumbriaFoundationTrust.Tests/
  Features/Search.feature
  Steps/SearchSteps.cs
  Pages/HomePage.cs
  Pages/SearchResultsPage.cs
  Pages/QualitySafetyPage.cs
  Hooks/PlaywrightHooks.cs
  Support/UiContext.cs
  specflow.json
  README.md
  NorthumbriaFoundationTrust.Tests.csproj
  chromium.runsettings
  firefox.runsettings

```

### Tracing (optional)
You can capture a Playwright **trace per scenario** to aid debugging. Traces are saved as `.zip` files and attached to the test run output.

**How it works in this suite**
- Set the `TRACE` environment variable to enable tracing.
- Each scenario writes a trace file into the test run directory as:
  `trace_<scenario-title>.zip`
- On failure, a full‑page screenshot is also attached.(enabled by default)

## Notes
- Scenarios map to acceptance criteria (Search via button & Enter, open "Quality and safety" from results, navigate to "Continually improving services", verify key content).
- Page Object Model for reusability & maintainability.
- Locators prefer accessible queries (roles, labels, placeholders) with Regex fallbacks.
- Cross-browser controlled via the `BROWSER` env var (`chromium` default, `firefox` supported).
- Supported browsers: **Chromium (Chrome family engine)** by default, and **Firefox** via runsettings. 
  (If required, the suite can be pointed to the Chrome channel via Playwright launch options.)

