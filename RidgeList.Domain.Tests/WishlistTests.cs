using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using RidgeList.Models;

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

        // [Test]
        // public async Task Gets_All_Wishlists_Only_Returns_Wishlists_For_Email()
        // {
        //     var repo = new InMemoryWishlistRepository();
        //     var wishlist1 = Wishlist.Create("Asd", "a@b.com", "Ed");
        //     wishlist1.AddPerson("", "c@d.com", true);
        //     await repo.Save(wishlist1);
        //     
        //     var wishlist2 = Wishlist.Create("Wishlist 2", "a@b.com", "Ed");
        //     await repo.Save(wishlist2);
        //
        //     var wishlistSumaries = await repo.GetWishlistSummaries("a@b.com");
        //     wishlistSumaries.Select(s => s.Name).Should().BeEquivalentTo(new []
        //     {
        //         "Asd" ,
        //         "Wishlist 2" 
        //     });
        //     
        //     var wishlistSumaries2 = await repo.GetWishlistSummaries("c@d.com");
        //     wishlistSumaries2.Select(s => s.Name).Should().BeEquivalentTo(new []
        //     {
        //         "Asd" 
        //     });
        // }
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
            wishlist.GetPeople().First().Giftee.Should().Be(true);      
            wishlist.Creator.Should().Be("a@b.com");
        }

        [Test]
        public void Can_Add_Person_With_Email_And_Name_To_Wishlist()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com", "Ed");
            wishlist.AddPerson("Ed", "edwardridge@gmail.com", true);
            
            wishlist.GetPeople().Should().BeEquivalentTo(new []
            {
                new WishlistPerson() { Email = "a@b.com", Name = "Ed", Giftee = true},
                new WishlistPerson() { Email = "edwardridge@gmail.com", Name = "Ed", Giftee = true}
            });
        }
        
        [Test]
        public void Can_Edit_Name()
        {
            var wishlist = Wishlist.Create("Eds test wishlist", "a@b.com", "Ed");
            wishlist.EditPerson("a@b.com", "Ed");
            
            wishlist.GetPeople().Should().BeEquivalentTo(new []
            {
                new WishlistPerson() { Email = "a@b.com", Name = "Ed", Giftee = true},
                
            });
        }

        [Test]
        public void Can_Add_Item_To_Wishlist()
        {
            var emailOfCreator = "a@b.com";
            
            var wishlist = Wishlist.Create("Eds test wishlist", emailOfCreator, "Ed");
            wishlist.AddGiftIdea(emailOfCreator, "My first present");

            var person = wishlist.GetPerson(emailOfCreator);

            person.PresentIdeas.Count.Should().Be(1);
            person.PresentIdeas.Single().Description.Should().Be("My first present");
        }

        [Test]
        public void Can_Remove_Item_From_Wishlist()
        {
            var emailOfCreator = "a@b.com";
            
            var wishlist = Wishlist.Create("Eds test wishlist", emailOfCreator, "Ed");
            wishlist.AddGiftIdea(emailOfCreator, "My first present");
            var present = wishlist.GetPerson(emailOfCreator).PresentIdeas.Single();
            wishlist.RemoveGiftIdea(emailOfCreator, present.Id);
            
            var person = wishlist.GetPerson(emailOfCreator);

            person.PresentIdeas.Count.Should().Be(0);
        }
        
        [Test]
        public void Can_Claim_Present()
        {
            var emailOfCreator = "a@b.com";
            var presentId = Guid.NewGuid();
            var wishlist =
                new WishlistBuilder()
                    .AddPerson("Ed", emailOfCreator)
                    .AddPerson("Second person", "second@email.com")
                    .AddPresentIdea(emailOfCreator, "asd", presentId)
                    .Build();
            
            var presentIdea = wishlist.GetPerson(emailOfCreator).PresentIdeas.Single();
            wishlist.ClaimGift(presentIdea.Id, emailOfCreator);

            presentIdea.Claimer.Should().Be(emailOfCreator);
        }
        
        [Test]
        public void Can_UnClaim_Present()
        {
            var emailOfCreator = "a@b.com";
            var presentId = Guid.NewGuid();
            var wishlist =
                new WishlistBuilder()
                    .AddPerson("Ed", emailOfCreator)
                    .AddPerson("Second person", "second@email.com")
                    .AddPresentIdea(emailOfCreator, "asd", presentId)
                    .AddClaimer(presentId, "second@email.com")
                    .Build();

            wishlist.UnclaimPresent(presentId);
            
            var presentIdea = wishlist.GetPerson(emailOfCreator).PresentIdeas.Single();

            presentIdea.Claimer.Should().BeNullOrEmpty();
        }

        [Test]
        public void Maps_Name_From_Email_In_Claim()
        {
            var presentId = Guid.NewGuid();

            var wishlist =
                new WishlistBuilder()
                    .AddPerson("Ed", "ed@ed.com")
                    .AddPerson("Second person", "b@b.com")
                    .AddPresentIdea("ed@ed.com", "desc 1", presentId)
                    .AddClaimer(presentId, "b@b.com")
                    .Build();
            
            var mapper = new WishlistMapper();
            var model = mapper.Map(wishlist);
            var presentIdeaModels = model.People
                .Single(s => s.Email == "ed@ed.com")
                .PresentIdeas;
            var claimer = presentIdeaModels
                .Single(s => s.Id == presentId);

            claimer.ClaimerName.Should().Be("Second person");            
            claimer.ClaimerEmail.Should().Be("b@b.com");
        }
    }

    public class WishlistBuilder
    {
        private Wishlist _wishlist;

        public WishlistBuilder()
        {
            this._wishlist = new Wishlist()
            {
                Creator = "asd",
                Name = "test"
            };
        }

        public WishlistBuilder AddPerson(string name, string email)
        {
            this._wishlist.People.Add(new WishlistPerson()
            {
                Email = email,
                Name = name
            });
            return this;
        }

        public WishlistBuilder AddPresentIdea(string email, string desc, Guid presentId)
        {
            this._wishlist
                .People
                .Single(s => s.Email == email)
                .PresentIdeas
                .Add(new PresentIdea()
                {
                    Description = desc,
                    Id = presentId
                });
            
            return this;
        }
        
        public WishlistBuilder AddClaimer(Guid presentId, string email)
        {
            this._wishlist
                .People
                .SelectMany(s => s.PresentIdeas)
                .Single(s => s.Id == presentId)
                .Claimer = email;
            
            return this;
        }

        public Wishlist Build()
        {
            return this._wishlist;
        }
    }
}