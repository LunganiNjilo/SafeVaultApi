namespace SafeVaultApi.Models.Request
{
    public class UpdateUserRequest
    {
        public Guid UserId { get; set; }  
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string IdNumber { get; set; } = string.Empty;
        public string? Password { get; set; } 
    }
}
