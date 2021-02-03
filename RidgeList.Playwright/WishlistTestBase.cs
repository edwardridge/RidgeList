using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;
using System;

namespace RidgeList.Playwright
{
    public class WishlistTestBase
    {
        protected IChromiumBrowser browser;
        protected IPage page;
        protected IPlaywright playwright;
        protected LoginPageObject loginPage;
        
        protected string baseUrl = "https://localhost:5001";

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            this.playwright = await PlaywrightSharp.Playwright.CreateAsync();
            this.browser = await playwright.Chromium.LaunchAsync();

            var getEnvVarsBuilder = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var env = getEnvVarsBuilder["ASPNETCORE_ENVIRONMENT"] ?? "Development";

            var builder = new ConfigurationBuilder()
                .SetBasePath(TestContext.CurrentContext.TestDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true);

            var f = builder.Build();

            baseUrl = f["BaseUrl"];
            Console.WriteLine("Base URL: " + baseUrl);
        }
        
        [SetUp]
        public async Task Setup()
        {
            this.page = await browser.NewPageAsync();
            await page.GoToAsync(baseUrl);
            await page.Context.ClearCookiesAsync();
            this.loginPage = new LoginPageObject(page);
        }
    }
}