using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class InputDtoAuth
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
