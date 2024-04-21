using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Enums;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class VoteProvider(DBContext dbContext, IStringLocalizer<VoteProvider> localizer) : IVoteProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<VoteProvider> _localizer = localizer;

    public async Task<Response<IEnumerable<GetVotingHistoryResponse>>> GetVoterVotingHistory(int voterId)
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

            var votes = await _dbContext.Vote
               .Where(v => v.VoterId == voterId)
               .ToListAsync();

            var response = new List<GetVotingHistoryResponse>();
            votes?.ForEach(vote => response.Add(new(vote)));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterVotingHistory"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<int>> AddVoterVote(AddVoterVoteRequest request)
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

            if (!voter.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = _localizer["VoterNotVerified"],
                    Description = $"{_localizer["VoterNotVerifiedWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            var election = await _dbContext.Election.FirstOrDefaultAsync(c => c.Id == request.ElectionId);
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoElectionFound"],
                    Description = $"{_localizer["NoElectionFoundWithId"]} {request.ElectionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var vote = new Vote()
            {
                ElectionId = election.Id,
                ElectionName = election.ElectionName,
                ElectionDescription = election.ElectionDescription,
                VoteDate = DateTime.Now,
                VoterId = voter.Id,
                Country = request.Country
            };

            _dbContext.Vote.Add(vote);
            await _dbContext.SaveChangesAsync();

            var addedVote = await _dbContext.Vote.Where(v =>
                v.ElectionId == election.Id
                && v.VoterId == voter.Id)
                .FirstOrDefaultAsync();

            if (addedVote is null || addedVote.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoteFound"],
                    Description = $"{_localizer["NoVoteFoundWithId"]} {request.ElectionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (election.ElectionType == ElectionType.Election_STV || election.ElectionType == ElectionType.Election_Preferential && !string.IsNullOrEmpty(request.ElectionTypeAdditionalInfo))
            {
                var details = JsonSerializer.Deserialize<VoteDetails>(request.ElectionTypeAdditionalInfo);
                if (details is not null)
                {
                    details.VoteId = addedVote.Id;

                    _dbContext.VoteDetails.Add(details);
                    await _dbContext.SaveChangesAsync();
                }

                var voteDetails = new VoteDetails()
                {
                    VoteId = addedVote.Id,
                    ElectionType = election.ElectionType,
                    Choices = request.Choices,
                    ElectionTypeAdditionalInfo = request.ElectionTypeAdditionalInfo
                };

                _dbContext.VoteDetails.Add(voteDetails);
                await _dbContext.SaveChangesAsync();
            }

            return new(addedVote.Id);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorAddVoterVote"]} {request.VoterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
