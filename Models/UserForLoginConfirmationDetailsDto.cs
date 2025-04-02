namespace fruits_store_backend_asp_net.Models
{
    public partial class UserForLoginConfirmationDetailsDto
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public UserForLoginConfirmationDetailsDto()
        {
            PasswordHash ??= new byte[0];
            PasswordSalt ??= new byte[0];
        }
    }
}
