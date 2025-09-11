using ColegiosBackend.Core.Interfaces;

namespace ColegiosBackend.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ColegiosDbContext _context;

    public UnitOfWork(ColegiosDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
