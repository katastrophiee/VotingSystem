using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class AdminProvider(DBContext dbContext) : IAdminProvider
{
    private readonly DBContext _dbContext = dbContext;
}
