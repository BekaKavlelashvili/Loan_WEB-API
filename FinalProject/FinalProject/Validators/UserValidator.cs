using FinalProject.Models;
using FluentValidation;

namespace FinalProject.Validators
{
    public class UserValidator : AbstractValidator<Users>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty()
                .WithMessage("The firstName field is empty").Length(0, 50)
                    .WithMessage("FirstName Length is more than 50");
            RuleFor(x => x.LastName).NotEmpty()
                .WithMessage("The lastName field is empty")
                    .Length(0, 50).WithMessage("LastName Length is more than 50");
            RuleFor(x => x.UserName).NotEmpty()
                .WithMessage("The UserName field is empty")
                    .Length(0, 50).WithMessage("UserName Length is more than 50");
            RuleFor(x => x.Password).NotEmpty()
               .WithMessage("The Password field is empty")
               .MinimumLength(7).WithMessage("Password Length is less than 7")
                .MaximumLength(25).WithMessage("Password Length is more than 25");
            RuleFor(x => x.Role).NotEmpty()
                .WithMessage("The Role field is empty");
            RuleFor(x => x.Age).NotEmpty()
                .WithMessage("The Age field is empty").InclusiveBetween(18, 70);
            RuleFor(x => x.Email).NotEmpty()
                .WithMessage("The Email field is empty").EmailAddress();
            RuleFor(x => x.Sallary).NotEmpty().WithMessage("The Sallary field is empty")
                .GreaterThanOrEqualTo(750);
            RuleFor(x => x.AccountanId).NotEmpty()
                .WithMessage("The AccountanId field is empty");
            RuleFor(x => x.LoanID).NotEmpty()
                .WithMessage("The LoanID field is empty");

        }
    }
}
