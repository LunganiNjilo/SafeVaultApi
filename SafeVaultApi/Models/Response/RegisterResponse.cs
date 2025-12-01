namespace SafeVaultApi.Models.Response
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = "";
    }
}
