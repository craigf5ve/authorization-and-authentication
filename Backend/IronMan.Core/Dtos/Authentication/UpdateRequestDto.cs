using IronMan.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace IronMan.Core.Dtos.Authentication
{
    public class UpdateRequestDto
    {
        private string _role;
        private string _email;
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EnumDataType(typeof(Role))]
        public string Role
        {
            get => _role;
            set => _role = replaceEmptyWithNull(value);
        }
        [EmailAddress]
        public string Email
        {
            get => _email;
            set => _email = replaceEmptyWithNull(value);
        }
        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
