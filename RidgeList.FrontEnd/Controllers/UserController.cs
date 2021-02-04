using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IWishlistSummaryRepository wishlistSummaryRepository;

        public UserController(IWishlistSummaryRepository wishlistSummaryRepository)
        {
            this.wishlistSummaryRepository = wishlistSummaryRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<Guid> Login(string emailAddress, string name)
        {
            var user = await this.wishlistSummaryRepository.GetUserFromEmail(emailAddress);

            if(user == null)
            {
                Guid userId = Guid.NewGuid();
                await this.wishlistSummaryRepository.CreatePerson(userId, emailAddress, name);
                return userId;
            }
            else
            {
                return user.Id;
            }
        }
    }
}
