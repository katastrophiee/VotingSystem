using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class VoteProvider(DBContext dbContext, IStringLocalizer<VoteProvider> localizer) : IVoteProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<VoteProvider> _localizer = localizer;

    public async Task<Response<IEnumerable<GetVotingHistoryResponse>>> GetCustomerVotingHistory(int customerId)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var votes = await _dbContext.Vote
               .Where(v => v.CustomerId == customerId)
               .ToListAsync();

            var response = new List<GetVotingHistoryResponse>();
            votes?.ForEach(vote => response.Add(new(vote)));

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve voting history for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> AddCustomerVote(AddCustomerVoteRequest request)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!customer.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = "Customer Not Verified",
                    Description = $"Cannot vote as customer {request.CustomerId} is not verified.",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var election = await _dbContext.Election.FirstOrDefaultAsync(c => c.Id == request.ElectionId);
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Election Found",
                    Description = $"No election was found with the election id {request.ElectionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var vote = new Vote()
            {
                ElectionId = election.Id,
                ElectionName = election.ElectionName,
                ElectionDescription = election.ElectionDescription,
                VoteDate = DateTime.Now,
                CustomerId = customer.Id,
                Country = request.Country
            };

            _dbContext.Vote.Add(vote);
            await _dbContext.SaveChangesAsync();

            var customerVote = await _dbContext.Vote.Where(v =>
                v.ElectionId == election.Id
                && v.CustomerId == customer.Id)
                .FirstOrDefaultAsync();

            if (customerVote is null || customerVote.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Vote Found",
                    Description = $"No vote was found by customer {request.CustomerId} for election {request.ElectionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var voteDetails = new VoteDetails()
            {
                VoteId = customerVote.Id,
                ElectionType = election.ElectionType,
                Choices = request.Choices,
                ElectionTypeAdditionalInfo = request.ElectionTypeAdditionalInfo
            };

            _dbContext.VoteDetails.Add(voteDetails);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to add vote for customer {request.CustomerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
