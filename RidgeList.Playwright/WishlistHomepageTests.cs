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
        [Retry(2)]
        public async Task LoginWorks()
        {
            await loginPage.LoginUsingFormWithTestAccount();
            
            page.Url.Should().Contain("/wishlists");
        }
        
        [Test]
        public async Task NoCookieRedirectsBackToHomepage()
        {
            await loginPage.LoginWithCookie(Guid.NewGuid(), baseUrl, baseUrl);
        
            await page.Context.ClearCookiesAsync();
            await page.GoToAsync(baseUrl);
            
            page.Url.Should().NotContain("/wishlists");
        }
    }
}