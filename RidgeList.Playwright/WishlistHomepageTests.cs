using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PlaywrightSharp;

namespace RidgeList.Playwright
{
    [TestFixture]
    public class WishlistHomepageTests
    {
        public const string baseUrl = "https://ridgelist-ci-gfhqqojama-nw.a.run.app";
        
        [Test]
        public async Task LoginWorks()
        {
            using var playwright = await PlaywrightSharp.Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(headless: false);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(baseUrl);
            await page.Context.ClearCookiesAsync();
            var loginPage = new LoginPageObject(page);
            await loginPage.LoginUsingFormWithTestAccount();

            page.Url.Should().Contain("/wishlists");
        }
        
        // [Test]
        // public async Task NoCookieRedirectsBackToHomepage()
        // {
        //     await loginPage.LoginWithCookie("test@testwishlist.com", "Test", baseUrl, baseUrl);
        //
        //     await page.Context.ClearCookiesAsync();
        //     await page.GoToAsync(baseUrl);
        //     
        //     page.Url.Should().NotContain("/wishlists");
        // }
    }
}