using fruits_store_backend_asp_net.Enums;

namespace fruits_store_backend_asp_net.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            FirstName ??= "";
            LastName ??= "";
            Email ??= "";
        }
    }
}
