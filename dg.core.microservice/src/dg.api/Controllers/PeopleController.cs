using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dg.common.validation;
using dg.contract;
using dg.dataservice;
using dg.validator;
using dg.common.logging;

namespace dg.api.controllers
{

    public class PeopleController : Controller
    {
        private IPeopleService _peopleService;
  //      private ILogger _logger;
  
        
              public PeopleController(IPeopleService service)
//        public PeopleController(IPeopleService service, ILogger logger)
        {
            _peopleService = service;
         //   _logger = logger;
        }


        [HttpGet("people")]
        public async Task<IActionResult> GetAllPeople()
        {
      //      _logger.LogInformation(LoggingEvents.LIST_ITEMS, "Getting all people");
            var result = await Task.Factory.StartNew(() =>
            {
                var people = _peopleService.GetAll();
                return people;
            });

            return Ok(result);
        }

        [HttpGet("people/{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                var person = _peopleService.Get(id);
                return person;
            });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // Validation solution 1 - validation via ActionFilterAttribute (ValidateInputAttribute)
        [HttpPost("people")]
        [ValidateInput]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
            return await AddPersonImpl(person);
        }

        // Validation solution 2 - validation via registered IActionFilter (ValidateInputFilter) in Startup.ConfigureServices()
        [HttpPost("people2")]
        public async Task<IActionResult> AddPerson2([FromBody] Person person)
        {
            return await AddPersonImpl(person);
        }


        // Validation solution 3 - explict call to validator
        [HttpPost("people3")]
        public async Task<IActionResult> AddPerson1([FromBody] Person person)
        {

            var validationResult = new PersonValidator().Validate(person);
            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult(validationResult);
            }

            return await AddPersonImpl(person);
        }

        private async Task<IActionResult> AddPersonImpl(Person p)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                var personInDb = _peopleService.Create(p);
                return personInDb;
            });

            return Ok(result);
        }

        [HttpPut("people")]
        [ValidateInput]
        public async Task<IActionResult> Update([FromBody]Person person)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                var personInDb = _peopleService.Update(person);
                return personInDb;
            });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await Task.Factory.StartNew(() =>
            {
                return _peopleService.Delete(id);
            });

            if (!result)
            {
                return NotFound();
            }

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
