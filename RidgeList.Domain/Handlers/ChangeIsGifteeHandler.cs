using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record ChangeIsGifteeCommand(Guid WishlistId, Guid PersonId, bool IsGiftee) : IEditWishlistCommand;
    
    public class ChangeIsGifteeHandler : EditWishlistHandlerBase<ChangeIsGifteeCommand>
    {
        public ChangeIsGifteeHandler(IWishlistRepository repository, IMediator mediator) : base(repository, mediator)
        {
        }

        public override Task EditWishlist(ChangeIsGifteeCommand command, Wishlist wishlist)
        {
            wishlist.ChangeIsGiftee(command.PersonId, command.IsGiftee);
            return Task.CompletedTask;
        }
    }
}