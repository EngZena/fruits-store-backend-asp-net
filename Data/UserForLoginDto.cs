using System.ComponentModel.DataAnnotations;

namespace fruits_store_backend_asp_net.Dtos
{
    public partial class UserForLoginDto
    {
        /// <summary>
        /// The email of the user
        /// </summary>
        /// <example>email@gmail.com</example>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public UserForLoginDto()
        {
            Email ??= "";
            Password ??= "";
        }
    }
}
