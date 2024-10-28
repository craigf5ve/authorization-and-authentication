using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class ValidateResetTokenRequestDto
    {
        [Required]
        public string Token { get; set; }
    }
}
