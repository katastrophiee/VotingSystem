﻿using Azure.Core;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

    public async Task<Response<List<GetOngoingElectionsResponse>>> GetCustomerOngoingElections(int customerId)
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
                .Where(v => v.Country == customer.Country)
                .ToListAsync();

            var response = ongoingElections.Select(e => new GetOngoingElectionsResponse(e)).ToList();

            response.ForEach(async e =>
            {
                var vote = await _dbContext.Vote
                   .Where(v => v.CustomerId == customerId && v.ElectionId == e.ElectionId)
                   .FirstOrDefaultAsync();

                if (vote is not null)
                    e.HasVoted = true;
            });

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
}
