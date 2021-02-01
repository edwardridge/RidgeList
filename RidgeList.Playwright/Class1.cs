using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esprima.Ast;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;

namespace RidgeList.Playwright
{
    public class ClearDown
    {
        [Test, Ignore("")]
        public async Task ClearDownOldWishlists()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(WishlistTestBase.baseUrl)
            };
            await httpClient.PostAsync("/WishlistTest/clearOldTestWishlists", new StringContent(string.Empty));
        }
    }

    public static class PlaywrightHelpers
    {
        public class CreateTestWishlistModel
        {
            public string title { get; set; }
        }
        public static async Task CreateWishlist(string title, IPage page)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(WishlistTestBase.baseUrl)
            };
            var response = await httpClient.PostAsync("/WishlistTest/createTestWishlist", 
                new StringContent(
                    JsonConvert.SerializeObject(new CreateTestWishlistModel() { title = title}), Encoding.UTF8, "application/json"
                    )
                );
            var responseBody = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
            await page.GoToAsync(WishlistTestBase.baseUrl + "/wishlist/" + responseBody);
        }
    }

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
            this.browser = await playwright.Chromium.LaunchAsync(headless: false);
        }
        
        [SetUp]
        public async Task Setup()
        {
            this.page = await browser.NewPageAsync();
            await page.GoToAsync(baseUrl);
            await page.Context.ClearCookiesAsync();
            this.loginPage = new LoginPageObject(page);
        }
        
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await this.browser.DisposeAsync();
            this.playwright.Dispose();
        }

        [TearDown]
        public async Task Teardown()
        {
            await page.CloseAsync();
        }
    }
    
    [TestFixture]
    public class WishlistHomepageTests : WishlistTestBase
    {
        [Test]
        public async Task LoginWorks()
        {
            await loginPage.LoginUsingFormWithTestAccount();

            page.Url.Should().Contain("/wishlists");
        }
        
        [Test]
        public async Task NoCookieRedirectsBackToHomepage()
        {
            await loginPage.LoginWithCookie("test@testwishlist.com", "Test", baseUrl, baseUrl);

            await page.Context.ClearCookiesAsync();
            await page.GoToAsync(baseUrl);
            
            page.Url.Should().NotContain("/wishlists");
        }
    }

    public class WishlistSummaryPageTests : WishlistTestBase
    {
        [SetUp]
        public async Task SetupThis()
        {
            await loginPage.LoginWithCookie("test@testwishlist.com", "Test", baseUrl, baseUrl);
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

    public class WishlistPageObject
    {
        private readonly IPage _page;

        public WishlistPageObject(IPage page)
        {
            _page = page;
        }

        public Task AddNewPerson(string name, string email)
        {
            ClickAddNewPerson().MakeItSync();
            EnterNameAndEmail(name, email).MakeItSync();
            ClickCreateNewPerson().MakeItSync();
            return Task.CompletedTask;
        }

        public Task ClickAddNewPerson()
        {
            _page.ClickAsync(Cy.Name("AddNewPerson")).MakeItSync();
            return Task.CompletedTask;
        }

        public Task ClickCreateNewPerson()
        {
            _page.ClickAsync(Cy.Name("CreateNewPerson")).MakeItSync();
            return Task.CompletedTask;
        }
        
        public async Task<bool> CreateNewPersonIsEnabled()
        {
            var f = await _page.QuerySelectorAsync(Cy.Name("CreateNewPerson"));
            return await f.IsEnabledAsync();
        }

        public Task EnterNameAndEmail(string name, string email)
        {
            _page.TypeAsync(Cy.Name("NewPersonEmail"), email).MakeItSync();
            _page.TypeAsync(Cy.Name("NewPersonName"), name).MakeItSync();
            return Task.CompletedTask;
        }
        
        public Task AddItem(string description)
        {
            _page.ClickAsync(Cy.Name("AddNewItemButton")).MakeItSync();
            _page.TypeAsync(Cy.Name("AddItem"), description).MakeItSync();
            _page.ClickAsync(Cy.Name("SaveItemButton")).MakeItSync();
            return Task.CompletedTask;
        }
    }

    public class LoginPageObject
    {
        private readonly IPage _page;

        public LoginPageObject(IPage page)
        {
            _page = page;
        }

        public async Task LoginUsingFormWithTestAccount() => await LoginUsingForm("test@testwishlist.com", "Test");
        
        public Task LoginUsingForm(string email, string name)
        {
            _page.TypeAsync(Cy.Name("EmailLogin"), email).ConfigureAwait(false).GetAwaiter().GetResult();
            _page.TypeAsync(Cy.Name("NameLogin"), name).ConfigureAwait(false).GetAwaiter().GetResult();
            _page.ClickAsync(Cy.Name("LoginButton")).ConfigureAwait(false).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }
        
        public Task LoginWithCookie(string email, string name, string baseUrl, string urlToVisit)
        {
            _page.Context.AddCookiesAsync(new SetNetworkCookieParam()
            {
                Name = "login",
                Url = baseUrl,
                Value = Newtonsoft.Json.JsonConvert.SerializeObject(new LoginDetails(){Email = email, Name = name})
            }).ConfigureAwait(false).GetAwaiter().GetResult();

            _page.GoToAsync(urlToVisit).ConfigureAwait(false).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }
    }

    public static class Cy
    {
        public static string Name(string str)
        {
            return $"[cypress-name={str}]";
        }
    }

    public class LoginDetails
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public static class TaskExtension
    {
        public static void MakeItSync(this Task task)
        {
            task.ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}