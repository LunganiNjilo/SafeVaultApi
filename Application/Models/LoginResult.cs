namespace Application.Models
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public Guid? UserId { get; init; }
        public string? Email { get; init; }

        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}
