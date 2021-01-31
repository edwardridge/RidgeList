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
        private readonly IWishlistSummaryRepository _wishlistSummaryRepository;
        private string testEmailAccount = "test@testwishlist.com";

        public WishlistTestController(IWishlistRepository repository, IWishlistSummaryRepository wishlistSummaryRepository)
        {
            _repository = repository;
            _wishlistSummaryRepository = wishlistSummaryRepository;
        }
        
        [HttpPost]
        [Route("createTestWishlist")]
        public async Task<Guid> CreateTestWishlist([FromBody] CreateWishlistRequestModel req)
        {
            var wishlist = Wishlist.Create("[Test] " + req.title, testEmailAccount, "Test");
           await _repository.Save(wishlist);
           return wishlist.Id;
        }

        [HttpPost]
        [Route("clearOldTestWishlists")]
        public async Task ClearOldTestWishlists()
        {
            var summaries = await _wishlistSummaryRepository.GetWishlistSummaries(testEmailAccount);
            foreach (var summary in summaries.ToList())
            {
                var wishlist = await _repository.Load(summary);
                if (wishlist.Name.Contains("[Test]"))
                {
                    foreach (var p in wishlist.People)
                    {
                        await _wishlistSummaryRepository.RemoveWishlistFromPerson(p.Email, wishlist.Id);
                    }
                    await _repository.Delete(wishlist.Id);
                }
            }
        }
    }

    public class CreateWishlistRequestModel
    {
        public string title { get; set; }
    }
}