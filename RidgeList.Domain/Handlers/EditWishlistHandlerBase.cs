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

        public EditWishlistHandlerBase(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<Wishlist> Handle(T command, CancellationToken cancellationToken)
        {
            var wishlist = await this._repository.Load(command.WishlistId);
            this.EditWishlist(command, wishlist);
            await _repository.Save(wishlist);
            
            return wishlist;
        }

        public abstract void EditWishlist(T command, Wishlist wishlist);
    }
}