using FluentValidation;
using FluentValidation.Results;

namespace dg.common.validation
{
    public static class FluentValidationExtensions
    {
        public static ValidationError ToValidationError(this ValidationFailure failure)
        {
            return new ValidationError
            {
                ErrorCode = failure.ErrorCode,
                ErrorMessage = failure.ErrorMessage,
            };
        }
    }
}
