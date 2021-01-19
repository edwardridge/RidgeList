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
                People = wishlist.GetPeople().Select(s => 
                    new WishlistPersonModel()
                    {
                        Name = s.Name, 
                        Email = s.Email,
                        PresentIdeas = s.PresentIdeas.Select(t => new PresentIdeaModel(){ Description = t.Description}).ToList()
                    }).ToList()
            };
        }
    }

    public class WishlistPersonModel
    {
        public string Email { get; set; }
        
        public string Name { get; set; }

        public List<PresentIdeaModel> PresentIdeas { get; set; }
    }

    public class PresentIdeaModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
    }
    
    public class WishlistModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<WishlistPersonModel> People { get; set; }
    }
}