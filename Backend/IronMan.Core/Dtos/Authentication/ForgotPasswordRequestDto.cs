using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]

        public string Email { get; set; }

    }
}
