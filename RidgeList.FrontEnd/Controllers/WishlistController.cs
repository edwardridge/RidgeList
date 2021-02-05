using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RidgeList.ApplicationServices;
using RidgeList.Domain;
using RidgeList.Domain.Handlers;
using RidgeList.FrontEnd.SignalRHubs;
using RidgeList.Models;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("[controller]")]
    public class WishlistController : Controller
    {
        private readonly WishlistMapper wishlistMapper;
        private readonly IWishlistRepository _repository;
        private readonly IUserRepository _wishlistSummaryRepository;
        private readonly IMediator _mediator;
        private readonly IUpdateWishlistHub _updateWishlistHub;

        public WishlistController(
            WishlistMapper wishlistMapper,
            IWishlistRepository repository, 
            IUserRepository wishlistSummaryRepository, 
            IMediator mediator, IUpdateWishlistHub updateWishlistHub)
        {
            this.wishlistMapper = wishlistMapper;
            _repository = repository;
            _wishlistSummaryRepository = wishlistSummaryRepository;
            _mediator = mediator;
            _updateWishlistHub = updateWishlistHub;
        }

        [HttpPost]
        [Route("create")]
        public async Task<WishlistModel> Create(string nameOfWishlist, Guid creatorId, bool creatorIsGiftee)
        {
            var wishlist = await this._mediator.Send(new CreateWishlistCommand(nameOfWishlist, creatorId, creatorIsGiftee));

            return await MapWishlistToModel(wishlist);
        }

        [HttpGet]
        [Route("wishlist")]
        public async Task<WishlistModel> GetWishlist(string name)
        {
            var wishlist = await this._repository.Load(Guid.Parse(name));
            return await wishlistMapper.Map(wishlist);
        }

        [HttpPost]
        [Route("addPerson")]
        public async Task<WishlistModel> AddPerson(Guid wishlistId, string email, string name, bool isGiftee)
        {
            return await SendCommandAndMapResponse(new AddPersonCommand(wishlistId, email, name, isGiftee));
        }

        [HttpPost]
        [Route("addGiftIdea")]
        public async Task<WishlistModel> AddGiftIdea(Guid wishlistId, Guid personId, string description)
        {
            return await SendCommandAndMapResponse(new AddGiftIdeaCommand(wishlistId, personId, description));
        }

        [HttpPost]
        [Route("removeGiftIdea")]
        public async Task<WishlistModel> RemoveGiftIdea(Guid wishlistId, Guid personId, Guid presentId)
        {
            return await SendCommandAndMapResponse(new RemoveGiftIdeaCommand(wishlistId, personId, presentId));
        }
        
        [HttpPost]
        [Route("claimGift")]
        public async Task<WishlistModel> ClaimGift(Guid wishlistId, Guid personid, Guid presentId)
        {
            return await SendCommandAndMapResponse(new ClaimGiftIdeaCommand(wishlistId, presentId, personid));
        }
        
        [HttpPost]
        [Route("unclaimGift")]
        public async Task<WishlistModel> UnclaimGift(Guid wishlistId, Guid presentId)
        {
            return await SendCommandAndMapResponse(new UnclaimGiftIdeaCommand(wishlistId, presentId));
        }

        [HttpGet]
        [Route("summaries")]
        public async Task<IEnumerable<WishlistSummaryModel>> GetSummaries(Guid personId)
        {
            var person = await this._wishlistSummaryRepository.GetUser(personId);
            var summaries = new List<WishlistSummary>();
            foreach (var wishlistId in person.Wishlists)
            {
                var wishlist = await _repository.Load(wishlistId);
                summaries.Add(new WishlistSummary()
                {
                    Id = wishlistId,
                    Name = wishlist.Name
                });
            }
            return summaries.Select(WishlistSummaryModel.Map);
        }

        private async Task<WishlistModel> MapWishlistToModel(Wishlist wishlist)
        {
            return await wishlistMapper.Map(wishlist);
        }

        private async Task<WishlistModel> SendCommandAndMapResponse(IEditWishlistCommand command)
        {
            var wishlist = await this._mediator.Send(command);
            var model = await MapWishlistToModel(wishlist);
            await _updateWishlistHub.SendWishlist(model);
            return model;
        }
    }
}