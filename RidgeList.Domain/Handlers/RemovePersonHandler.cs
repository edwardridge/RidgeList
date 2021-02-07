using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record RemovePersonCommand(Guid WishlistId, Guid PersonId) : IEditWishlistCommand;
    
    public class RemovePersonHandler : EditWishlistHandlerBase<RemovePersonCommand>
    {
        public RemovePersonHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(RemovePersonCommand command, Wishlist wishlist)
        {
            wishlist.RemovePerson(command.PersonId);
            return Task.CompletedTask;
        }
    }
}