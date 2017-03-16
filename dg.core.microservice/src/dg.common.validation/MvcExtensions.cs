using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;


namespace dg.common.validation
{
    public static class MvcExtensions
    {

        public static IMvcBuilder AddValidateInputFilter<T>(this IServiceCollection services) where T : class
        {
            return
                services
                       .AddMvc()
                       .AddActionFilterValidator<T>();
        }

        public static IMvcBuilder AddActionFilterValidator<T>(this IMvcBuilder mvcBuilder) where T : class
        {
            mvcBuilder.AddValidateInputAttribute<T>();
            mvcBuilder.AddMvcOptions(options => options.Filters.Add(new ValidateInputFilter()));
            return mvcBuilder;
        }

        public static IMvcBuilder AddValidateInputAttribute<T>(this IServiceCollection services) where T : class
        {
            return
                  services
                       .AddMvc()
                       .AddValidateInputAttribute<T>();
        }

        public static IMvcBuilder AddValidateInputAttribute<T>(this IMvcBuilder mvcBuilder) where T : class
        {
            mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<T>());
            mvcBuilder.Services.AddScoped<IActionContextModelValidator, ActionContextModelValidator>();
            return mvcBuilder;
        }


    }
}
