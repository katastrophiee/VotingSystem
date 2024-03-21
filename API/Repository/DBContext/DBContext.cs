using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbResults;
using VotingSystem.API.Repository.EntityTypeConfiguration;

namespace VotingSystem.API.Repository.DBContext;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
        
    }

    public DbSet<Customer> Customer { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
    }
}
