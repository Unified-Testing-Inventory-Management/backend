using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Interface;
using System.Security.Claims;

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

        [Authorize(Policy = "OwnerOnly")]
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
            catch (Exception e)
            {
                return Conflict(new { error = e.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
        {
            try
            {
                var token = await _userServices.LoginUser(_loginUserDTOs);
                Console.WriteLine("Generated Token: " + token);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                return Ok(new { message = "Login successful" });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { error = e.Message });
            }
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logout successful" });
        }
    }
}
