using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class CustomerProvider(DBContext dbContext, IStringLocalizer<CustomerProvider> localizer) : ICustomerProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<CustomerProvider> _localizer = localizer;

    public async Task<Response<GetCustomerAccountDetailsResponse>> GetCustomerAccountDetails(int customerId)
    {
        try
        {
            var customer = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer is null || customer.Id == 0)
                return new(new ErrorResponse()
                {
                    //VotingSystem.Resources.API.Providers.CustomerProvider
                    Title = _localizer["NoCustomerFound"], //"No Customer Found",
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

    public async Task<Response<List<GetCandidateResponse>>> GetActiveCandidates(int customerId)
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

            var candidates = await _dbContext.Customer.Where(c =>
                c.IsCandidate == true
                && c.IsActive == true
                && c.IsVerified == true).ToListAsync();
            
            var response = candidates.Select(c => new GetCandidateResponse(c)).ToList();

            foreach(var candidate in response)
            {
                var candidateId = candidate.CandidateId;
                var candidateName = candidate.Name;

                //electionid and optionId not being saved to election options

                var ongoingElections = await _dbContext.Election.Where(e =>
                    e.Country == customer.Country
                    && e.StartDate <= DateTime.Now
                    && e.EndDate > DateTime.Now).ToListAsync();

                var ongoingEnteredElectionsIds = ongoingElections
                    .Where(e => e.ElectionOptions.Any(o => o.CandidateId == customerId))
                    .Select(e => e.Id)
                    .ToList() ?? [];

                candidate.OngoingEnteredElectionsIds = ongoingEnteredElectionsIds;
            }

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve candidates for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> MakeCustomerACandidate(BecomeCandidateRequest request)
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
                    Description = $"Cannot convert to candidate as customer {request.CustomerId} is not verified.",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = "No Name or Description Provided",
                    Description = $"No candidate name or description was provided when trying convert to candidate for customer id {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            customer.IsCandidate = true;
            customer.CandidateName = request.CandidateName;
            customer.CandidateDescription = request.CandidateDescription;
            customer.DateOfCandidacy = DateTime.Now;

            _dbContext.Customer.Update(customer);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to convert to candidate for customer {request.CustomerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetCandidateResponse>> GetCandidate(int customerId, int candidateId)
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

            var candidate = await _dbContext.Customer.Where(c => c.Id == candidateId).FirstOrDefaultAsync();

            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Candidate Found",
                    Description = $"No candidate was found with the customer id {candidate}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new GetCandidateResponse(candidate);

            var ongoingElections = await _dbContext.Election.Where(e =>
                e.Country == customer.Country
                && e.StartDate <= DateTime.Now
                && e.EndDate > DateTime.Now).ToListAsync();

            var ongoingEnteredElectionsIds = ongoingElections
                .Where(e => e.ElectionOptions.Any(o => o.CandidateId == customerId))
                .Select(e => e.Id)
                .ToList() ?? [];

            response.OngoingEnteredElectionsIds = ongoingEnteredElectionsIds;

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to retrieve candidate for customer {customerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateCandidate(UpdateCandidateRequest request)
    {
        try
        {
            var candidate = await _dbContext.Customer.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = "No Customer Found",
                    Description = $"No customer was found with the customer id {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!candidate.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = "Customer Not Verified",
                    Description = $"Cannot update candidate as candidate {request.CustomerId} is not verified.",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = "No Name or Description Provided",
                    Description = $"No candidate name or description was provided when trying update candidate {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            candidate.CandidateName = request.CandidateName;
            candidate.CandidateDescription = request.CandidateDescription;

            _dbContext.Customer.Update(candidate);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "Internal Server Error",
                Description = $"An unknown error occured when trying to update candidate {request.CustomerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
