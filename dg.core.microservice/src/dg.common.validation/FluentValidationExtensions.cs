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
                AttemptedValue = failure.AttemptedValue,
                PropertyName = failure.PropertyName
            };
        }


        public static IRuleBuilderOptions<T, TProperty> 
            WithErrorInfo<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, 
                                       string propertyName, string errorMessage, int errorCode)
        {
            return 
                rule
                    .WithName(propertyName)
                    .WithMessage(errorMessage)
                    .WithErrorCode(errorCode.ToString());
        }

    }

  

}
