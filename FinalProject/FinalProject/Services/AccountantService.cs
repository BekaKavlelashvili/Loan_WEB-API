using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.PersonContext;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject.Services
{
    public interface IAccountantService
    {
        Accountant Login(int Id, string userName, string password, string role);
        Users AddUser(Users user);
        IEnumerable<Users> Filter(int age, double sallary);
        IEnumerable<Users> GetAllUser();
        Users GetUserById(int id);
        Users UpdateUser(int id, Users user);
        IEnumerable<Users> DeleteUser(int id);
    }
    public class AccountantService : IAccountantService
    {
        readonly ApplicationContext _context;
        public AccountantService(ApplicationContext context)
        {
            _context = context;
        }

        public Accountant Login(int Id, string userName, string password, string role)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            var user = _context.accountants.SingleOrDefault(x => x.UserName == userName);
            if (user == null)
            {
                return null;
            }
            if (password != user.Password)
            {
                return null;
            }
            return user;
        }


        public Users AddUser(Users user)
        {
            var hashing = new PasswordHasher();
            var hash = hashing.HashToString(user.Password);
            var isValid = hashing.Verify(user.Password, hash);
            if (isValid)
            {
                user.Password = hash;
            }
            var newUser = new Users()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                LoanID = user.LoanID,
                AccountanId = user.AccountanId,
                Password = user.Password,
                Age = user.Age,
                Email = user.Email,
                Sallary = user.Sallary,
                Role = user.Role,
                IsBlocked = user.IsBlocked
            };
            _context.users.Add(newUser);
            _context.SaveChanges();
            return newUser;
        }


        public IEnumerable<Users> DeleteUser(int id)
        {
            var userIdforDelete = _context.users.SingleOrDefault(x => x.Id == id);
            if (userIdforDelete == null)
            {
                return null;
            }
            _context.users.Remove(userIdforDelete);
            _context.SaveChanges();
            return _context.users.ToList();
        }


        public IEnumerable<Users> GetAllUser()
        {
            return _context.users.ToList();
        }


        public Users GetUserById(int id)
        {
            var user = _context.users.SingleOrDefault(x => x.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }


        public Users UpdateUser(int id, Users user)
        {
            var updatetedUser = _context.users.SingleOrDefault(x => x.Id == id);
            if (updatetedUser == null)
            {
                return null;
            }
            updatetedUser.IsBlocked = user.IsBlocked;
            updatetedUser.LastName = user.LastName;
            updatetedUser.FirstName = user.FirstName;
            updatetedUser.Sallary = user.Sallary;
            updatetedUser.Role = user.Role;
            updatetedUser.Age = user.Age;
            updatetedUser.LoanID = user.LoanID;
            updatetedUser.AccountanId = user.AccountanId;
            updatetedUser.Email = user.Email;
            _context.SaveChanges();
            return updatetedUser;
        }



        public IEnumerable<Users> Filter(int age, double sallary)
        {
            var filteredUser = _context.users
                .Where(x => x.Age > age && x.Sallary > sallary);
            if (filteredUser == null)
            {
                return null;
            }
            return filteredUser;
        }
    }
}
