using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Services;
using MovieShop.Entities;
using MovieShop.API.DTO;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public UserController(IUserService userService,IConfiguration configuration)
        {
            _userService = userService;
            _config = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserAsync([FromBody]CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null || string.IsNullOrEmpty(createUserDTO.Email) || string.IsNullOrEmpty(createUserDTO.Password))
            {
                return BadRequest();
            }
            var user = await _userService.Createuser(createUserDTO.Email, createUserDTO.Password, createUserDTO.FirstName, createUserDTO.LastName);
            if (user == null)
            {
                return BadRequest("Email already exit");
            }
            return Ok("Create user successfully");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LoginAsync([FromBody]ValidateUserDTO validateUserDTO)
        {
            var user = await _userService.ValidateUser(validateUserDTO.Email, validateUserDTO.Password);
            if (user == null)
            {
                return Unauthorized("invalid email or password");
            }


            return Ok(new { 
                token= GenerateToken(user)
                });
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/purchases")]
        public async Task<ActionResult> GetUserPurchasedMovies(int id)
        {
            var usermovies = await _userService.GetPurchases(id);
            return Ok(usermovies);
        }

        private string GenerateToken(User user)
        {
            //claim information in payload part
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim("alias", user.FirstName[0] + user.LastName[0].ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenSettings:PrivateKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["TokenSettings:ExpirationDays"]));
            
            // generate the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = credentials,
                Issuer = _config["TokenSettings:Issuer"],
                Audience = _config["TokenSettings:Audience"]
            };


            var encodedJwt = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            return new JwtSecurityTokenHandler().WriteToken(encodedJwt);

        }

    }
}