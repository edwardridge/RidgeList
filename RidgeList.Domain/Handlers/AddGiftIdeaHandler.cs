using System;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record AddGiftIdeaCommand(Guid WishlistId, string Email, string Description) : IEditWishlistCommand;
    
    public class AddGiftIdeaHandler : EditWishlistHandlerBase<AddGiftIdeaCommand>
    {
        public AddGiftIdeaHandler(IWishlistRepository repository) : base(repository)
        {
        }

        public override void EditWishlist(AddGiftIdeaCommand command, Wishlist wishlist)
        {
            wishlist.AddGiftIdea(command.Email, command.Description);
        }
    }
}