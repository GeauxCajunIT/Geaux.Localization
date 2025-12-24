using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Geaux.Localization.Contexts;

/// <summary>
/// Creates a <see cref="GeauxLocalizationDbContext"/> at design time for EF Core tooling (migrations).
/// </summary>
/// <remarks>
/// EF tooling calls this factory when it needs to instantiate the DbContext without running the host.
/// Connection string is resolved from the environment variable <c>GEAUX_LOCALIZATION_CONNECTION</c>.
/// If not set, a safe default LocalDb connection string is used.
/// </remarks>
public sealed class DesignTimeLocalizationDbContextFactory : IDesignTimeDbContextFactory<GeauxLocalizationDbContext>
{
    /// <inheritdoc />
    public GeauxLocalizationDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<GeauxLocalizationDbContext> builder = new DbContextOptionsBuilder<GeauxLocalizationDbContext>();

        string conn = Environment.GetEnvironmentVariable("GEAUX_LOCALIZATION_CONNECTION")
                   ?? @"Server=(localdb)\\MSSQLLocalDB;Database=LocalizationDb;Trusted_Connection=True;TrustServerCertificate=True;";

        builder.UseSqlServer(conn, b =>
            b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));

        return new GeauxLocalizationDbContext(builder.Options);
    }
}

