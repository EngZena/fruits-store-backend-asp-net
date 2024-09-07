namespace FruitsStoreBackendASPNET.Dtos
{
    public partial class UserForLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserForLoginDto()
        {
            Email ??= "";
            Password ??= "";
        }
    }
}
