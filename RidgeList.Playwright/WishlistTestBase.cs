using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;

namespace RidgeList.Playwright
{
    public class WishlistTestBase
    {
        protected IChromiumBrowser browser;
        protected IPage page;
        protected IPlaywright playwright;
        protected LoginPageObject loginPage;
        
        public const string baseUrl = "https://ridgelist-ci-gfhqqojama-nw.a.run.app";

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            this.playwright = await PlaywrightSharp.Playwright.CreateAsync();
            this.browser = await playwright.Chromium.LaunchAsync();
        }
        
        // [SetUp]
        // public async Task Setup()
        // {
        //     this.page = await browser.NewPageAsync();
        //     await page.GoToAsync(baseUrl);
        //     await page.Context.ClearCookiesAsync();
        //     this.loginPage = new LoginPageObject(page);
        // }
    }
}