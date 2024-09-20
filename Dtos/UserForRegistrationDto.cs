using System.ComponentModel.DataAnnotations;
using FruitsStoreBackendASPNET.Enums;

namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForRegistrationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "PasswordConfirm is required.")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(Gender), ErrorMessage = "Invalid gender value.")]
        public Gender Gender { get; set; }

        public UserForRegistrationDto()
        {
            Email ??= "";

            Password ??= "";

            PasswordConfirm ??= "";

            FirstName ??= "";

            LastName ??= "";
        }
    }
}
