using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.Requests;

namespace VotingSystem.Components.Pages.Voter;

public partial class VoterRegisterPage
{
    private CreateVoterAccountRequest CreateAccountRequest = new();

    [Parameter]
    public EventCallback<CreateVoterAccountRequest> OnCreation { get; set; }

    [Parameter]
    public EventCallback<bool> Return { get; set; }

    [Inject]
    public IStringLocalizer<VoterRegisterPage> Localizer { get; set; }

    public bool ShowLoading { get; set; } = false;

    private async Task HandleRegistration()
    {
        ShowLoading = true;
        await OnCreation.InvokeAsync(CreateAccountRequest);
        ShowLoading = false;
    }
}
