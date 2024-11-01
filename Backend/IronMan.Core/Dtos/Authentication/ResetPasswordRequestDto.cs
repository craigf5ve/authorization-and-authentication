using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class ResetPasswordRequestDto
    {
        [Required]

        public string Token { get; set; }


        [Required]
        [MinLength(6)]

        public string Password { get; set; }


        [Required]
        [Compare("Password")]

        public string ConfirmPassword { get; set; }

    }
}
