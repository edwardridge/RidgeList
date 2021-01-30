using System;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record ClaimGiftIdeaCommand(Guid WishlistId, Guid PresentId,  string Email) : IEditWishlistCommand;
    
    public class ClaimGiftIdeaHandler : EditWishlistHandlerBase<ClaimGiftIdeaCommand>
    {
        public ClaimGiftIdeaHandler(IWishlistRepository repository) : base(repository)
        {
        }

        public override void EditWishlist(ClaimGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.ClaimGift(command.PresentId, command.Email);
        }
    }
}