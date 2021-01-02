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
            var wishlist = Wishlist.Create();
            await repo.Save(wishlist);

            var reloadedWishlist = await repo.Load(wishlist.Id);
            reloadedWishlist.Should().NotBeNull();
        }
    }

    public class WishlistTests
    {
        [Test]
        public void Creating_A_Wishlist_Adds_An_Id()
        {
            var wishlist = Wishlist.Create();
            wishlist.Id.Should().NotBeEmpty();
        }

        [Test]
        public void Can_Add_Person_With_Email_To_Wishlist()
        {
            var wishlist = Wishlist.Create();
            wishlist.AddPerson("edwardridge@gmail.com");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new [] { "edwardridge@gmail.com" });
        }
    }
}