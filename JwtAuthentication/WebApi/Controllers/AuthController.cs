using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Infrastructure;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataAccess _dataAccess;
        public AuthController(DataAccess dataAccess ) 
        {
            _dataAccess = dataAccess;
        }

        [HttpPost("register")]
        public ActionResult Register(InputDtoRegister input)
        {
            string nashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
            var result = _dataAccess.RegisterUsers(input.Email, nashedPassword, input.Role);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
