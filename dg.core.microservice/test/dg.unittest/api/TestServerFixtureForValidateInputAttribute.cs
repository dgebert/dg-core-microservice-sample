
using Microsoft.Extensions.DependencyInjection;
using dg.validator;
using dg.test.infrastructure;

namespace dg.unittest.api
{
    public class TestServerFixtureForValidateInputAttribute : TestServerFixture
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            ConfigureMvcForValidateInputAttribute<PersonValidator>(services);
        }
      
    }
}
