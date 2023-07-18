using Microsoft.EntityFrameworkCore;
using StudentQueries.Data.Configurations;
using StudentQueries.Domain;

namespace StudentQueries.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Domain.Student> Students { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StudentsConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}