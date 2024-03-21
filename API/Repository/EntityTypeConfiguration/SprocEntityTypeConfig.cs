using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class SprocEntityTypeConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class, new()
{
    public void Configure(EntityTypeBuilder<TEntity> entity)
    {
        entity.HasNoKey();
        
    }
}
