using Microsoft.Extensions.DependencyInjection;

using dg.test.infrastructure;
using dg.validator;

namespace dg.unittest.api
{
    public class TestServerFixtureforValidateInputFilter : TestServerFixture
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            ConfigureMvcForValidateInputFilter<PersonValidator>(services);
        }
   
    }
}
