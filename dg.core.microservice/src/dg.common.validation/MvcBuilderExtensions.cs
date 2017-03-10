using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;


namespace dg.common.validation
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddActionFilterValidator<T>(this IMvcBuilder mvcBuilder) where T : class
        {
            mvcBuilder.AddValidatorsFromAssemblyContaining<T>();
            mvcBuilder.AddMvcOptions(options => options.Filters.Add(new ValidateInputFilter()));
            return mvcBuilder;
        }

        public static IMvcBuilder AddValidatorsFromAssemblyContaining<T>(this IMvcBuilder mvcBuilder) where T : class
        {
            mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<T>());
            return mvcBuilder;
        }
    }
}
