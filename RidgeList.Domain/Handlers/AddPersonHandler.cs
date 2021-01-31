using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record AddPersonCommand(Guid WishlistId, string Name, string Email, bool IsGiftee) : IEditWishlistCommand;
    
    public class AddPersonHandler : EditWishlistHandlerBase<AddPersonCommand>
    {
        public AddPersonHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override async Task EditWishlist(AddPersonCommand command, Wishlist wishlist)
        {
            wishlist.AddPerson(command.Name, command.Email, command.IsGiftee);
            await this._mediator.Publish(new PersonAddedToWishlist(command.Email, wishlist.Id));
        }
    }
}