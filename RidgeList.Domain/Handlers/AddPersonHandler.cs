using System;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record AddPersonCommand(Guid WishlistId, string PersonEmail, bool IsGiftee) : IEditWishlistCommand;
    
    public class AddPersonHandler : EditWishlistHandlerBase<AddPersonCommand>
    {
        private readonly IWishlistSummaryRepository wishlistSummaryRepository;

        public AddPersonHandler(IWishlistRepository repository, IMediator mediator, IWishlistSummaryRepository wishlistSummaryRepository) : base(repository, mediator)
        {
            this.wishlistSummaryRepository = wishlistSummaryRepository;
        }

        public override async Task EditWishlist(AddPersonCommand command, Wishlist wishlist)
        {
            Guid personId;
            var person = await wishlistSummaryRepository.GetUserFromEmail(command.PersonEmail);
            if(person == null)
            {
                personId = Guid.NewGuid();
                await this.wishlistSummaryRepository.CreatePerson(personId, command.PersonEmail);
            }
            else
            {
                personId = person.Id;
            }
            wishlist.AddPerson(personId, command.IsGiftee);
            await this._mediator.Publish(new PersonAddedToWishlist(personId, wishlist.Id));
        }
    }
}