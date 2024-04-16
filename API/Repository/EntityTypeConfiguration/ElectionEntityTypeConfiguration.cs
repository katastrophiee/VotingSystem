using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.Enums;

namespace VotingSystem.API.Repository.EntityTypeConfiguration
{
    public class ElectionEntityTypeConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Country).HasConversion(dbIn => (byte)dbIn, dbOut => (UserCountry)dbOut);

            builder.Property(e => e.ElectionType).HasConversion(dbIn => (byte)dbIn, dbOut => (ElectionType)dbOut);

            builder.Property(e => e.ElectionOptions).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<List<ElectionOption>>(dbOut, (JsonSerializerOptions)null));
        }
    }
}
