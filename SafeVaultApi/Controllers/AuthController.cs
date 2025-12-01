using Application.Common.Exceptions;
using Application.Enums;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;
using System.Net;

namespace SafeVaultApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _auth.LoginAsync(request.Email, request.Password);

            if (user == null)
            {
                throw new ApiException((int)HttpStatusCode.Unauthorized, ErrorType.Unauthorized, "Invalid credentials");
            }

            // TODO: Replace with JWT later
            var token = "FAKE-TOKEN";

            return Ok(AuthMapper.ToLoginResponse(user, token));
        }
    }
}

