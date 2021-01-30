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
    public class TestWebApplicationFactory : WebApplicationFactory<RidgeList.FrontEnd.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                var docStore = s.SingleOrDefault(s => s.ServiceType == typeof(IDocumentStore));
                s.Remove(docStore);
                
                var martenRepository = s.SingleOrDefault(s => s.ServiceType == typeof(MartenDbRepository));
                s.Remove(martenRepository);
                
                s.Add((new ServiceDescriptor(typeof(IWishlistRepository), new InMemoryWishlistRepository())));
            });
        }
    }
    
    public class FrontEndTests
    {
        private readonly TestWebApplicationFactory _factory;

        public FrontEndTests()
        {
            _factory = new TestWebApplicationFactory();
        }
        [Test]
        public async Task Test_Wishlist()
        {
            var inMemoryRepository = new InMemoryWishlistRepository();
            var client = _factory
                .WithWebHostBuilder(t =>
                {
                    t.ConfigureServices(s =>
                    {
                        // var docStore = s.SingleOrDefault(s => s.ServiceType == typeof(IDocumentStore));
                        // s.Remove(docStore);
                        //
                        // var martenRepository = s.Where(s => s.ServiceType == typeof(IWishlistRepository));
                        s.RemoveAll(typeof(IDocumentStore));
                        s.RemoveAll(typeof(IWishlistRepository));

                        s.Add((new ServiceDescriptor(typeof(IWishlistRepository), inMemoryRepository)));
                    });
                })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.PostAsync("/wishlist/create?nameOfWishlist=TestWIshlist&emailOfCreator=ed&nameOfCreator=ed&creatorIsGiftee=true", new StringContent(""));

            // Assert
            response.EnsureSuccessStatusCode();
            inMemoryRepository._wishlists.Count.Should().BeGreaterThan(0);
            inMemoryRepository._wishlists.Single().Value.Creator.Should().Be("ed");
        }
    }
}