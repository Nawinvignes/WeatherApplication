using Microsoft.EntityFrameworkCore;
using Weather.Domain;
using Weather.Domain.Entities;

namespace Weather.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

}