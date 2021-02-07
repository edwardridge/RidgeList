using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record CloneWishlistCommand(Guid WishlistId, string NewName) : IRequest;
    
    public class CloneWishlistHandler : IRequestHandler<CloneWishlistCommand>
    {
        private readonly WishlistCloner _cloner;
        private readonly IMediator _mediator;

        public CloneWishlistHandler(WishlistCloner cloner, IMediator mediator)
        {
            _cloner = cloner;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CloneWishlistCommand command, CancellationToken cancellationToken)
        {
            var newWishlist = await this._cloner.Clone(command.WishlistId, command.NewName);
            foreach (var person in newWishlist.People)
            {
                await _mediator.Publish(new PersonAddedToWishlist(person.PersonId, newWishlist.Id));
            }
            return new Unit();
        }
    }
}