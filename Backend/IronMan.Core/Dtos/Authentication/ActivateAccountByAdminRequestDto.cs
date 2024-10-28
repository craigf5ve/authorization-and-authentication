using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class ActivateAccountByAdminRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
