﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;
using VotingSystem.API.DTO.DbModels.Admin;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Responses;
using VotingSystem.API.Interfaces.Provider;
using VotingSystem.API.Repository.DBContext;

namespace VotingSystem.API.Providers;

public class DocumentProvider(DBContext dbContext, IStringLocalizer<DocumentProvider> localizer) : IDocumentProvider
{
    private readonly DBContext _dbContext = dbContext;
    private readonly IStringLocalizer<DocumentProvider> _localizer = localizer;

    public async Task<Response<Document?>> GetCurrentVoterDocument(int voterId)
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

            var currentDocument = await _dbContext.Document
              .Where(v => v.VoterId == voterId && v.MostRecentId == true)
              .FirstOrDefaultAsync();

            return new(currentDocument);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetCurrentVoterDocument"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<IEnumerable<Document>>> GetVoterDocuments(int voterId)
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

            var documents = await _dbContext.Document
              .Where(v => v.VoterId == voterId)
              .ToListAsync() ?? [];

            return new(documents);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorGetVoterDocuments"]} {voterId}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<Response<int>> UploadVoterDocument(Document document)
    {
        try
        {
            var voter = await _dbContext.Voter.FirstOrDefaultAsync(c => c.Id == document.VoterId);
            if (voter is null || voter.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoVoterFound"],
                    Description = $"{_localizer["NoVoterFoundWithId"]} {document.VoterId}",
                    StatusCode = StatusCodes.Status404NotFound
                });

            var currentIdDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.VoterId == document.VoterId && c.MostRecentId == true);
            if (currentIdDocument != null)
            {
                currentIdDocument.MostRecentId = false;
                _dbContext.Document.Update(currentIdDocument);
            }

            document.VoterId = voter.Id;
            document.UploadedDate = DateTime.UtcNow;
            document.IsVerified = false;
            document.MostRecentId = true;

            voter.IsVerified = false;

            _dbContext.Document.Add(document);
            _dbContext.Voter.Update(voter);
            await _dbContext.SaveChangesAsync();

            var addedDocument = await _dbContext.Document.FirstOrDefaultAsync(c => c.VoterId == document.VoterId && c.MostRecentId == true);
            if (addedDocument is null || addedDocument.Id == 0)
                return new(new ErrorResponse()
                {
                    Title = _localizer["NoDocumentFound"],
                    Description = $"{_localizer["NoDocumentFoundWithId"]} {document.VoterId}",
                    StatusCode = StatusCodes.Status500InternalServerError
                });

            var oldTasks = await _dbContext.AdminTask.Where(c => c.ForVoterId == voter.Id && c.Name == "Verify Voter ID").ToListAsync() ?? [];
            _dbContext.AdminTask.RemoveRange(oldTasks);

            var task = new AdminTask()
            {
                ForVoterId = voter.Id,
                Name = "Verify Voter ID",
                Description = "Verify the uploaded voter ID",
                TaskStatus = Enums.TaskStatus.New,
                CreatedAt = DateTime.Now,
            };

            _dbContext.AdminTask.Add(task);
            await _dbContext.SaveChangesAsync();

            return new(addedDocument.Id);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = _localizer["InternalServerError"],
                Description = $"{_localizer["InternalServerErrorUploadVoterDocument"]} {document.FileName}",
                StatusCode = StatusCodes.Status500InternalServerError,
                AdditionalDetails = ex.Message
            });
        }
    }
}
