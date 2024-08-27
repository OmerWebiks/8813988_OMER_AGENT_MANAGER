using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManagementOfMossadAgentsAPI.api.Del;
using ManagementOfMossadAgentsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ManagementOfMossadAgentsAPI.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private readonly ManagementOfMossadAgentsDbContext _context;

        public Login(ManagementOfMossadAgentsDbContext context)
        {
            _context = context;
        }

        private string GenerateToken(string userIP)
        {
            // token handler can create token
            var tokenHandler = new JwtSecurityTokenHandler();

            string secretKey = "ManagementOfMossadAgentsAPIManagementOfMossadAgentsAPI"; //TODO: remove this from code
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            // token descriptor describe HOW to create the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // things to include in the token
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userIP), }),
                // expiration time of the token
                Expires = DateTime.UtcNow.AddSeconds(1000),
                // the secret key of the token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // creating the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // converting the token to string
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        [HttpPost]
        public IActionResult LoginUser(User user)
        {
            if (user.Id == "MVC" || user.Id == "SimulationServer")
            {
                string userIP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                return StatusCode(200, new { token = GenerateToken(userIP) });
            }
            return StatusCode(
                StatusCodes.Status401Unauthorized,
                new { error = "invalid credentials" }
            );
        }
    }
}
