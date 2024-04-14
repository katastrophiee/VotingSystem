using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class VoterEntityTypeConfiguration : IEntityTypeConfiguration<Voter>
{
    public void Configure(EntityTypeBuilder<Voter> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Country).HasConversion(dbIn => (byte)dbIn, dbOut => (VoterCountry)dbOut);
    }
}
