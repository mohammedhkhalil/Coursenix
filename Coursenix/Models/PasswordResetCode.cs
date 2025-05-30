namespace Coursenix.Models
{
    public class ResetCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime Expiry { get; set; }
        public string Purpose { get; set; } // "ResetPassword" or "Register"

    }
}
