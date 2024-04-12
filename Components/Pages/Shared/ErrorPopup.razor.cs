using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.ErrorHandling;

namespace VotingSystem.Components.Pages.Shared;

public partial class ErrorPopup
{
    [Parameter]
    public List<ErrorResponse> Errors { get; set; }

    [Inject]
    public IStringLocalizer<ErrorPopup> Localizer { get; set; }
}
