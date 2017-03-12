using dg.common.validation;
using Microsoft.Extensions.DependencyInjection;


namespace dg.api.test
{
    public class TestFixtureWithValidationActionFilter : TestFixtureWithValidationActionAttribute
    {
        // No need to configure Validators explicitly., This filter locates validator for contract 
        protected override IMvcBuilder ConfigureFluentValidation<T>(IServiceCollection services)
        {
            var mvcBuilder = base.ConfigureFluentValidation<T>(services);
            mvcBuilder.AddActionFilterValidator<T>();
            return mvcBuilder;
        }
    }
}
