using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;
using RidgeList.Models;

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

        [HttpPost]
        [Route("create")]
        public WishlistModel Create(string nameOfWishlist, string emailOfCreator, string nameOfCreator)
        {
            return CreateNewWishlist(nameOfWishlist, emailOfCreator, nameOfCreator);
        }

        [HttpGet]
        [Route("wishlist")]
        public async Task<WishlistModel> GetWishlist(string name)
        {
            var wishlist = await this._repository.Load(Guid.Parse(name));
            return new WishlistMapper().Map(wishlist);
        }

        [HttpPost]
        [Route("addPerson")]
        public async Task<WishlistModel> AddPerson(string wishlistId, string email, string name)
        {
            var wishlist = await this._repository.Load(Guid.Parse(wishlistId));
            wishlist.AddPerson(name, email);
            await this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }

        [HttpPost]
        [Route("addPresentIdea")]
        public async Task<WishlistModel> AddPresentIdea(string wishlistId, string email, string description)
        {
            var wishlist = await this._repository.Load(Guid.Parse(wishlistId));
            wishlist.AddPresentIdea(email, description);
            await this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
        
        [HttpPost]
        [Route("claimPresent")]
        public async Task<WishlistModel> ClaimPresent(string wishlistId, string email, string presentId)
        {
            var wishlist = await this._repository.Load(Guid.Parse(wishlistId));
            wishlist.ClaimPresent(Guid.Parse(presentId), email);
            await this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
        
        [HttpPost]
        [Route("unclaimPresent")]
        public async Task<WishlistModel> UnclaimPresent(string wishlistId, string presentId)
        {
            var wishlist = await this._repository.Load(Guid.Parse(wishlistId));
            wishlist.UnclaimPresent(Guid.Parse(presentId));
            await this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }

        [HttpGet]
        [Route("summaries")]
        public async Task<IEnumerable<WishlistSummaryModel>> GetSummaries(string emailAddress)
        {
            var summaries = await this._repository.GetWishlistSummaries(emailAddress);
            return summaries.Select(WishlistSummaryModel.Map);
        }

        private WishlistModel CreateNewWishlist(string nameOfWishlist, string emailOfCreator, string nameOfCreator)
        {
            var wishlist = Wishlist.Create(nameOfWishlist, emailOfCreator, nameOfCreator);
            this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
    }
}