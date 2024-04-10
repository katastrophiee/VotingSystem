﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.ErrorHandling;
using VotingSystem.API.DTO.Requests;
using VotingSystem.API.DTO.Responses;
using VotingSystem.Services;

namespace VotingSystem.Components.Pages.Customer;

public partial class BecomeCandidate
{
    [Inject]
    public IApiRequestService ApiRequestService { get; set; }

    [Inject]
    public ILocalStorageService _localStorage { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public List<ErrorResponse> Errors { get; set; } = [];
    public static int CustomerId { get; set; }

    public GetCustomerAccountDetailsResponse? CustomerDetails { get; set; }

    public BecomeCandidateRequest BecomeCandidateRequest { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        CustomerId = await _localStorage.GetItemAsync<int>("currentUserId");

        var customerDetails = await ApiRequestService.GetCustomerInfo(CustomerId);
        if (customerDetails.Error == null)
        {
            //Trim as white space is being added to existing strings
            customerDetails.Data.FirstName = customerDetails.Data.FirstName.Trim();
            customerDetails.Data.LastName = customerDetails.Data.LastName.Trim();
            CustomerDetails = customerDetails.Data;
            BecomeCandidateRequest.CandidateName = $"{CustomerDetails.FirstName} {CustomerDetails.LastName}";
            BecomeCandidateRequest.CustomerId = CustomerId;
        }
        else
            Errors.Add(customerDetails.Error);

    }

    public async Task HandleValidSubmit()
    {
        var response = await ApiRequestService.PutMakeCustomerACandidate(BecomeCandidateRequest);
        if (response.Error == null)
            NavigationManager.NavigateTo("/view-candidate");
        else
            Errors.Add(response.Error);
    }
}
