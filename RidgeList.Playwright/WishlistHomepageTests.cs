using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PlaywrightSharp;

namespace RidgeList.Playwright
{
    [TestFixture]
    public class WishlistHomepageTests : WishlistTestBase
    {
        [Test]
        public async Task LoginWorks()
        {
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