namespace Application.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }
}
