using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;
using RidgeList.ApplicationServices;
using RidgeList.Models;

namespace RidgeList.Domain.Tests
{
    public class WishlistRepositoryTests
    {
        [Test]
        public async Task Saves_WishList_And_Loads_Wishlist()
        {
            var repo = new InMemoryWishlistRepository();
            var wishlist = Wishlist.Create("Asd", Guid.NewGuid());
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
            var idOfCreator = Guid.NewGuid();
            var wishlist = Wishlist.Create("Eds test wishlist", idOfCreator);
            wishlist.Id.Should().NotBeEmpty();
            wishlist.Name.Should().Be("Eds test wishlist");            
            wishlist.GetPeople().First().PersonId.Should().Be(idOfCreator);             
            wishlist.GetPeople().First().Giftee.Should().Be(true);      
            wishlist.CreatorId.Should().Be(idOfCreator);
        }

        [Test]
        public void Can_Add_Person_With_Email_And_Name_To_Wishlist()
        {
            var idOfCreator = Guid.NewGuid();
            var newPersonId = Guid.NewGuid();
            var wishlist = Wishlist.Create("Eds test wishlist", idOfCreator);
            wishlist.AddPerson(newPersonId, true);
            
            wishlist.GetPeople().Should().BeEquivalentTo(new []
            {
                new WishlistPerson() { PersonId = idOfCreator, Giftee = true},
                new WishlistPerson() { PersonId = newPersonId, Giftee = true}
            });
        }
        
        //[Test]
        //public void Can_Edit_Name()
        //{
        //    var idOfCreator = Guid.NewGuid();
        //    var wishlist = Wishlist.Create("Eds test wishlist", idOfCreator);
        //    wishlist.EditPerson("a@b.com", "Ed");
            
        //    wishlist.GetPeople().Should().BeEquivalentTo(new []
        //    {
        //        new WishlistPerson() { Email = "a@b.com", Name = "Ed", Giftee = true},
                
        //    });
        //}

        [Test]
        public void Can_Add_Item_To_Wishlist()
        {
            var idOfCreator = Guid.NewGuid();
            
            var wishlist = Wishlist.Create("Eds test wishlist", idOfCreator);
            wishlist.AddGiftIdea(idOfCreator, "My first present");

            var person = wishlist.GetPerson(idOfCreator);

            person.PresentIdeas.Count.Should().Be(1);
            person.PresentIdeas.Single().Description.Should().Be("My first present");
        }

        [Test]
        public void Can_Remove_Item_From_Wishlist()
        {
            var idOfCreator = Guid.NewGuid();
            
            var wishlist = Wishlist.Create("Eds test wishlist", idOfCreator);
            wishlist.AddGiftIdea(idOfCreator, "My first present");
            var present = wishlist.GetPerson(idOfCreator).PresentIdeas.Single();
            wishlist.RemoveGiftIdea(idOfCreator, present.Id);
            
            var person = wishlist.GetPerson(idOfCreator);

            person.PresentIdeas.Count.Should().Be(0);
        }
        
        [Test]
        public void Can_Claim_Present()
        {
            var idOfCreator = Guid.NewGuid();

            var secondPersonid = Guid.NewGuid();
            var presentId = Guid.NewGuid();
            var wishlist =
                new WishlistBuilder(idOfCreator)
                    .AddPerson(secondPersonid)
                    .AddPerson(idOfCreator)
                    .AddPresentIdea(idOfCreator, "asd", presentId)
                    .Build();
            
            var presentIdea = wishlist.GetPerson(idOfCreator).PresentIdeas.Single();
            wishlist.ClaimGift(presentIdea.Id, secondPersonid);

            presentIdea.ClaimerId.Should().Be(secondPersonid);
        }
        
        [Test]
        public void Can_UnClaim_Present()
        {
            var idOfCreator = Guid.NewGuid();
            var secondPersonId = Guid.NewGuid();
            var presentId = Guid.NewGuid();
            var wishlist =
                new WishlistBuilder(idOfCreator)
                    .AddPerson(idOfCreator)
                    .AddPerson(secondPersonId)
                    .AddPresentIdea(idOfCreator, "asd", presentId)
                    .AddClaimer(presentId, secondPersonId)
                    .Build();

            wishlist.UnclaimPresent(presentId);
            
            var presentIdea = wishlist.GetPerson(idOfCreator).PresentIdeas.Single();

            presentIdea.ClaimerId.Should().BeNull();
        }
        
