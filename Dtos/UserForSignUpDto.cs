using System.ComponentModel.DataAnnotations;

namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForSignUpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public UserForSignUpDto()
        {
            Email ??= "";

            Password ??= "";
        }
    }
}
