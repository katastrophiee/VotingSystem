using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.API.Repository.EntityTypeConfiguration;

public class VoteDetailsFptpEntityTypeConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder.HasNoKey();
    }
}

public class Test()
{
    public string Candidate { get; set; }
}