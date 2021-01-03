using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        [HttpPost]
        [Route("create")]
        public WishlistModel Create(string name)
        {
            return CreateNewWishlist(name);
        }
        
        [HttpGet]
        [Route("wishlist")]
        public async Task<WishlistModel> GetWishlist(string id)
        {
            var wishlist = await this._repository.Load(Guid.Parse(id));
            return new WishlistMapper().Map(wishlist);
        }
        
        [HttpPost]
        [Route("addPerson")]
        public async Task<WishlistModel> AddPerson(string wishlistId, string email)
        {
            var wishlist = await this._repository.Load(Guid.Parse(wishlistId));
            wishlist.AddPerson(email);
            await this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
        
        [HttpGet]
        [Route("summaries")]
        public async Task<IEnumerable<WishlistSummaryModel>> GetSummaries()
        {
            var summaries = await this._repository.GetWishlistSummaries();
            return summaries.Select(WishlistSummaryModel.Map);
        }

        private WishlistModel CreateNewWishlist(string name)
        {
            var wishlist = Wishlist.Create(name);
            this._repository.Save(wishlist);
            return new WishlistMapper().Map(wishlist);
        }
    }

    public class WishlistSummaryModel
    {
        public string Name { get; set; }        
        
        public Guid Id { get; set; }

        public static WishlistSummaryModel Map(WishlistSummary summary)
        {
            return new WishlistSummaryModel()
            {
                Id = summary.Id,
                Name = summary.Name
            };
        }
    }

    public class WishlistMapper
    {
        public WishlistModel Map(Wishlist wishlist)
        {
            return new WishlistModel()
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                People = wishlist.GetPeople()
            };
        }
    }
    
    public class WishlistModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<string> People { get; set; }
    }
}