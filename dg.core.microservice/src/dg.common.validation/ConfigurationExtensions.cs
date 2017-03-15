using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;


namespace dg.common.validation
{
    public static class ConfigurationExtensions
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
            mvcBuilder.AddValidatorsFromAssemblyContaining<T>();
            mvcBuilder.AddMvcOptions(options => options.Filters.Add(new ValidateInputFilter()));
            return mvcBuilder;
        }

        public static IMvcBuilder AddValidateInputAttribute<T>(this IServiceCollection services) where T : class
        {
            return
                  services
                       .AddMvc()
                       .AddValidatorsFromAssemblyContaining<T>();
        }

        public static IMvcBuilder AddValidatorsFromAssemblyContaining<T>(this IMvcBuilder mvcBuilder) where T : class
        {
            mvcBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<T>());
            mvcBuilder.Services.AddScoped<IActionContextModelValidator, ActionContextModelValidator>();
            return mvcBuilder;
        }


    }
}
