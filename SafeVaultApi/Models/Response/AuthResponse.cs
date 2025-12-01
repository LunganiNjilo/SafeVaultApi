namespace SafeVaultApi.Models.Response
{
    public class AuthResponse
    {
        public UserResponse User { get; set; } = new();
        public AccountResponse Account { get; set; } = new();
    }
}
