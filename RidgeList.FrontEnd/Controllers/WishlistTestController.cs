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
        private string testEmailAccount = "test@testwishlist.com";

        public WishlistTestController(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost]
        [Route("createTestWishlist")]
        public async Task<Guid> CreateTestWishlist([FromBody] CreateWishlistRequestModel req)
        {
            var wishlist = Wishlist.Create("[Test] " + req.title, testEmailAccount, "Test");
           _repository.Save(wishlist);
           return wishlist.Id;
        }

        [HttpPost]
        [Route("clearOldTestWishlists")]
        public async Task ClearOldTestWishlists()
        {
            var summaries = await _repository.GetWishlistSummaries(testEmailAccount);
            foreach (var summary in summaries.ToList())
            {
                if (summary.Name.Contains("[Test]"))
                {
                    await _repository.Delete(summary.Id);
                }
            }
        }
    }

    public class CreateWishlistRequestModel
    {
        public string title { get; set; }
    }
}