using System;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record RemoveGiftIdeaCommand(Guid WishlistId, string Email, Guid PresentId) : IEditWishlistCommand;
    
    public class RemoveGiftIdeaHandler : EditWishlistHandlerBase<RemoveGiftIdeaCommand>
    {
        public RemoveGiftIdeaHandler(IWishlistRepository repository) : base(repository)
        {
        }

        public override void EditWishlist(RemoveGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.RemoveGiftIdea(command.Email, command.PresentId);
        }
    }
}