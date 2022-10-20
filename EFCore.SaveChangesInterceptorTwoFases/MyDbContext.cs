using Microsoft.EntityFrameworkCore;

namespace EFCore.SaveChangesInterceptorTwoFases;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>();
        modelBuilder.Entity<Post>();
        modelBuilder.Entity<DomainEventDetails>();
    }
}
