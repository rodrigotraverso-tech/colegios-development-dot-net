using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ColegiosBackend.Infrastructure.Data;

public class ColegiosDbContextFactory : IDesignTimeDbContextFactory<ColegiosDbContext>
{
    public ColegiosDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ColegiosDbContext>();

        // Connection string para migraciones
        optionsBuilder.UseNpgsql("Host=localhost;Database=colegios-claude;Username=postgres;Password=postgres");
        return new ColegiosDbContext(optionsBuilder.Options);
    }
}