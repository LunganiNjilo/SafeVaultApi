namespace Application.Interfaces
{
    public interface IDbInitializer
    {
        Task SeedAsync();
    }
}
