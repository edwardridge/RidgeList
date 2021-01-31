using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record RemoveGiftIdeaCommand(Guid WishlistId, string Email, Guid PresentId) : IEditWishlistCommand;
    
    public class RemoveGiftIdeaHandler : EditWishlistHandlerBase<RemoveGiftIdeaCommand>
    {
        public RemoveGiftIdeaHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(RemoveGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.RemoveGiftIdea(command.Email, command.PresentId);
            return Task.CompletedTask;
        }
    }
}