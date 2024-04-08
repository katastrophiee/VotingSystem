using System.ComponentModel.DataAnnotations;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin
{
    public class CreateAdminAccountRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public CustomerCountry Country { get; set; }
    }
}
