namespace fruits_store_backend_asp_net.Models
{
    public partial class RequestResetPassword
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public int Number_Of_Attempts { get; set; }

        public bool IS_VALID { get; set; }

        public DateTime CreatedAt { get; set; }

        public RequestResetPassword()
        {
            Token ??= "";
        }
    }
}
