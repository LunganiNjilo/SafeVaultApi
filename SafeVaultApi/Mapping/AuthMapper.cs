using Application.Models;
using Domain.Entities;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;

namespace SafeVaultApi.Mapping
{
    public class AuthMapper
    {
        public static LoginResponse ToLoginResponse(LoginResult user, string token)
        {
            return new LoginResponse
            {
                UserId = user.UserId ?? Guid.Empty,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                Email = user.Email!,
                Token = token
            };
        }
    }
}
