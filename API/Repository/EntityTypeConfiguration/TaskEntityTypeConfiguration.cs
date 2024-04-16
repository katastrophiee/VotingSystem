using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingSystem.API.DTO.DbModels.Admin;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class TaskEntityTypeConfiguration : IEntityTypeConfiguration<AdminTask>
{
    public void Configure(EntityTypeBuilder<AdminTask> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TaskStatus).HasConversion(dbIn => (byte)dbIn, dbOut => (Enums.TaskStatus)dbOut);
    }
}
