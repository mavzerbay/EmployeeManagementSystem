using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthenticationController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if(user is null) return BadRequest("Model is empty");
            var result = await userAccount.CreateAsync(user);
            return Ok(result);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if(user is null) return BadRequest("Model is empty");
            var result = await userAccount.SignInAsync(user);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if(token is null) return BadRequest("Model is empty");
            var result = await userAccount.RefreshTokenAsync(token);
            return Ok(result);
        }
    }
}