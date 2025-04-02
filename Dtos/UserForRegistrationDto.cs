using System.ComponentModel.DataAnnotations;
using fruits_store_backend_asp_net.Enums;

namespace fruits_store_backend_asp_net.Dtos
{
    public partial class UserForRegistrationDto
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

        /// <summary>
        /// The password confirm of the user
        /// </summary>
        [Required(ErrorMessage = "PasswordConfirm is required.")]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        /// <example>name</example>
        [Required(ErrorMessage = "FirstName is required.")]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        /// <example>name</example>
        [Required(ErrorMessage = "LastName is required.")]
        public string LastName { get; set; }

        /// <summary>
        /// The gender of the user
        /// </summary>
        /// <example>Female</example>
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
