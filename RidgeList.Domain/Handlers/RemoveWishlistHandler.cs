using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record RemoveWishlistCommand(Guid WishlistId) : IRequest;
    
    public class RemoveWishlistHandler : IRequestHandler<RemoveWishlistCommand>
    {
        private readonly IWishlistRepository _repository;
        private readonly IMediator _mediator;

        public RemoveWishlistHandler(IWishlistRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }
        
        public async Task<Unit> Handle(RemoveWishlistCommand request, CancellationToken cancellationToken)
        {
            var wishlist = await this._repository.Load(request.WishlistId);
            await this._repository.Delete(request.WishlistId);

            foreach (var person in wishlist.People)
            {
                await _mediator.Publish(new PersonRemovedFromWishlist(person.PersonId, wishlist.Id));
            }

            return new Unit();
        }
    }
}