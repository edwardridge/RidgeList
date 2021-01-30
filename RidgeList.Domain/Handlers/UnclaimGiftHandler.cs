using System;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record UnclaimGiftIdeaCommand(Guid WishlistId, Guid PresentId) : IEditWishlistCommand;
    
    public class UnclaimGiftIdeaHandler : EditWishlistHandlerBase<UnclaimGiftIdeaCommand>
    {
        public UnclaimGiftIdeaHandler(IWishlistRepository repository) : base(repository)
        {
        }

        public override void EditWishlist(UnclaimGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.UnclaimPresent(command.PresentId);
        }
    }
}