        [Test]
        public void Can_Change_Is_Claimer()
        {
            var idOfCreator = Guid.NewGuid();
            var secondPersonId = Guid.NewGuid();
            
            var wishlist =
                new WishlistBuilder(idOfCreator)
                    .AddPerson(idOfCreator)
                    .AddPerson(secondPersonId, true)
                    .Build();

            wishlist.ChangeIsGiftee(secondPersonId, false);
            wishlist.GetPerson(secondPersonId).Giftee.Should().BeFalse();
            
            wishlist.ChangeIsGiftee(secondPersonId, true);
            wishlist.GetPerson(secondPersonId).Giftee.Should().BeTrue();
        }
        
        [Test]
        public void Can_Remove_Person()
        {
            var idOfCreator = Guid.NewGuid();
            var secondPersonId = Guid.NewGuid();
            
            var wishlist =
                new WishlistBuilder(idOfCreator)
                    .AddPerson(idOfCreator)
                    .AddPerson(secondPersonId, true)
                    .Build();

            wishlist.RemovePerson(secondPersonId);

            wishlist.People.Count.Should().Be(1);
        }

        [Test]
        public async Task Maps_Name_From_Email_In_Claim()
        {
            var presentId = Guid.NewGuid();
            var idOfCreator = Guid.NewGuid();
            var secondPersonId = Guid.NewGuid();
            var thirdPersonId = Guid.NewGuid();


            var wishlist =
                new WishlistBuilder(idOfCreator)
                    .AddPerson(secondPersonId)
                    .AddPerson(thirdPersonId)
                    .AddPresentIdea(secondPersonId, "desc 1", presentId)
                    .AddClaimer(presentId, thirdPersonId)
                    .Build();


            IUserRepository wishlistSummariesRepo = new InMemoryWishlistSummaryRepository();
            await wishlistSummariesRepo.AddWishlistToPerson(idOfCreator, wishlist.Id);
            await wishlistSummariesRepo.AddWishlistToPerson(secondPersonId, wishlist.Id);
            await wishlistSummariesRepo.AddWishlistToPerson(thirdPersonId, wishlist.Id);
            await wishlistSummariesRepo.SetEmailAndName(secondPersonId, "second@person.com", "Second person");
            await wishlistSummariesRepo.SetEmailAndName(thirdPersonId, "third@person.com", "Third person");

            var mapper = new WishlistMapper(wishlistSummariesRepo);
            var model = await mapper.Map(wishlist);
            var presentIdeaModels = model.People
                .Single(s => s.PersonId == secondPersonId)
                .PresentIdeas;
            var claimer = presentIdeaModels
                .Single(s => s.Id == presentId);

            claimer.ClaimerName.Should().Be("Third person");            
            claimer.ClaimerEmail.Should().Be("third@person.com");
        }
    }

    public class WishlistBuilder
    {
        private Wishlist _wishlist;

        public WishlistBuilder(Guid creatorId)
        {
            this._wishlist = new Wishlist()
            {
                CreatorId = creatorId,
                Name = "test"
            };
        }

        public WishlistBuilder AddPerson(Guid personId, bool isGiftee = true)
        {
            this._wishlist.People.Add(new WishlistPerson()
            {
                PersonId = personId,
                Giftee =  isGiftee
            });
            return this;
        }

        public WishlistBuilder AddPresentIdea(Guid personId, string desc, Guid presentId)
        {
            this._wishlist
                .People
                .Single(s => s.PersonId == personId)
                .PresentIdeas
                .Add(new PresentIdea()
                {
                    Description = desc,
                    Id = presentId
                });
            
            return this;
        }
        
        public WishlistBuilder AddClaimer(Guid presentId, Guid claimerId)
        {
            this._wishlist
                .People
                .SelectMany(s => s.PresentIdeas)
                .Single(s => s.Id == presentId)
                .ClaimerId = claimerId;
            
            return this;
        }

        public Wishlist Build()
        {
            return this._wishlist;
        }
    }
}