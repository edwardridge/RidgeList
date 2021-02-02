using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;
using RidgeList.Domain.Handlers;
using RidgeList.FrontEnd.SignalRHubs;
using RidgeList.Models;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _repository;
        private readonly IWishlistSummaryRepository _wishlistSummaryRepository;
        private readonly IMediator _mediator;
        private readonly IUpdateWishlistHub _updateWishlistHub;

        public WishlistController(
            IWishlistRepository repository, 
            IWishlistSummaryRepository wishlistSummaryRepository, 
            IMediator mediator, IUpdateWishlistHub updateWishlistHub)
        {
            _repository = repository;
            _wishlistSummaryRepository = wishlistSummaryRepository;
            _mediator = mediator;
            _updateWishlistHub = updateWishlistHub;
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
            var ids = await this._wishlistSummaryRepository.GetWishlistSummaries(emailAddress);
            var summaries = new List<WishlistSummary>();
            foreach (var id in ids)
            {
                var wishlist = await _repository.Load(id);
                summaries.Add(new WishlistSummary()
                {
                    Id = id,
                    Name = wishlist.Name
                });
            }
            return summaries.Select(WishlistSummaryModel.Map);
        }

        private static WishlistModel MapWishlistToModel(Wishlist wishlist)
        {
            return new WishlistMapper().Map(wishlist);
        }

        private async Task<WishlistModel> SendCommandAndMapResponse(IEditWishlistCommand command)
        {
            var wishlist = await this._mediator.Send(command);
            var model = MapWishlistToModel(wishlist);
            _updateWishlistHub.SendWishlist(model);
            return model;
        }
    }
}