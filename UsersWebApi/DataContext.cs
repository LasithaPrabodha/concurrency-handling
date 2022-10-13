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

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
        .Property(x => x.RowVersion)
        .IsRowVersion();
    }
}