using Microsoft.EntityFrameworkCore;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class CustomerProvider(DBContext dbContext) : ICustomerProvider
{
    private readonly DBContext _dbContext = dbContext;

    public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId)
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

            var response = new GetCustomerAccountDetailsResponse(customer);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

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

    public async Task<Response<bool>> PostUploadCustomerDocument(Document document)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == document.CustomerId);

            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {document.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.CustomerId == document.CustomerId && c.MostRecentId == true);
            if (currentIdDocument != null)
            {
                currentIdDocument.MostRecentId = false;
                _dbContext.Document.Update(currentIdDocument);
            }

            document.CustomerId = customer.Id;
            document.UploadedDate = DateTime.UtcNow;
            document.IsVerified = false;
            document.MostRecentId = true;

            _dbContext.Document.Add(document);
            await _dbContext.SaveChangesAsync();

            // TO DO
            // add task for admins to verify the uploaded id, then sets the account to verified
            // ensure admins set the expirey date for the id by viewing it

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to upload document {document.FileName}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<List<Document>>> GetCustomerDocuments(int customerId)
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

            var documents = await _dbContext.Document
              .Where(v => v.CustomerId == customerId)
              .ToListAsync();

            return new(documents);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve documents for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> PutUpdateCustomerProfile(UpdateCustomerProfileRequest request)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == request.UserId);
            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {request.UserId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!string.IsNullOrEmpty(request.Email))
                customer.Email = request.Email;

            if (!string.IsNullOrEmpty(request.FirstName))
                customer.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                customer.LastName = request.LastName;

            if (request.Country is not null)
                customer.Country = request.Country.Value;

            _dbContext.Customer.Update(customer);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to update profile for customer {request.UserId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

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

    public async Task<Response<Document?>> GetCurrentCustomerDocument(int customerId)
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

            var currentDocument = await _dbContext.Document
              .Where(v => v.CustomerId == customerId && v.MostRecentId == true)
              .FirstOrDefaultAsync();

            return new(currentDocument);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve documents for customer {customerId}",
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

    public async Task<Response<List<GetElectionResponse>>> GetUpcomingEndedElections(int customerId)
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
                Description = $"An unknown error occured when trying to fetch recently ended elections for customer {customerId}",
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
