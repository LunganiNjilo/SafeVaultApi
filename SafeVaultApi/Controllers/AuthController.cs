using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SafeVaultApi.Mapping;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;

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
            var result = await _auth.LoginAsync(request.Email, request.Password);

            if (!result.Success)
                return Unauthorized(new
                {
                    error = "login_failed",
                    message = "Incorrect email or password. Please try again."
                });

            var token = "FAKE-TOKEN"; // replace with JWT later
            return Ok(AuthMapper.ToLoginResponse(result!, token));
        }
    }
}

