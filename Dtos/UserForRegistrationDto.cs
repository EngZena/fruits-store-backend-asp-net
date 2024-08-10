namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public UserForRegistrationDto()
        {
            Email ??= "";

            Password ??= "";

            PasswordConfirm ??= "";

            FirstName ??= "";

            LastName ??= "";

            Gender ??= "";

        }

    }
}