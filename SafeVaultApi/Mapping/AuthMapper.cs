using Application.Models;
using Domain.Entities;
using SafeVaultApi.Models.Request;
using SafeVaultApi.Models.Response;

namespace SafeVaultApi.Mapping
{
    public class AuthMapper
    {
        public static LoginResponse ToLoginResponse(User user, string token)
        {
            return new LoginResponse
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token
            };
        }

        public static RegisterResponse ToRegisterResponse(User user)
        {
            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
            };
        }

        public static RegisterUserCommand FromRegisterRequest(RegisterRequest request)
        {
            return new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                DateOfBirth = request.DateOfBirth,
                IdNumber = request.IdNumber,
                LastName = request.LastName,
                FirstName = request.FirstName
            };
        }
    }
}
