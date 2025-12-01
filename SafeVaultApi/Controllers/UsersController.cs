using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;
using System.Net;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
        {
            var command = UserMapper.FromUpdateUserRequest(request);

            var result = await _userService.UpdateUserProfileAsync(command);

            if (!result.Success)
                throw new ApiException((int)HttpStatusCode.BadRequest, ErrorType.BadRequest, $"Profile updated successfully : {result.Message}");

            return Ok(new { success = true, message = "Profile updated successfully" });
        }
    }
}
