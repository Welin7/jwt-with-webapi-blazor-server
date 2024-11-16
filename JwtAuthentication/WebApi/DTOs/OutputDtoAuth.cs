using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class OutputDtoAuth
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
