using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RidgeList.Models;

namespace RidgeList.FrontEnd.SignalRHubs
{
    public class WishlistHub : Hub
    {
        public Task SendWishlist(WishlistModel wishlistModel)
        {
            return Clients.Group(wishlistModel.Id.ToString()).SendAsync("UpdateWishlist", wishlistModel);
        }
        
        public Task Connect(Guid wishlistId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, wishlistId.ToString());
        }
    }

    public class UpdateWishlistHub : IUpdateWishlistHub
    {
        private readonly IHubContext<WishlistHub> _hubContext;

        public UpdateWishlistHub(IHubContext<WishlistHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public Task SendWishlist(WishlistModel wishlistModel)
        {
            return _hubContext.Clients.Group(wishlistModel.Id.ToString()).SendAsync("UpdateWishlist", wishlistModel);
        }
    }

    public interface IUpdateWishlistHub
    {
        Task SendWishlist(WishlistModel wishlistModel);
    }
}