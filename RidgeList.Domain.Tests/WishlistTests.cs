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
            var wishlist = Wishlist.Create("Asd", "a@b.com", "Ed");
            await repo.Save(wishlist);

            var reloadedWishlist = await repo.Load(wishlist.Id);
            reloadedWishlist.Should().NotBeNull();
        }

        [Test]
        public async Task Gets_All_Wishlists_Only_Returns_Wishlists_For_Email()
        {
            var repo = new InMemoryWishlistRepository();
            var wishlist1 = Wishlist.Create("Asd", "a@b.com", "Ed");
            wishlist1.AddPerson("", "c@d.com");
            await repo.Save(wishlist1);
            
            var wishlist2 = Wishlist.Create("Wishlist 2", "a@b.com", "Ed");
            await repo.Save(wishlist2);

            var wishlistSumaries = await repo.GetWishlistSummaries("a@b.com");
            wishlistSumaries.Select(s => s.Name).Should().BeEquivalentTo(new []
            {
                "Asd" ,
                "Wishlist 2" 
            });
            
            var wishlistSumaries2 = await repo.GetWishlistSummaries("c@d.com");
            wishlistSumaries2.Select(s => s.Name).Should().BeEquivalentTo(new []
            {
                "Asd" 
            });
        }
    }

    

    public class WishlistTests
    {
        [Test]
        public void Creating_A_Wishlist_Adds_An_Id_And_Initial_User()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com", "Ed");
            wishlist.Id.Should().NotBeEmpty();
            wishlist.Name.Should().Be("Eds test wishlist");            
            wishlist.GetPeople().First().Email.Should().Be("a@b.com");       
            wishlist.Creator.Should().Be("a@b.com");

        }

        [Test]
        public void Can_Add_Person_With_Email_And_Name_To_Wishlist()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com", "Ed");
            wishlist.AddPerson("Ed", "edwardridge@gmail.com");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new []
            {
                new WishlistPerson() { Email = "a@b.com", Name = null},
                new WishlistPerson() { Email = "edwardridge@gmail.com", Name = "Ed"}
            });
        }
        
        [Test]
        public void Can_Edit_Name()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com", "Ed");
            wishlist.EditPerson("a@b.com", "Ed");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new []
            {
                new WishlistPerson() { Email = "a@b.com", Name = "Ed"},
                
            });
        }

        [Test]
        public void Can_Add_Item_To_Wishlist()
        {
            var emailOfCreator = "a@b.com";
            var wishlist = Wishlist.Create("Eds test wishlist", emailOfCreator, "Ed");
            wishlist.AddPresentIdea(emailOfCreator, "My first present");

            var person = wishlist.GetPerson(emailOfCreator);

            person.PresentIdeas.Count.Should().Be(1);
            person.PresentIdeas.Single().Description.Should().Be("My first present");
        }
    }
}