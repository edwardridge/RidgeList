using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record ClaimGiftIdeaCommand(Guid WishlistId, Guid PresentId, Guid claimerId) : IEditWishlistCommand;
    
    public class ClaimGiftIdeaHandler : EditWishlistHandlerBase<ClaimGiftIdeaCommand>
    {
        public ClaimGiftIdeaHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(ClaimGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.ClaimGift(command.PresentId, command.claimerId);
            return Task.CompletedTask;
        }
    }
}