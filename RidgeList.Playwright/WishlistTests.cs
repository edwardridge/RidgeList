using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace RidgeList.Playwright
{
    public class WishlistTests : WishlistTestBase
    {
        [SetUp]
        public async Task SetupThis()
        {
            await loginPage.LoginWithCookie("test@testwishlist.com", "Test", baseUrl, baseUrl);
        }
        
        [Test]
        public async Task AllowsNewPeopleToBeAdded()
        {
            await PlaywrightHelpers.CreateWishlist(baseUrl, "AllowsNewPeopleToBeAdded", page);
            var wishlistPage = new WishlistPageObject(page);
            await wishlistPage.AddNewPerson("ed@ed.com", "Ed");
        }
        
        [Test]
        public async Task DoesntAllowDuplicateNamesToBeAdded()
        {
            await PlaywrightHelpers.CreateWishlist(baseUrl, "DoesntAllowDuplicateNamesToBeAdded", page);
            var wishlistPage = new WishlistPageObject(page);
            await wishlistPage.AddNewPerson("Ed", "ed@ed.com");
            
            await wishlistPage.ClickAddNewPerson();
            await wishlistPage.EnterNameAndEmail("Ed", "ed@ed.com");
            
            var createNewPersonIsEnabled = await wishlistPage.CreateNewPersonIsEnabled();
            createNewPersonIsEnabled.Should().BeFalse();
            
            await wishlistPage.EnterNameAndEmail("Ed", "ed2@ed.com");
            createNewPersonIsEnabled = await wishlistPage.CreateNewPersonIsEnabled();
            createNewPersonIsEnabled.Should().BeTrue();
        }
        
        [Test]
        public async Task AllowsYouToAddItem()
        {
            await PlaywrightHelpers.CreateWishlist(baseUrl, "AllowsYouToAddItem", page);
            var wishlistPage = new WishlistPageObject(page);
            await wishlistPage.AddNewPerson("Ed", "ed@ed.com");

            await wishlistPage.AddItem("New present idea");
        }
        
        [Test]
        public async Task AllowsYouToClaimAndUnclaim()
        {
            var rand = Guid.NewGuid().ToString();
            var nameOfWishlist = "AllowsYouToClaimAndUnclaim " + rand;
            await PlaywrightHelpers.CreateWishlist(baseUrl, nameOfWishlist, page);
            var wishlistPage = new WishlistPageObject(page);
            await wishlistPage.AddNewPerson("New", "new@new.com");
            await wishlistPage.AddItem("New present idea");

            await loginPage.LoginWithCookie("new@new.com", "New", baseUrl, baseUrl);
            await page.GoToAsync(baseUrl);
            await page.ClickAsync("text=" + nameOfWishlist);

            await page.ClickAsync("text=New present idea");
        }
        
        [Test]
        public async Task NewItemAutoAppearsOnOtherPersonsWishlist()
        {
            var rand = Guid.NewGuid().ToString();
            var nameOfWishlist = "NewItemAutoAppearsOnOtherPersonsWishlist " + rand;
            await PlaywrightHelpers.CreateWishlist(baseUrl, nameOfWishlist, page);
            var wishlistPage = new WishlistPageObject(page);
            await wishlistPage.AddNewPerson("New", "new@new.com");

            var page2 = await browser.NewPageAsync();
            var loginPage2 = new LoginPageObject(page2);
            await loginPage2.LoginWithCookie("new@new.com", "New", baseUrl, baseUrl);
            await page2.GoToAsync(baseUrl);
            await page2.ClickAsync("text=" + nameOfWishlist);

            await page2.WaitForTimeoutAsync(1000);

            await wishlistPage.AddItem("New present idea");
            await page2.ClickAsync("text=New present idea");
        }
    }
}