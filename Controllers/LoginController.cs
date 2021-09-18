using contactos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;

namespace contactos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController: Controller
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] Usuario login)
        {
            IActionResult response = Unauthorized();

            var user = AuthenticateUser(login);

            if(user != null)
            {
                var tokenString = GenerateJwt(user);
                response = Ok(new {token = tokenString});
            }

            return response;
        }

        private Usuario AuthenticateUser(Usuario login)
        {
            Usuario user = null;

            if(login.username == "userTest")
            {
                //user = new Usuario {username = "userTest", password = "1234"};
                user = new Usuario {username = login.username, password = login.password, email = login.email, fechaCreado = login.fechaCreado};
            }

            return user;
        }

        private string GenerateJwt(Usuario user)
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