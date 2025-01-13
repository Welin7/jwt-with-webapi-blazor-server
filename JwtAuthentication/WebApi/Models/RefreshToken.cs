namespace WebApi.Models
{
    public class RefreshToken
    {
        public required string Token { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
        public bool Enabled { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
