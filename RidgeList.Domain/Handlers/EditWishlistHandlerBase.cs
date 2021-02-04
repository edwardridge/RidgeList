using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public interface IEditWishlistCommand : IRequest<Wishlist>
    {
        public Guid WishlistId { get; }
    }
    
    public abstract class EditWishlistHandlerBase<T> : IRequestHandler<T, Wishlist> where T : IEditWishlistCommand
    {
        private readonly IWishlistRepository _repository;
        protected readonly IMediator _mediator;

        public EditWishlistHandlerBase(IWishlistRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }
        
        public async Task<Wishlist> Handle(T command, CancellationToken cancellationToken)
        {
            var wishlist = await this._repository.Load(command.WishlistId);
            await this.EditWishlist(command, wishlist);
            await _repository.Save(wishlist);
            
            return wishlist;
        }

        public abstract Task EditWishlist(T command, Wishlist wishlist);
    }
}