namespace UsersWebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using UsersWebApi.Entities;
using UsersWebApi.Services;

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