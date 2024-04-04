using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Repository.EntityTypeConfiguration;

namespace VotingSystem.API.Repository.DBContext;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Vote> Vote { get; set; }
    public DbSet<Document> Document { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VoteEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentEntityTypeConfiguration());
    }
}
