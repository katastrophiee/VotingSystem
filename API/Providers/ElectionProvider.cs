using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class ElectionProvider(DBContext dbContext, IStringLocalizer<ElectionProvider> localizer) : IElectionProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<ElectionProvider> _localizer = localizer;

    public async Task<Response<List<GetElectionResponse>>> GetVoterOngoingElections(int voterId)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var ongoingElections = await _dbContext.Election
                .Where(v => v.Country == voter.Country
                && v.StartDate <= DateTime.Now
                && v.EndDate > DateTime.Now)
                .ToListAsync() ?? [];

            var response = ongoingElections.Select(e => new GetElectionResponse(e)).ToList();

            foreach (var election in response)
            {
                var vote = await _dbContext.Vote
                   .Where(v => v.VoterId == voterId
                   && v.ElectionId == election.ElectionId)
                   .FirstOrDefaultAsync();

                if (vote is not null)
                    election.HasVoted = true;
                else
                    election.HasVoted = false;
            }

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterOngoingElections"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetVoterVotedInElections(int voterId)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var electionInRegion = await _dbContext.Election
                .Where(e => e.Country == voter.Country)
                .ToListAsync() ?? [];

            var voterVotes = await _dbContext.Vote.Where(v => v.VoterId == voterId).ToListAsync() ?? [];

            var votedInElectionInRegion = electionInRegion
                .Where(e => voterVotes.Any(v => v.ElectionId == e.Id))
                .ToList() ?? [];

            var response = votedInElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterVotedInElections"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int voterId)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var recentElectionInRegion = await _dbContext.Election.Where(e =>
                e.Country == voter.Country
                && e.EndDate < DateTime.Now
                && e.EndDate <= DateTime.Now.AddMonths(3))
               .ToListAsync() ?? [];

            var response = recentElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetRecentlyEndedElections"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetVoterUpcomingElections(int voterId)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var upcomingElectionInRegion = await _dbContext.Election.Where(e =>
                e.Country == voter.Country
                && e.StartDate > DateTime.Now
                && e.StartDate <= DateTime.Now.AddMonths(3))
               .ToListAsync() ?? [];

            var response = upcomingElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterUpcomingElections"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetElectionResponse>> GetElection(int electionId, int voterId)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var election = await _dbContext.Election.FirstOrDefaultAsync(c => c.Id == electionId);
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoElectionFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new GetElectionResponse(election);

            var vote = await _dbContext.Vote
                  .Where(v => v.VoterId == voterId
                  && v.ElectionId == election.Id)
                  .FirstOrDefaultAsync();

            if (vote is not null)
                response.HasVoted = true;
            else
                response.HasVoted = false;

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetElection"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetElections(GetElectionsRequest request)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == request.VoterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var elections = await _dbContext.Election.Where(e => request.ElectionIds.Contains(e.Id)).ToListAsync() ?? [];

            var response = elections.Select(c => new GetElectionResponse(c)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetElections"]} {request.VoterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
