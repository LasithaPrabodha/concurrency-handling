namespace UsersWebApi;

using Microsoft.EntityFrameworkCore;
using UsersWebApi.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public DbSet<User> Users { get; set; }

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(x => x.RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<User>().ToTable(t =>
            t.IsTemporal(t =>
            {
                t.HasPeriodStart("ValidFrom");
                t.HasPeriodEnd("ValidTo");
            }));
    }
}