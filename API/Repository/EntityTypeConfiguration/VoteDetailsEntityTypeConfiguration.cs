using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class VoteDetailsEntityTypeConfiguration : IEntityTypeConfiguration<VoteDetails>
{
    public void Configure(EntityTypeBuilder<VoteDetails> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ElectionType).HasConversion(dbIn => (byte)dbIn, dbOut => (ElectionType)dbOut);

        builder.Property(e => e.Choices).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<List<ElectionOption>>(dbOut, (JsonSerializerOptions)null));
    }
}
