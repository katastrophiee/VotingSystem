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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new GetCustomerAccountDetailsResponse(customer);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCustomerAccountDetails"]} {customerId}",
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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {request.UserId}",
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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorPutUpdateCustomerProfile"]} {request.UserId}",
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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {customerId}",
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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetActiveCandidates"]} {customerId}",
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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!customer.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = _localizer["CustomerNotVerified"],
                    Description = $"{_localizer["CustomerNotVerifiedWithId"]} {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoNameOrDescriptionProvided"],
                    Description = $"{_localizer["NoNameOrDescriptionProvidedConvertWithId"]} {request.CustomerId}",
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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorMakeCustomerACandidate"]} {request.CustomerId}",
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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {customerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var candidate = await _dbContext.Customer.Where(c => c.Id == candidateId).FirstOrDefaultAsync();

            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCandidateFound"],
                    Description = $"{_localizer["NoCandidateFoundWithId"]} {candidate}",
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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCandidate"]} {customerId}",
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
                    Title = _localizer["NoCustomerFound"],
                    Description = $"{_localizer["NoCustomerFoundWithId"]} {request.CustomerId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!candidate.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = _localizer["CustomerNotVerified"],
                    Description = $"{_localizer["CustomerNotVerifiedWithId"]} {request.CustomerId}",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoNameOrDescriptionProvided"],
                    Description = $"{_localizer["NoNameOrDescriptionProvidedUpdateWithId"]} {request.CustomerId}",
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
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateCandidate"]} {request.CustomerId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
