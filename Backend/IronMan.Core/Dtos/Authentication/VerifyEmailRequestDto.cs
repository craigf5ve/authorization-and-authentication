using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class VerifyEmailRequestDto
    {
        [Required]

        public string Token { get; set; }

    }
}
