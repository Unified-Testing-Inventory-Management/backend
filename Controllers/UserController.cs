using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Interface;
using System.Security.Claims;
using Server.Exceptions;

namespace Server.Controllers
{
    [Route("api/v1/users/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserInterface _userServices;

        public UserController(IUserInterface userServices)
        {
            _userServices = userServices;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var user = await _userServices.GetUserById(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
        {
            try
            {
                await _userServices.RegisterUser(_registerUserDTOs);
                return Ok();
            }
            catch (UserExceptions.UserAlreadyExists e)
            {
                return StatusCode(409, new { error = e.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
        {
            try
            {
                await _userServices.LoginUser(_loginUserDTOs);
                Console.WriteLine("Login successful");

                return Ok(new { message = "Login successful" });
            }
            catch (UserExceptions.UnAuthorizedUserException e)
            {
                return Unauthorized(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }
    }
}
