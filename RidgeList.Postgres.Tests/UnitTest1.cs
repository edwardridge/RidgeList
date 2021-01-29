using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RidgeList.Domain;

namespace RidgeList.Postgres.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            //MartenDbRepository repo = new MartenDbRepository();
            //await repo.Save(new Wishlist()
            //{
            //    Id = Guid.NewGuid(),
            //    Creator = "asd",
            //    Name = "sdf",
            //    People = new WishlistPeople()
            //    {
            //        new WishlistPerson()
            //        {
            //            Email = "asd",
            //            Name = "ed",
            //            PresentIdeas = new List<PresentIdea>()
            //            {
            //                new PresentIdea()
            //                {
            //                    Description = "asd",

            //                }
            //            }
            //        }
            //    }
            //});

            //var f = await repo.GetWishlistSummaries("asd");
            //f.ToList().Single().Name.Should().Be("sdf");
        }
    }
}