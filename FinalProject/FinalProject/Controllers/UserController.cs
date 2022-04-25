using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.Services;
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
    public class UserController : Controller
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        public UserController(IUserService userService, IOptions<AppSettings> appSettings)
        {

            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string userName, string password, int loanId, string role)
        {
            var userLogin = _userService.Login(userName, password, loanId, role);
            if (userLogin == null)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            string tokenString = GenerateToken(userLogin);
            return Ok(new
            {
                Id = userLogin.Id,
                AccountantId = userLogin.AccountanId,
                LoanId = userLogin.LoanID,
                FirstName = userLogin.FirstName,
                LastName = userLogin.LastName,
                Age = userLogin.Age,
                Email = userLogin.Email,
                Sallary = userLogin.Sallary,
                UserName = userLogin.UserName,
                Role = userLogin.Role,
                IsBlocked = userLogin.IsBlocked,
                Token = tokenString
            });
        }
        [Authorize(Roles = Role.User)]
        [HttpPost("AddLoan")]
        public IActionResult AddLoan(int userLoanId, Loan loan)
        {
            var currentUserLoanId = int.Parse(User.Identity.Name);
            if (currentUserLoanId != userLoanId || currentUserLoanId != loan.LoanID)
            {
                return Forbid("You do not have access to the method");
            }
            _userService.AddLoan(loan);
            return CreatedAtAction("getAllLoan", new { userLoanId = loan.LoanID }, loan);
        }

        [Authorize(Roles = Role.User)]
        [HttpGet("getAllLoan")]
        public IActionResult GetAllLoan(int userLoanId)
        {
            var currentUserLoanId = int.Parse(User.Identity.Name);
            try
            {
                if (currentUserLoanId != userLoanId)
                {
                    return Forbid("You do not have access to the method");
                }
                return Ok(_userService.GetAllLoan(userLoanId));
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(message: "You do not have access to the method");
            }

        }

        [Authorize(Roles = Role.User)]
        [HttpPut("UpdateLoan")]
        public IActionResult UpdateLoan(int userLoanId, Loan loan)
        {
            try
            {
                var currentUserLoanId = int.Parse(User.Identity.Name);
                if (currentUserLoanId != userLoanId)
                {
                    return Forbid("You do not have access to the method");
                }
                _userService.UpdateLoan(loan.Id, loan);
                return Ok(loan);
            }
            catch (NullReferenceException)
            {
                throw new InvalidOperationException(message: "You do not have access to the method");
            }

        }

        [Authorize(Roles = Role.User)]
        [HttpDelete("deleteLoan")]
        public IActionResult DeleteLoan(int userLoanId, int id)
        {
            var currentUserLoanId = int.Parse(User.Identity.Name);
            if (currentUserLoanId != userLoanId || userLoanId == 0)
            {
                return Forbid("You do not have access to the method");
            }
            _userService.DeleteLoan(id);
            return Ok($"Loan with id {id} was deleted");
        }


        private string GenerateToken(Users users)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, users.LoanID.ToString()),
                    new Claim(ClaimTypes.Role, users.Role.ToString()),
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
