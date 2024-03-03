using Microsoft.AspNetCore.Mvc;
using FCxLabs.API.Models;
using Microsoft.AspNetCore.Authorization;
using FCxLabs.Domain.Account;
using FCxLabs.Domain.Entities;

namespace FCxLabs.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authentication;

        public AuthenticateController(IAuthenticateService authentication)
        {
            _authentication = authentication;
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Authenticate([FromBody] LoginDTO loginDTO)
        {
            var result = await _authentication.Authenticate(loginDTO.UserName, loginDTO.Password);
            if (result)
            {
                var token = _authentication.GenerateToken(loginDTO.UserName);
                return new UserToken { Token = token };
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }
    }
}
