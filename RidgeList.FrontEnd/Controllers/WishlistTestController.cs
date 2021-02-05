using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistTestController : Controller
    {
        private readonly IWishlistRepository _repository;
        private readonly IUserRepository _wishlistSummaryRepository;
        private string testEmailAccount = "test@testwishlist.com";
        private Guid testAccountId = Guid.Parse("8eb3fe5c-6965-443b-8828-752c0121f21f");

        public WishlistTestController(IWishlistRepository repository, IUserRepository wishlistSummaryRepository)
        {
            _repository = repository;
            _wishlistSummaryRepository = wishlistSummaryRepository;
        }

        [HttpPost]
        [Route("createTestUser")]
        public async Task CreateTestUser([FromBody] CreateTestUserModel req)
        {
            await _wishlistSummaryRepository.CreatePerson(req.id, "test@test.com", "Test");
        }

        [HttpPost]
        [Route("createTestWishlist")]
        public async Task<Guid> CreateTestWishlist([FromBody] CreateWishlistRequestModel req)
        {
            var wishlist = Wishlist.Create("[Test] " + req.title, req.creatorId);
           await _repository.Save(wishlist);
           return wishlist.Id;
        }

        [HttpPost]
        [Route("clearOldTestWishlists")]
        public async Task ClearOldTestWishlists()
        {
            var summaries = await _wishlistSummaryRepository.GetUser(testAccountId);
            foreach (var summary in summaries.Wishlists)
            {
                var wishlist = await _repository.Load(summary);
                if (wishlist.Name.Contains("[Test]"))
                {
                    foreach (var p in wishlist.People)
                    {
                        await _wishlistSummaryRepository.RemoveWishlistFromPerson(p.PersonId, wishlist.Id);
                    }
                    await _repository.Delete(wishlist.Id);
                }
            }
        }
    }
}