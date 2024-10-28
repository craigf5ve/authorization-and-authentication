using IronMan.Core.Dtos.EntityDtos;
using System.Text.Json.Serialization;

namespace IronMan.Core.Dtos.Authentication
{
    public class AuthenticateResponseDto : EntityDto<int>
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActivated { get; set; }
        public string JwtToken { get; set; }
        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

    }
}
