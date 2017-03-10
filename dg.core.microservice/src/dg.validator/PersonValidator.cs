
using FluentValidation;
using FluentValidation.Results;

using dg.contract;

namespace dg.validator
{
    public enum ErrorCode
    {
        FirstNameRequired = 11,
        FirstNameTooLong = 12,
        FirstNameHasInvalidChars = 13,
        LastNameREquired = 21,
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(p => p.FirstName)
                  .NotEmpty()
                  .WithMessage("First name is required.")
                  .WithErrorCode(ErrorCode.FirstNameRequired.ToString())
                  .Length(0, 5)
                  .WithName("FirstName")
                  .WithMessage("First name cannot exceed 5 characters.")
                  .WithErrorCode(ErrorCode.FirstNameTooLong.ToString())
                  .Matches(@"^[A-Za-z\-\.\s]+$")
                  .WithMessage("First name contains invalid characters.")
                  .WithErrorCode(ErrorCode.FirstNameTooLong.ToString());


            //RuleFor(p => p.LastName).NotEmpty().WithMessage("First Name is required");
            //RuleFor(p => p.MembershipFee).GreaterThan(0).WithMessage("Membership is required");
            //RuleFor(p => p.BirthDate).
        }

        public override ValidationResult Validate(Person instance)
        {
            return base.Validate(instance);
        }
    }
}
