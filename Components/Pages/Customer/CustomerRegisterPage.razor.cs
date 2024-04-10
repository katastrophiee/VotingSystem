﻿using Microsoft.AspNetCore.Components;
using VotingSystem.API.DTO.Requests;

namespace VotingSystem.Components.Pages.Customer;

public partial class CustomerRegisterPage
{
    private CreateCustomerAccountRequest CreateAccountRequest = new();

    [Parameter]
    public EventCallback<CreateCustomerAccountRequest> OnCreation { get; set; }

    [Parameter]
    public EventCallback<bool> Return { get; set; }

    public bool ShowLoading { get; set; } = false;

    private async Task HandleRegistration()
    {
        ShowLoading = true;
        await OnCreation.InvokeAsync(CreateAccountRequest);
        ShowLoading = false;
    }
}
