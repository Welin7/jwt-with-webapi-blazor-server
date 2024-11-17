using BCrypt.Net;
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
        private readonly TokenProvider _tokenProvider;
        public AuthController(DataAccess dataAccess, TokenProvider tokenProvider) 
        {
            _dataAccess = dataAccess;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("register")]
        public ActionResult Register(InputDtoRegister input)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
            var result = _dataAccess.RegisterUsers(input.Email, hashedPassword, input.Role);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public ActionResult<OutputDtoAuth> Login(InputDtoAuth input)
        {
            OutputDtoAuth response = new OutputDtoAuth();
            
            var user = _dataAccess.FindUserByEmail(input.Email);

            if (user == null) 
                return BadRequest("User is not found.");

            var storedHash = user.Password.Trim();
            var verifyPassword = BCrypt.Net.BCrypt.Verify(input.Password, storedHash);

            if (verifyPassword == false)
                return BadRequest("Wrong password.");

            //Generate access token
            var token = _tokenProvider.GenerateToken(user);
            response.AccessToken = token.AccessToken;
            
            //Generate refresh token

            return response;
        }
    }
}
