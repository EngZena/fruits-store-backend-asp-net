using FruitsStoreBackendASPNET.Enums;

namespace FruitsStoreBackendASPNET.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }

        public User()
        {
            FirstName ??= "";
            LastName ??= "";
            Email ??= "";
        }
    }
}
