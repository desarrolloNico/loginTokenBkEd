using contactos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;

using contactos.Service;

namespace contactos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController: Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public LoginController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserDto login)
        {
            IActionResult response = Unauthorized();

            //var user = AuthenticateUser(login);
            var user = _userService.Authenticate(login.username, login.password);

            if(user != null)
            {
                var tokenString = GenerateJwt(user);
                response = Ok(new {token = tokenString});
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        public IActionResult Registro([FromBody] UserDto userDto)
        {
            User user = new User();
            user.username = userDto.username;
            user.email = userDto.email;
            user.fechaCreado = userDto.fechaCreado;

            try
            {
                _userService.Create(user, userDto.password);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        private string GenerateJwt(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new [] {
                new Claim(JwtRegisteredClaimNames.Sub, user.username),
                new Claim(JwtRegisteredClaimNames.Sub, user.username),
                new Claim("FechaCreado", user.fechaCreado.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
                );
            
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}