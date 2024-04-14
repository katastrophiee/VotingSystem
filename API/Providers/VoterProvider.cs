﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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

            if (request.Country is not null)
                voter.Country = request.Country.Value;

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

    public async Task<Response<List<GetCandidateResponse>>> GetActiveCandidates(int voterId)
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
                && c.IsVerified == true).ToListAsync();
            
            var response = candidates.Select(c => new GetCandidateResponse(c)).ToList();

            foreach(var candidate in response)
            {
                var candidateId = candidate.CandidateId;
                var candidateName = candidate.Name;

                // TO DO - FIX
                //electionid and optionId not being saved to election options

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
                    Description = $"{_localizer["NoNameOrDescriptionProvidedConvertWithId"]} {request.VoterId}",
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

            //TO DO
            // Add option for voter to revoke candidacy

            roles.RoleIds.ToList().Add((int)Enums.Roles.Candidate);

            _dbContext.UserRole.Add(roles);
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
                && e.EndDate > DateTime.Now).ToListAsync();

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
}
