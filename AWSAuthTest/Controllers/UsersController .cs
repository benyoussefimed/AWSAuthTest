using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWSAuthTest.Models;
using AWSAuthTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AWSAuthTest.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserService _userService;

        public UsersController(IConfiguration config, IUserService userService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        
        
        

        [AllowAnonymous]
        [HttpPost("~/api/authenticate")]
        public async Task<ActionResult<bool>> Authenticate(string email, string password)
        {
            var user = await _userService.Authenticate(email, password);


            return Ok((user != null));
        }

        [Authorize]
        [HttpPost("~/api/confidentials")]
        public async Task<ActionResult<bool>> Confidentials(string email)
        {
            var users = await _userService.GetAll();


            return Ok(users.Any(e=>e.Email.ToUpper() == email.ToUpper()));
        }

    }
}
