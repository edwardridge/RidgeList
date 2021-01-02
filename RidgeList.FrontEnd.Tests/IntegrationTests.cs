using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace RidgeList.FrontEnd.Tests
{
    public class FrontEndTests
    {
        private readonly WebApplicationFactory<RidgeList.FrontEnd.Startup> _factory;

        public FrontEndTests()
        {
            _factory = new WebApplicationFactory<RidgeList.FrontEnd.Startup>();
        }
        
        [Test]
        public async Task Test_Healthcheck()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Healthcheck");

            // Assert
            response.EnsureSuccessStatusCode();
            var test = await response.Content.ReadAsStringAsync();
            test.Should().Be("All ok!");
        }
        
        [Test]
        public async Task Test_Wishlist()
        {
            // System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsync("/wishlist/create", new StringContent(""));

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}