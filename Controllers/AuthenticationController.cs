using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _config;
        public AuthenticationController(IConfiguration Configuration)
        {
            _config = Configuration;
        }

        [HttpPost, Route("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Request do cliente inválido");
            }

            MySqlConnection mySqlConnection = new MySqlConnection("server=localhost;user=root;password=15031989;database=webapi");
            await mySqlConnection.OpenAsync();

            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            user.Password = Encryptor.MD5Hash(user.Password);
            mySqlCommand.CommandText = $"SELECT * FROM users WHERE Username = '{user.Username}' and Password = '{user.Password}'";

            MySqlDataReader reader = mySqlCommand.ExecuteReader();

            if (await reader.ReadAsync())
            {
                var _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var _issuer = _config["Jwt:Issuer"];
                var _audience = _config["Jwt:Audience"];
                var signinCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: signinCredentials);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Request do cliente inválido");
            }

            MySqlConnection mySqlConnection = new MySqlConnection("server=localhost;user=root;password=15031989;database=webapi");
            await mySqlConnection.OpenAsync();

            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            user.Password = Encryptor.MD5Hash(user.Password);
            user.RegisterIP = GetIP();
            mySqlCommand.CommandText = $"INSERT INTO users (`Username`,`Password`,`Active`,`UserLevel`,`RegisterDate`,`RegisterIP`) VALUES ('{user.Username}','{user.Password}',{user.Active},{user.UserLevel},NOW(),'{user.RegisterIP}'); ";

            MySqlDataReader reader = mySqlCommand.ExecuteReader();

            if (reader.RecordsAffected > 0)
            {
                return Ok("Usuario criado com Sucesso!");
            }
            else
            {
                return BadRequest("Erro ao criar usuario");
            }
        }

        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout([FromBody] User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
            }
            return Ok("Usuario Deslogado");
        }

        [Obsolete]
        private string GetIP()
        {
            string hostname = Dns.GetHostName();
            string ip = Dns.GetHostByName(hostname).AddressList[3].ToString();
            return ip;
        }
    }
}
