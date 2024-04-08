using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class AdminEntityTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Country).HasConversion(dbIn => (byte)dbIn, dbOut => (CustomerCountry)dbOut);
    }
}
