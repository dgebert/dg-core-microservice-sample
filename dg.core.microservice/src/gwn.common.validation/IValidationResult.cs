using System;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gwn.common.validation
{
    public interface IValidationResult : IActionFilter
    {
        ValidationResult Result { get; }

    }
}
