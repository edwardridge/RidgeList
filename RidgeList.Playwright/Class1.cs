using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esprima.Ast;
using Newtonsoft.Json;
using NUnit.Framework;
using PlaywrightSharp;

namespace RidgeList.Playwright
{
    //public class ClearDown
    //{
    //    [Test, Ignore("")]
    //    public async Task ClearDownOldWishlists(string baseUrl)
    //    {
    //        var httpClient = new HttpClient()
    //        {
    //            BaseAddress = new Uri(baseUrl)
    //        };
    //        await httpClient.PostAsync("/WishlistTest/clearOldTestWishlists", new StringContent(string.Empty));
    //    }
    //}

    public static class PlaywrightHelpers
    {
        public class CreateTestWishlistModel
        {
            public string title { get; set; }
        }
        public static async Task CreateWishlist(string baseUrl, string title, IPage page)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };
            var response = await httpClient.PostAsync("/WishlistTest/createTestWishlist", 
                new StringContent(
                    JsonConvert.SerializeObject(new CreateTestWishlistModel() { title = title}), Encoding.UTF8, "application/json"
                    )
                );
            var responseBody = (await response.Content.ReadAsStringAsync()).Replace("\"", "");
            await page.GoToAsync(baseUrl + "/wishlist/" + responseBody);
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
            _page.TypeAsync(Cy.InputName("NewPersonEmail"), email).MakeItSync();
            _page.TypeAsync(Cy.InputName("NewPersonName"), name).MakeItSync();
            return Task.CompletedTask;
        }
        
        public Task AddItem(string description)
        {
            _page.ClickAsync(Cy.Name("AddNewItemButton")).MakeItSync();
            _page.TypeAsync(Cy.TextAreaName("AddItem"), description).MakeItSync();
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

        public static string InputName(string str)
        {
            return $"[cypress-name={str}] >> input";
        }

        public static string TextAreaName(string str)
        {
            return $"[cypress-name={str}] >> textarea";
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