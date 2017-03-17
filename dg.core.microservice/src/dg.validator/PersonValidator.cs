
using FluentValidation;
using FluentValidation.Results;

using dg.contract;
using dg.common.validation;

namespace dg.validator
{

    public class PersonValidator : AbstractValidator<Person>
    {
        public enum ErrorCode
        {
            FirstNameRequired = 11,
            FirstNameInvalidLength = 12,
            FirstNameHasInvalidChars = 13,

            LastNameRequired = 21,
            LastNameInvalidLength = 22,
            LastNameHasInvalidChars = 23,

            EmailInvalidFormat = 33,
            EmailNotUnique = 34,

            BirthDateInFuture = 41
        }

        public PersonValidator()
        {
            RuleForFirstName();

          //  RuleForLastName();
          //  RuleForEmail();
          //  RuleForBirthDate();
        }

        public override ValidationResult Validate(Person instance)
        {
            return base.Validate(instance);
        }

        private IRuleBuilderOptions<Person, string> RuleForFirstName()
        {
           return RuleFor(p => p.FirstName)
                  .NotEmpty()
                  .WithName("FirstName")
                  .WithMessage("First name is required.")
                  .WithErrorCode(ErrorCode.FirstNameRequired.ToString())
                  .Length(0, 20)
                  .WithMessage("First name cannot exceed 20 characters.")
                  .WithErrorCode(ErrorCode.FirstNameInvalidLength.ToString())
                  .Matches(@"^[A-Za-z0-9\-\.\s]+$")
                  .WithMessage("First name contains invalid characters.")
                  .WithErrorCode(ErrorCode.FirstNameHasInvalidChars.ToString());              
        }

        private IRuleBuilderOptions<Person, string> RuleForLastName()
        {
            return RuleFor(p => p.LastName)
                   .NotEmpty()
                   .WithName("LastName")
                   .WithMessage("Last name is required.")
                   .WithErrorCode(ErrorCode.LastNameRequired.ToString())
                   .Length(0, 20)
                   .WithMessage("Last name cannot exceed 20 characters.")
                   .WithErrorCode(ErrorCode.LastNameInvalidLength.ToString())
                   .Matches(@"^[A-Za-z\-\.\s]+$")
                   .WithMessage("Last name contains invalid characters.")
                   .WithErrorCode(ErrorCode.LastNameHasInvalidChars.ToString());
        }

    }
}
