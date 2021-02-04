using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;
using RidgeList.Domain;
using RidgeList.Postgres;

namespace RidgeList.FrontEnd.Tests
{
    public class FrontEndTests
    {
        private readonly WebApplicationFactory<RidgeList.FrontEnd.Startup> _factory;
        private InMemoryWishlistRepository inMemoryRepository;
        private HttpClient client;
        private InMemoryWishlistSummaryRepository inMemorySummaryRepository;

        public FrontEndTests()
        {
            _factory = new WebApplicationFactory<RidgeList.FrontEnd.Startup>();
        }

        [SetUp]
        public void Setup()
        {
            
            this.inMemoryRepository = new InMemoryWishlistRepository();
            this.inMemorySummaryRepository = new InMemoryWishlistSummaryRepository();
            this.client = _factory
                .WithWebHostBuilder(t =>
                {
                    t.UseSetting("ASPNETCORE_ENVIRONMENT", "test");
                    t.ConfigureServices(s =>
                    {
                        s.RemoveAll(typeof(IDocumentStore));
                        s.RemoveAll(typeof(IWishlistRepository));
                        s.RemoveAll(typeof(IWishlistSummaryRepository));

                        

                        s.Add((new ServiceDescriptor(typeof(IWishlistRepository), inMemoryRepository)));
                        s.Add((new ServiceDescriptor(typeof(IWishlistSummaryRepository), inMemorySummaryRepository)));

                    });
                })
                
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }
        
        [Test]
        public async Task Test_Create_Wishlist()
        {
            var response = await client.PostAsync("/wishlist/create?nameOfWishlist=TestWIshlist&emailOfCreator=ed&nameOfCreator=ed&creatorIsGiftee=true", new StringContent(""));

            inMemoryRepository._wishlists.Count.Should().BeGreaterThan(0);
            inMemoryRepository._wishlists.Single().Value.CreatorId.Should().NotBeEmpty();
        }
        
        [Test]
        public async Task Test_AddPerson_Wishlist()
        {
            var idOfCreator = Guid.NewGuid();
            var wishlist = Wishlist.Create("a", idOfCreator, true);
            await this.inMemoryRepository.Save(wishlist);
            
            var response = await client.PostAsync($"/wishlist/addPerson?wishlistId={wishlist.Id.ToString()}&name=Ed&email=ed&isGiftee=true", new StringContent(""));

            response.EnsureSuccessStatusCode();
            inMemoryRepository._wishlists.Count.Should().BeGreaterThan(0);
            inMemoryRepository._wishlists.Single().Value.People.Count.Should().Be(2);
            var wishlistSummaries = await inMemorySummaryRepository.GetWishlistSummaries(idOfCreator);
            wishlistSummaries.Count().Should().Be(1);
        }
    }
}