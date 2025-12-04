using Application.Models;
using SafeVaultApi.Models.Request;

namespace SafeVaultApi.Mapping
{
    public class UserMapper
    {

        public static UpdateUserCommand FromUpdateUserRequest(UpdateUserRequest request)
        {
            return new UpdateUserCommand
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                IdNumber = request.IdNumber
            };

        }
    }
}
