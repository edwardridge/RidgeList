using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;
using RidgeList.Domain.Handlers;
using RidgeList.Models;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _repository;
        private readonly IMediator _mediator;

        public WishlistController(IWishlistRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("create")]
        public async Task<WishlistModel> Create(string nameOfWishlist, string emailOfCreator, string nameOfCreator, bool creatorIsGiftee)
        {
            var wishlist = await this._mediator.Send(new CreateWishlistCommand(nameOfWishlist, nameOfCreator, emailOfCreator, creatorIsGiftee));

            return MapWishlistToModel(wishlist);
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
        public async Task<WishlistModel> AddPerson(Guid wishlistId, string email, string name, bool isGiftee)
        {
            return await SendCommandAndMapResponse(new AddPersonCommand(wishlistId, name, email, isGiftee));
        }

        [HttpPost]
        [Route("addGiftIdea")]
        public async Task<WishlistModel> AddGiftIdea(Guid wishlistId, string email, string description)
        {
            return await SendCommandAndMapResponse(new AddGiftIdeaCommand(wishlistId, email, description));
        }

        [HttpPost]
        [Route("removeGiftIdea")]
        public async Task<WishlistModel> RemoveGiftIdea(Guid wishlistId, string email, Guid presentId)
        {
            return await SendCommandAndMapResponse(new RemoveGiftIdeaCommand(wishlistId, email, presentId));
        }
        
        [HttpPost]
        [Route("claimGift")]
        public async Task<WishlistModel> ClaimGift(Guid wishlistId, string email, Guid presentId)
        {
            return await SendCommandAndMapResponse(new ClaimGiftIdeaCommand(wishlistId, presentId, email));
        }
        
        [HttpPost]
        [Route("unclaimGift")]
        public async Task<WishlistModel> UnclaimGift(Guid wishlistId, Guid presentId)
        {
            return await SendCommandAndMapResponse(new UnclaimGiftIdeaCommand(wishlistId, presentId));
        }

        [HttpGet]
        [Route("summaries")]
        public async Task<IEnumerable<WishlistSummaryModel>> GetSummaries(string emailAddress)
        {
            var summaries = await this._repository.GetWishlistSummaries(emailAddress);
            return summaries.Select(WishlistSummaryModel.Map);
        }

        private static WishlistModel MapWishlistToModel(Wishlist wishlist)
        {
            return new WishlistMapper().Map(wishlist);
        }

        private async Task<WishlistModel> SendCommandAndMapResponse(IEditWishlistCommand command)
        {
            var wishlist = await this._mediator.Send(command);
            return MapWishlistToModel(wishlist);
        }
    }
}