using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _repository;

        public WishlistController(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        // GET
        [HttpPost]
        [Route("create")]
        public WishlistModel Create()
        {
            return CreateNewWishlist();
        }

        private WishlistModel CreateNewWishlist()
        {
            var wishlist = Wishlist.Create();
            this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
    }

    public class WishlistMapper
    {
        public WishlistModel Map(Wishlist wishlist)
        {
            return new WishlistModel()
            {
                Id = wishlist.Id,
                People = wishlist.GetPeople()
            };
        }
    }
    
    public class WishlistModel
    {
        public Guid Id { get; set; }

        public List<string> People { get; set; }
    }
}