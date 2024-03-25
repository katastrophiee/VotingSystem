using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbResults;
using VotingSystem.API.Enums;
using System.Text.Json;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class VoteEntityTypeConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Country).HasConversion(dbIn => (byte)dbIn, dbOut => (CustomerCountry)dbOut);

        //builder.Property(e => e.VoteOption).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<List<Test>>(dbOut, (JsonSerializerOptions)null));
    }
}
