using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistTestController : Controller
    {
        private readonly IWishlistRepository _repository;
        private string testEmailAccount = "test@testwishlistcom";

        public WishlistTestController(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost]
        [Route("createTestWishlist")]
        public Guid CreateTestWishlist()
        {
           var wishlist = Wishlist.Create("[Test] Wishlist", testEmailAccount);
           _repository.Save(wishlist);
           return wishlist.Id;
        }

        [HttpPost]
        [Route("clearOldTestWishlists")]
        public async Task ClearOldTestWishlists()
        {
            var summaries = await _repository.GetWishlistSummaries(testEmailAccount);
            foreach (var summary in summaries)
            {
                if (summary.Name.Contains("[Test]"))
                {
                    await _repository.Delete(summary.Id);
                }
            }
        }
    }
}