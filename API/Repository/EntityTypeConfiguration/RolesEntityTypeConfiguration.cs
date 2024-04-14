using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class RolesEntityTypeConfiguration : IEntityTypeConfiguration<Roles>
{
    //TO DO
    //Make generic ef config file for ones only with id
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(e => e.RoleId);
    }
}
