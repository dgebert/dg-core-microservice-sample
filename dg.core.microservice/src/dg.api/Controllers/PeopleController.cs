

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;


namespace Absg.Common.Validation.DemoApi.Controllers
{

    public class PeopleController : Controller
    {
        private IPeopleService _peopleService;

        //      public PeopleController(IPersonService service)
        public PeopleController()
        {
            //        _personService = service;
        }

        // Validation solution 1 - explict call to validator
        [HttpPost("people1")]
        public async Task<IActionResult> AddPerson1([FromBody] Person person)
        {
            var validationResult = new PersonValidator().Validate(person);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult.Errors);
            }

            return Ok(person);
        }

        // Validation solution 2 - validation via ActionFilterAttribute (ValidateInputAttribute)
        [HttpPost("people3")]
        [ValidateInput]
        public async Task<IActionResult> AddPerson3([FromBody] Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

        // Validation solution 3 - validation via registered IActionFilter (ValidateInputFilter) in Startup.ConfigureServices()
        [HttpPost("people4")]
        public async Task<IActionResult> AddPerson4([FromBody] Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

        // PUT api/values/5
        [HttpPost("people")]
        public async Task<IActionResult> Create([FromBody]Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

        // PUT api/values/5
        [HttpPut("people")]
        public async Task<IActionResult> Update([FromBody]Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return 1;
            });

            return Ok();
        }


        [HttpGet("people/ping")]
        public async Task<IActionResult> Ping()
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return "ping at " + System.DateTime.UtcNow;
            });

            return Ok(result);
        }
    }
}
