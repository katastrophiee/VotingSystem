using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class RolesEntityTypeConfiguration : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(e => e.RoleId);
    }
}
