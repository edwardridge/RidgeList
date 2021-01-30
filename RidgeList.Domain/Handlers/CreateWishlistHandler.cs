using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record CreateWishlistCommand(string Name, string NameOfCreator, string EmailOfCreator, bool CreatorIsGiftee) : IRequest<Wishlist>
    {
    }
    
    public class CreateWishlistHandler : IRequestHandler<CreateWishlistCommand, Wishlist>
    {
        private readonly IWishlistRepository _repository;

        public CreateWishlistHandler(IWishlistRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<Wishlist> Handle(CreateWishlistCommand command, CancellationToken cancellationToken)
        {
            var wishlist = Wishlist.Create(command.Name, command.EmailOfCreator, command.NameOfCreator, command.CreatorIsGiftee);

            await _repository.Save(wishlist);
            
            return wishlist;
        }
    }
}