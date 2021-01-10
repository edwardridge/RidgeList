using System;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistTestController : Controller
    {
        private readonly IWishlistRepository _repository;

        public WishlistTestController(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost]
        [Route("createTestWishlist")]
        public Guid CreateTestWishlist()
        {
           var wishlist = Wishlist.Create("Test wishlist");
           _repository.Save(wishlist);
           return wishlist.Id;
        }
    }
}