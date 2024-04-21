using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class VoterProvider(DBContext dbContext, IStringLocalizer<VoterProvider> localizer) : IVoterProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<VoterProvider> _localizer = localizer;

    public async Task<Response<GetVoterAccountDetailsResponse>> GetVoterAccountDetails(int voterId)
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

            var response = new GetVoterAccountDetailsResponse(voter);

            return new(response);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterAccountDetails"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> PutUpdateVoterProfile(UpdateVoterProfileRequest request)
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

            if (!string.IsNullOrEmpty(request.Email))
                voter.Email = request.Email;

            if (!string.IsNullOrEmpty(request.FirstName))
                voter.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                voter.LastName = request.LastName;

            if (!string.IsNullOrEmpty(request.Address))
                voter.Address = request.Address;

            if (request.DateOfBirth != new DateTime())
                voter.DateOfBirth = request.DateOfBirth;

            if (request.Country is not null)
                voter.Country = request.Country.Value;

            if (request.Password != null)
            {
                string newSalt = GenerateSalt();
                voter.PasswordSalt = newSalt;

                var pbkdf2HashedPassword = request.Password.Pbkdf2HashString(ref newSalt);

                voter.Password = pbkdf2HashedPassword;
            }

            voter.IsVerified = false;

            _dbContext.Voter.Update(voter);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorPutUpdateVoterProfile"]} {request.VoterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<IEnumerable<GetCandidateResponse>>> GetActiveCandidates(int voterId)
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

            var candidates = await _dbContext.Voter.Where(c =>
                c.IsCandidate == true
                && c.IsActive == true
                && c.IsVerified == true).ToListAsync() ?? [];
            
            var response = candidates.Select(c => new GetCandidateResponse(c)).ToList();

            foreach(var candidate in response)
            {
                var candidateId = candidate.CandidateId;
                var candidateName = candidate.Name;

                var ongoingElections = await _dbContext.Election.Where(e =>
                    e.Country == voter.Country
                    && e.StartDate <= DateTime.Now
                    && e.EndDate > DateTime.Now).ToListAsync();

                var ongoingEnteredElectionsIds = ongoingElections
                    .Where(e => e.ElectionOptions.Any(o => o.CandidateId == voterId))
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
                Description = $"{_localizer["InternalServerErrorGetActiveCandidates"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> MakeVoterACandidate(BecomeCandidateRequest request)
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
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoNameOrDescriptionProvided"],
                    Description = $"{_localizer["NoNameOrDescriptionProvidedWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            voter.IsCandidate = true;
            voter.CandidateName = request.CandidateName;
            voter.CandidateDescription = request.CandidateDescription;
            voter.DateOfCandidacy = DateTime.Now;

            _dbContext.Voter.Update(voter);
            await _dbContext.SaveChangesAsync();

            var roles = await _dbContext.UserRole.Where(r => r.UserId == request.VoterId).FirstOrDefaultAsync();
            if (roles is null)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoRolesFound"],
                    Description = $"{_localizer["NoRolesFoundWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            roles.RoleIds.ToList().Add((int)Enums.Roles.Candidate);

            _dbContext.UserRole.Update(roles);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorMakeVoterACandidate"]} {request.VoterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<GetCandidateResponse>> GetCandidate(int voterId, int candidateId)
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

            var candidate = await _dbContext.Voter.Where(c => c.Id == candidateId).FirstOrDefaultAsync();

            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoCandidateFound"],
                    Description = $"{_localizer["NoCandidateFoundWithId"]} {candidate}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var response = new GetCandidateResponse(candidate);

            var ongoingElections = await _dbContext.Election.Where(e =>
                e.Country == voter.Country
                && e.StartDate <= DateTime.Now
                && e.EndDate > DateTime.Now).ToListAsync() ?? [];

            var ongoingEnteredElectionsIds = ongoingElections
                .Where(e => e.ElectionOptions.Any(o => o.CandidateId == voterId))
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
                Description = $"{_localizer["InternalServerErrorGetCandidate"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> UpdateCandidate(UpdateCandidateRequest request)
    {
        try
        {
            var candidate = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == request.VoterId);
            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!candidate.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = _localizer["VoterNotVerified"],
                    Description = $"{_localizer["VoterNotVerifiedWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            if (string.IsNullOrEmpty(request.CandidateName) || string.IsNullOrEmpty(request.CandidateDescription))
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoNameOrDescriptionProvided"],
                    Description = $"{_localizer["NoNameOrDescriptionProvidedUpdateWithId"]} {request.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            candidate.CandidateName = request.CandidateName;
            candidate.CandidateDescription = request.CandidateDescription;

            _dbContext.Voter.Update(candidate);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUpdateCandidate"]} {request.VoterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> GetInPersonVotingEligibility(int voterId)
    {
        try
        {
            var user = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == voterId);
            if (user is null || user.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            if (!user.IsVerified)
                return new(new ErrorResponse()
                {
                    Title = _localizer["UserNotVerified"],
                    Description = $"{_localizer["UserNotVerifiedWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            if (!user.IsActive)
                return new(new ErrorResponse()
                {
                    Title = _localizer["UserNotActive"],
                    Description = $"{_localizer["UserNotActiveWithId"]} {voterId}",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetInPersonVotingEligibility"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<bool>> RevokeCandidacy(int candidateId)
    {
        try
        {
            var candidate = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == candidateId);
            if (candidate is null || candidate.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {candidateId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            candidate.IsCandidate = false;
            candidate.CandidateName = null;
            candidate.CandidateDescription = null;
            candidate.DateOfCandidacy = null;

            _dbContext.Voter.Update(candidate);
            await _dbContext.SaveChangesAsync();

            var roles = await _dbContext.UserRole.Where(r => r.UserId == candidateId).FirstOrDefaultAsync();
            if (roles is null)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoRolesFound"],
                    Description = $"{_localizer["NoRolesFoundWithId"]} {candidateId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            roles.RoleIds.ToList().Remove((int)Enums.Roles.Candidate);

            _dbContext.UserRole.Update(roles);
            await _dbContext.SaveChangesAsync();

            return new(true);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorRevokeCandidacy"]} {candidateId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    private static string GenerateSalt(int size = 32)
    {
        var buff = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buff);

        return Convert.ToBase64String(buff);
    }
}
