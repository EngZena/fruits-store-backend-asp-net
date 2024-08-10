namespace FruitsStoreBackendASPNET.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public bool Active { get; set; }

        public User()
        {
            FirstName ??= "";
            LastName ??= "";
            Email ??= "";
            Gender ??= "";
        }
    }
}