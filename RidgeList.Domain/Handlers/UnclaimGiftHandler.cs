using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record UnclaimGiftIdeaCommand(Guid WishlistId, Guid PresentId) : IEditWishlistCommand;
    
    public class UnclaimGiftIdeaHandler : EditWishlistHandlerBase<UnclaimGiftIdeaCommand>
    {
        public UnclaimGiftIdeaHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(UnclaimGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.UnclaimPresent(command.PresentId);
            return Task.CompletedTask;
        }
    }
}