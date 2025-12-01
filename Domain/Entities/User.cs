namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = null!;      
        public string LastName { get; set; } = null!;       
        public string IdNumber { get; set; } = null!;       
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
