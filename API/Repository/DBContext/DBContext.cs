using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.Repository.EntityTypeConfiguration;

namespace VotingSystem.API.Repository.DBContext;

// I used this video to help me set up my db context and entity framework
//https://www.youtube.com/watch?v=AdpMd29cIzg&t=146s
// And this
// https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
{
    public DbSet<Voter> Voter { get; set; }
    public DbSet<Vote> Vote { get; set; }
    public DbSet<Document> Document { get; set; }
    public DbSet<Election> Election { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public DbSet<VoteDetails> VoteDetails { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<AdminTask> AdminTask { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VoterEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VoteEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ElectionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AdminEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VoteDetailsEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TaskEntityTypeConfiguration());
    }
}
