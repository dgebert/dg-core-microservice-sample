using FluentAssertions;
using Xunit;
using dg.contract;
using dg.validator;


namespace dg.api.test
{
    public class PersonValidatorTest
    {

        public PersonValidatorTest()
        {
           
        }

        [Fact]
        public void FirstName_EmptyString_ValidationFails()
        {
            var person = new Person();
            person.FirstName = string.Empty;
            var validator = new PersonValidator();
            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
        }
    }
}
