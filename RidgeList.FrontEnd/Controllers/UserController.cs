using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RidgeList.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RidgeList.Models;

namespace RidgeList.FrontEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<Guid> Login(string emailAddress, string name)
        {
            var user = await this.userRepository.GetUserFromEmail(emailAddress);

            if(user == null)
            {
                Guid userId = Guid.NewGuid();
                await this.userRepository.CreatePerson(userId, emailAddress, name);
                return userId;
            }
            else
            {
                return user.Id;
            }
        }

        [HttpGet]
        [Route("getDetails")]
        public async Task<UserModel> GetUserDetails(Guid id)
        {
            var user = await this.userRepository.GetUser(id);
            return new UserModel()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }
        
        [HttpPost]
        [Route("setDetails")]
        public async Task SetUserDetails([FromBody] UserModel userModel)
        {
            var user = await this.userRepository.GetUser(userModel.Id);
            await this.userRepository.SetEmailAndName(userModel.Id, userModel.Email, userModel.Name);
        }
    }
    
    

    public class UserModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
