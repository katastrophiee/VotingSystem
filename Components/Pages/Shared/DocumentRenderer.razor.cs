using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using VotingSystem.API.DTO.DbModels;

namespace VotingSystem.Components.Pages.Shared;

public partial class DocumentRenderer
{
    [Parameter]
    public Document Document { get; set; }

    [Inject]
    public IStringLocalizer<DocumentRenderer> Localizer { get; set; }
}
