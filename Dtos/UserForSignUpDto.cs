using System.ComponentModel.DataAnnotations;

namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForSignUpDto
    {
        /// <summary>
        /// The email of the user
        /// </summary>
        /// <example>user@gmail.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public UserForSignUpDto()
        {
            Email ??= "";

            Password ??= "";
        }

        public UserForSignUpDto(string UserEmail, string UserPassword)
        {
            Email ??= UserEmail;

            Password ??= UserPassword;
        }
    }
}
