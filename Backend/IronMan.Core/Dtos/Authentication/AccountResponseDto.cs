using IronMan.Core.Dtos.EntityDtos;

namespace IronMan.Core.Dtos.Authentication
{
    public class AccountResponseDto : EntityDto<int>
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int LocationId { get; set;}
        public string LocationName {get; set;}
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActivated { get; set; }
    }
}
