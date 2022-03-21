using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TodoListAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using TodoListAPI.Contexts;
using Microsoft.Extensions.Configuration;

namespace TodoListAPI.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private UserManager<User> _uManager;
        private IConfiguration _config;
        
        public UserController(UserManager<User> uManager, IConfiguration config)
        {
            _uManager = uManager;
            _config = config;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Hello()
        {
            return Ok();
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginBody body)
        {
            try
            {
                var user = await _uManager.FindByEmailAsync(body.email);
                if (user == null)
                {
                    return NotFound();
                }

                var passwordCheck = await _uManager.CheckPasswordAsync(user, body.password);
                if (!passwordCheck)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    token = JwtEncryption(user)
                });
            } catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpBody body)
        {
            try
            {
                var user = await _uManager.FindByEmailAsync(body.email);
                if(user != null)
                {
                    return BadRequest("Email already exists.");
                }

                var newUser = new User
                {
                    UserName = body.username,
                    Email = body.email
                };
                var result = await _uManager.CreateAsync(newUser, body.password);
                if(!result.Succeeded)
                {
                    return BadRequest("Password length must be atleast 10 characters.");
                }

                return Ok(new
                {
                    token = JwtEncryption(newUser)
                }); ;

            }catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        private string JwtEncryption(User user)
        {
            string key = _config["Jwt:Secret"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>();
            permClaims.Add(new Claim("userid", user.Id));
            permClaims.Add(new Claim("username", user.UserName));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer,
                            audience,
                            permClaims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }
    }
    
    //private string GenerateToken(int length)
    //{
    //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=";
    //    var random = new Random();
    //    var result = "";

    //    for (int i = 0; i < length; ++i)
    //    {
    //        result += chars[random.Next(chars.Length)];
    //    }

    //    return result;
    //}
}
