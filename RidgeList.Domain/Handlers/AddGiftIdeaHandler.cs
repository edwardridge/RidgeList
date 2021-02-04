using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record AddGiftIdeaCommand(Guid WishlistId, Guid PersonId, string Description) : IEditWishlistCommand;
    
    public class AddGiftIdeaHandler : EditWishlistHandlerBase<AddGiftIdeaCommand>
    {
        public AddGiftIdeaHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(AddGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.AddGiftIdea(command.PersonId, command.Description);
            return Task.CompletedTask;
        }
    }
}