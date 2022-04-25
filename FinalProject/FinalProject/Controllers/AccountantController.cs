using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountantController : Controller
    {
        private readonly IAccountantService _accountant;
        private readonly AppSettings _appSettings;
        public AccountantController(IAccountantService accountant, IOptions<AppSettings> appSettings)
        {
            _accountant = accountant;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(int Id, string userName, string password, string role)
        {
            var accountantLogin = _accountant.Login(Id, userName, password, role);
            if (accountantLogin == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            string tokenString = GenerateToken(accountantLogin);
            return Ok(new
            {
                Id = accountantLogin.Id,
                AccountantId = accountantLogin.AccountanId,
                FirstName = accountantLogin.FirstName,
                LastName = accountantLogin.LastName,
                UserName = accountantLogin.UserName,
                Role = accountantLogin.Role,
                Token = tokenString
            });
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpPost("addUser")]
        public IActionResult AddUser(Users user, int id)
        {
            var validator = new UserValidator();
            var validatorResult = validator.Validate(user);
            if (!validatorResult.IsValid)
            {
                return BadRequest(validatorResult.Errors[0].ErrorMessage);
            }
            _accountant.AddUser(user);
            return CreatedAtAction("GetUserById", new { id = user.Id }, user);
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpGet("filter")]
        public IActionResult FilterUser(int age, double sallary)
        {

            return Ok(_accountant.Filter(age, sallary));
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpDelete("deleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            _accountant.DeleteUser(id);
            return Ok($"User with id {id} was deleted");
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpGet("getALlUser")]
        public IActionResult GetAllUser()
        {
            return Ok(_accountant.GetAllUser());
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpGet("getUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            return Ok(_accountant.GetUserById(id));
        }

        [Authorize(Roles = Role.Accountant)]
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, Users user)
        {
            _accountant.UpdateUser(id, user);
            return Ok(user);
        }

        private string GenerateToken(Accountant accountant)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, accountant.Id.ToString()),
                    new Claim(ClaimTypes.Role, accountant.Role.ToString()),
               }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return tokenString;
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException(message: "Do not leave fields empty");
            }
        }
    }
}
