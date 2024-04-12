using Microsoft.AspNetCore.Routing.Matching;
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

    public async Task<Response<List<GetElectionResponse>>> GetCustomerOngoingElections(int customerId)
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

            var ongoingElections = await _dbContext.Election
                .Where(v => v.Country == customer.Country
                && v.StartDate <= DateTime.Now
                && v.EndDate > DateTime.Now)
                .ToListAsync();

            var response = ongoingElections.Select(e => new GetElectionResponse(e)).ToList();

            foreach (var election in response)
            {
                var vote = await _dbContext.Vote
                   .Where(v => v.CustomerId == customerId
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
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch ongoing elections for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetCustomerVotedInElections(int customerId)
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

            var electionInRegion = await _dbContext.Election
                .Where(e => e.Country == customer.Country)
                .ToListAsync();

            var customerVotes = await _dbContext.Vote.Where(v => v.CustomerId == customerId).ToListAsync();

            var votedInElectionInRegion = electionInRegion
                .Where(e => customerVotes.Any(v => v.ElectionId == e.Id))
                .ToList();

            var response = votedInElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch voted in elections for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetRecentlyEndedElections(int customerId)
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

            var recentElectionInRegion = await _dbContext.Election.Where(e =>
                e.Country == customer.Country
                && e.EndDate < DateTime.Now
                && e.EndDate <= DateTime.Now.AddMonths(3))
               .ToListAsync();

            var response = recentElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch recently ended elections for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetCustomerUpcomingElections(int customerId)
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

            var upcomingElectionInRegion = await _dbContext.Election.Where(e =>
                e.Country == customer.Country
                && e.StartDate > DateTime.Now
                && e.StartDate <= DateTime.Now.AddMonths(3))
               .ToListAsync();

            var response = upcomingElectionInRegion.Select(e => new GetElectionResponse(e)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch upcoming elections for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetElectionResponse>> GetElection(int electionId, int customerId)
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

            var election = await _dbContext.Election.FirstOrDefaultAsync(c => c.Id == electionId);
            if (election is null || election.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Election Found",
                    Description = $"No election was found with the election id {electionId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var vote = await _dbContext.Vote
                  .Where(v => v.CustomerId == customerId
                  && v.ElectionId == election.Id)
                  .FirstOrDefaultAsync();

            var response = new GetElectionResponse(election);

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
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch election for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<GetElectionResponse>>> GetElections(GetElectionsRequest request)
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

            var elections = await _dbContext.Election.Where(e => request.ElectionIds.Contains(e.Id)).ToListAsync() ?? [];

            var response = elections.Select(c => new GetElectionResponse(c)).ToList();

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to fetch elections for customer {request.CustomerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
