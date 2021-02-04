using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace RidgeList.Domain.Handlers
{
    public record PersonAddedToWishlist(Guid PersonId, Guid wishlistId) : INotification
    {
    }
    
    public record PersonRemovedFromWishlist(Guid PersonId, Guid wishlistId) : INotification
    {
    }
    
    public class UserWishlistsEventHandler 
        : INotificationHandler<PersonAddedToWishlist>, INotificationHandler<PersonRemovedFromWishlist>
    {
        private readonly IWishlistSummaryRepository _wishlistSummaryRepository;

        public UserWishlistsEventHandler(IWishlistSummaryRepository wishlistSummaryRepository)
        {
            _wishlistSummaryRepository = wishlistSummaryRepository;
        }
        
        public async Task Handle(PersonAddedToWishlist notification, CancellationToken cancellationToken)
        {
            await _wishlistSummaryRepository.AddWishlistToPerson(notification.PersonId, notification.wishlistId);
        }

        public async Task Handle(PersonRemovedFromWishlist notification, CancellationToken cancellationToken)
        {
            await this._wishlistSummaryRepository.RemoveWishlistFromPerson(notification.PersonId, notification.wishlistId);
        }
    }
}