using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.PersonContext;
using System.Collections.Generic;
using System.Linq;

namespace FinalProject.Services
{
    public interface IUserService
    {
        Users Login(string userName, string password, int loanId, string role);
        Loan AddLoan(Loan loan);
        IEnumerable<Loan> GetAllLoan(int id);
        IEnumerable<Loan> DeleteLoan(int id);
        Loan UpdateLoan(int id, Loan loan);
    }
    public class UserService : IUserService
    {
        readonly ApplicationContext _context;
        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public Users Login(string userName, string password, int loanId, string role)
        {
            var hashing = new PasswordHasher();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            var user = _context.users.SingleOrDefault(x => x.UserName == userName);
            if (user == null || !hashing.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }
        public Loan AddLoan(Loan loan)
        {
            var newLoan = new Loan()
            {
                Id = loan.Id,
                LoanID = loan.LoanID,
                LoanType = loan.LoanType,
                Currency = loan.Currency,
                Ammount = loan.Ammount,
                Status = loan.Status = LoanStatus.Processing
            };
            try
            {
                _context.loans.Add(newLoan);
                _context.SaveChanges();
                return newLoan;
            }
            catch (System.Exception ex)
            {

                throw new System.Exception(message: "Don't leave fields empty");
            }

        }

        public IEnumerable<Loan> DeleteLoan(int id)
        {
            var idForDelete = _context.loans.SingleOrDefault(x => x.Id == id);
            if (idForDelete.Status != LoanStatus.Processing)
            {
                return null;
            }
            _context.loans.Remove(idForDelete);
            _context.SaveChanges();
            return _context.loans.ToList().Where(x => x.LoanID == id);
        }

        public IEnumerable<Loan> GetAllLoan(int loanid)
        {
            var loans = _context.loans.Where(x => x.LoanID == loanid).ToList();

            return loans;
        }

        public Loan UpdateLoan(int id, Loan loan)
        {

            try
            {
                var loanForUpdate = _context.loans.SingleOrDefault(x => x.Id == id);
                if (loanForUpdate.Status == LoanStatus.Processing && loanForUpdate != null)
                {
                    loanForUpdate.Currency = loan.Currency;
                    loanForUpdate.LoanPeriod = loan.LoanPeriod;
                    loanForUpdate.LoanType = loan.LoanType;
                    loanForUpdate.Ammount = loan.Ammount;
                    loanForUpdate.Status = loan.Status = LoanStatus.Processing;
                }
                _context.SaveChanges();
                return loanForUpdate;
            }
            catch (System.NullReferenceException)
            {
                throw new System.NullReferenceException(message: "Loan is not processing");
            }
        }
    }
}
