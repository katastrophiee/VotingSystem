using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class VoteEntityTypeConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Country).HasConversion(dbIn => (byte)dbIn, dbOut => (VoterCountry)dbOut);
    }
}
