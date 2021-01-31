using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record ClaimGiftIdeaCommand(Guid WishlistId, Guid PresentId,  string Email) : IEditWishlistCommand;
    
    public class ClaimGiftIdeaHandler : EditWishlistHandlerBase<ClaimGiftIdeaCommand>
    {
        public ClaimGiftIdeaHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(ClaimGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.ClaimGift(command.PresentId, command.Email);
            return Task.CompletedTask;
        }
    }
}