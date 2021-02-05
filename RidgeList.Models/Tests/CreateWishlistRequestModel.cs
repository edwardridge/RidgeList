using System;

namespace RidgeList.Models.Tests
{
    public class CreateWishlistRequestModel
    {
        public string title { get; set; }

        public Guid creatorId { get; set; }
    }
}
