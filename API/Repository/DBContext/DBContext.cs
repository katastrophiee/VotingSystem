using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.Repository.EntityTypeConfiguration;

namespace VotingSystem.API.Repository.DBContext;

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public DbSet<Customer> Customer { get; set; }
    public DbSet<Vote> Vote { get; set; }
    public DbSet<Document> Document { get; set; }
    public DbSet<Election> Election { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public DbSet<VoteDetails> VoteDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VoteEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ElectionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AdminEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VoteDetailsEntityTypeConfiguration());
    }
}
