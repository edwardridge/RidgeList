using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace RidgeList.Playwright
{
    public class WishlistSummaryPageTests : WishlistTestBase
    {
        [SetUp]
        public async Task SetupThis()
        {
            await loginPage.LoginWithCookie(Guid.NewGuid(), baseUrl, baseUrl);
        }

        [Test]
        public async Task CanCreateNewWishlist()
        {
            var rand = new Random().Next(0, 10000);
            var name = $"[Test] From Cypress {rand}";
            await page.ClickAsync(Cy.Name("CreateNewWishlist"));
            await page.TypeAsync(Cy.Name("NameOfWishlist"), name);
            await page.ClickAsync(Cy.Name("Create"));
            page.Url.Should().Contain("/wishlist");
        }
    }
}