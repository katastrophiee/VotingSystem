using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.RoleIds).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<IEnumerable<int>>(dbOut, (JsonSerializerOptions)null));
    }
}
