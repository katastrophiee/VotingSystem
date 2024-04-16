using System.ComponentModel.DataAnnotations;
using VotingSystem.API.Enums;

namespace VotingSystem.API.DTO.Requests.Admin
{
    public class AdminCreateAdminAccountRequest
    {
        [Required]
        public int RequestingAdminId { get; set; }

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
        public UserCountry Country { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
