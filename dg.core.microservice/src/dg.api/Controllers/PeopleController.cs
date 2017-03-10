using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;


namespace Absg.Common.Validation.DemoApi.Controllers
{

    public class PeopleController : Controller
    {
        private IPeopleService _peopleService;
        private ILogger _logger;

        public PeopleController(IPeopleService service, ILogger logger)
         {
            _peopleService = service;
            _logger = logger;
        }


        [HttpGet("people")]
        public async Task<IActionResult> GetAllPeople()
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return new List<Person> {
                    new Person
                    {
                        Id = 100,
                        FirstName = "Mike",
                        LastName = "Pense",
                        Email = "dumass@aol.com",
                        BirthDate = new System.DateTime(1959, 11, 10),
                        ModifiedOn = System.DateTime.UtcNow,
                    },
                     new Person
                    {
                        Id = 100,
                        FirstName = "Bill",
                        LastName = "Gifford",
                        Email = "joe.plumber@yahoo.com",
                        BirthDate = new System.DateTime(1965, 05, 10),
                        ModifiedOn = System.DateTime.UtcNow,
                    },
                      new Person
                    {
                        Id = 100,
                        FirstName = "Jane",
                        LastName = "Reilly",
                        Email = "jane.reilly@yahoo.com",
                        BirthDate = new System.DateTime(1970, 02, 28),
                        ModifiedOn = System.DateTime.UtcNow,
                    },
                };
            });

            return Ok(result);
        }

        [HttpGet("people/{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return new Person
                {
                    Id = 100,
                    FirstName = "Joe",
                    LastName = "Plumber",
                    Email = "joe.plumber@aol.com",
                    BirthDate = new System.DateTime(1965, 05, 10),
                    ModifiedOn = System.DateTime.UtcNow,

                };
            });

            return Ok(result);
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
        public async Task<IActionResult> AddPerson2([FromBody] Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

        // Validation solution 3 - validation via registered IActionFilter (ValidateInputFilter) in Startup.ConfigureServices()
        [HttpPost("people4")]
        public async Task<IActionResult> AddPerson3([FromBody] Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }



        [HttpPut("people")]
        public async Task<IActionResult> Update([FromBody]Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return person;
            });

            return Ok(result);
        }

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
