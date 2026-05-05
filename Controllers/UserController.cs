using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.DTOs;
using Server.Interface;
using System.Security.Claims;
using Server.Exceptions;

namespace Server.Controllers
{
    [Route("api/v1/users/auth")]
    [ApiController]
    public class UserController(IUserService userServices) : ControllerBase
    {
        private readonly IUserService _userServices = userServices;

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
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
        {
            try
            {
                await _userServices.RegisterUser(_registerUserDTOs);

                Console.WriteLine(new { message = "Registered user successfully", status = 201 });
                return Ok(new { Message = "Registered user successfully" });
            }
            catch (UserExceptions.UserAlreadyExists e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return Conflict(new { error = e.Message, status = e.StatusCode });
            }
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
        {
            try
            {
                await _userServices.LoginUser(_loginUserDTOs);

                Console.WriteLine(new { message = "Logout successfully", status = 200 });
                return Ok(new { message = "Login successfully", status = 200 });
            }
            catch (UserExceptions.UnAuthorizedUserException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return Unauthorized(new { error = e.Message, status = e.StatusCode });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("update-profile")]
        public async Task<IActionResult> UpdateUser(UserDTOs.UpdateUserDTOs _updateUserDTOs)
        {
            try
            {
                await _userServices.UpdateUser(_updateUserDTOs);

                Console.WriteLine(new { message = "User updated successfully", status = 201 });
                return Ok(new { message = "User updated successfully", status = 201 });
            }
            catch (UserExceptions.UserNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
            }
            catch (UserExceptions.UserAlreadyExists e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return Conflict(new { error = e.Message, status = e.StatusCode });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            Console.WriteLine(new { message = "Logout successfully", status = 201 });
            return Ok(new { message = "Logout successfully" });
        }
    }
}
