using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

namespace RidgeList.Domain.Tests
{
    public class WishlistRepositoryTests
    {
        [Test]
        public async Task Saves_WishList_And_Loads_Wishlist()
        {
            var repo = new InMemoryWishlistRepository();
            var wishlist = Wishlist.Create("Asd");
            await repo.Save(wishlist);

            var reloadedWishlist = await repo.Load(wishlist.Id);
            reloadedWishlist.Should().NotBeNull();
        }

        [Test]
        public async Task Gets_All_Wishlists()
        {
            var repo = new InMemoryWishlistRepository();
            var wishlist1 = Wishlist.Create("Asd");
            await repo.Save(wishlist1);
            
            var wishlist2 = Wishlist.Create("Wishlist 2");
            await repo.Save(wishlist2);

            var wishlistSumaries = await repo.GetWishlistSummaries();
            wishlistSumaries.Should().BeEquivalentTo(new []
            {
                new WishlistSummary() { Name = "Asd" },
                new WishlistSummary() { Name = "Wishlist 2" }
            });
        }
    }

    

    public class WishlistTests
    {
        [Test]
        public void Creating_A_Wishlist_Adds_An_Id()
        {
            var wishlist = Wishlist.Create("Eds test wishlist");
            wishlist.Id.Should().NotBeEmpty();
            wishlist.Name.Should().Be("Eds test wishlist");
        }

        [Test]
        public void Can_Add_Person_With_Email_To_Wishlist()
        {
            var wishlist = Wishlist.Create("Eds test wishlist");
            wishlist.AddPerson("edwardridge@gmail.com");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new [] { "edwardridge@gmail.com" });
        }
    }
}