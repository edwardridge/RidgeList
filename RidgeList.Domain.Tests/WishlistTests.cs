using System;
using System.Collections.Generic;
using System.Linq;
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
            var wishlist = Wishlist.Create("Asd", "a@b.com");
            await repo.Save(wishlist);

            var reloadedWishlist = await repo.Load(wishlist.Id);
            reloadedWishlist.Should().NotBeNull();
        }

        [Test]
        public async Task Gets_All_Wishlists_Only_Returns_Wishlists_For_Email()
        {
            var repo = new InMemoryWishlistRepository();
            var wishlist1 = Wishlist.Create("Asd", "a@b.com");
            wishlist1.AddPerson("c@d.com");
            await repo.Save(wishlist1);
            
            var wishlist2 = Wishlist.Create("Wishlist 2", "a@b.com");
            await repo.Save(wishlist2);

            var wishlistSumaries = await repo.GetWishlistSummaries("a@b.com");
            wishlistSumaries.Should().BeEquivalentTo(new []
            {
                new WishlistSummary() { Name = "Asd" },
                new WishlistSummary() { Name = "Wishlist 2" }
            });
            
            var wishlistSumaries2 = await repo.GetWishlistSummaries("c@d.com");
            wishlistSumaries2.Should().BeEquivalentTo(new []
            {
                new WishlistSummary() { Name = "Asd" }
            });
        }
    }

    

    public class WishlistTests
    {
        [Test]
        public void Creating_A_Wishlist_Adds_An_Id_And_Initial_User()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com");
            wishlist.Id.Should().NotBeEmpty();
            wishlist.Name.Should().Be("Eds test wishlist");            
            wishlist.GetPeople().First().Should().Be("a@b.com");       
            wishlist.Creator.Should().Be("a@b.com");

        }

        [Test]
        public void Can_Add_Person_With_Email_To_Wishlist()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com");
            wishlist.AddPerson("edwardridge@gmail.com");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new [] { "edwardridge@gmail.com" });
        }
    }
}