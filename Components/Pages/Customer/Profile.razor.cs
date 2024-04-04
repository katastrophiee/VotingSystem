using Microsoft.AspNetCore.Components;

namespace VotingSystem.Components.Pages.Customer;

public partial class Profile
{
    // https://stackoverflow.com/questions/51226405/net-core-blazor-app-how-to-pass-data-between-pages 
    [Parameter]
    public int UserId { get; set; }

    public bool Editable { get; set; } = false;

    public API.DTO.DbModels.Customer Customer { get; set; }

    public async Task Update()
    {

        Editable = false;
    }
}
