using System;

namespace RidgeList.Domain.Handlers
{
    public record AddPersonCommand(Guid WishlistId, string Name, string Email, bool IsGiftee) : IEditWishlistCommand;
    
    public class AddPersonHandler : EditWishlistHandlerBase<AddPersonCommand>
    {
        public AddPersonHandler(IWishlistRepository repository) : base(repository)
        {
        }

        public override void EditWishlist(AddPersonCommand command, Wishlist wishlist)
        {
            wishlist.AddPerson(command.Name, command.Email, command.IsGiftee);
        }
    }
}