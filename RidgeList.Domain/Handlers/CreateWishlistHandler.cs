using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record CreateWishlistCommand(string Name, Guid IdOfCreator, bool CreatorIsGiftee) : IRequest<Wishlist>
    {
    }
    
    public class CreateWishlistHandler : IRequestHandler<CreateWishlistCommand, Wishlist>
    {
        private readonly IWishlistRepository _repository;
        private readonly IMediator _mediator;

        public CreateWishlistHandler(IWishlistRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }
        
        public async Task<Wishlist> Handle(CreateWishlistCommand command, CancellationToken cancellationToken)
        {
            var wishlist = Wishlist.Create(command.Name, command.IdOfCreator, command.CreatorIsGiftee);

            await _repository.Save(wishlist);
            
            await this._mediator.Publish(new PersonAddedToWishlist(command.IdOfCreator, wishlist.Id));
            
            return wishlist;
        }
    }
}