namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForSignUpDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserForSignUpDto()
        {
            Email ??= "";

            Password ??= "";
        }
    }
}
