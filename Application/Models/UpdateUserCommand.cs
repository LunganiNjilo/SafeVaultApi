
namespace Application.Models
{
    public class UpdateUserCommand
    {
        public Guid UserId { get; set; }  // from session
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdNumber { get; set; }= string.Empty;
        public string? Password { get; set; } // optional
    }
}
