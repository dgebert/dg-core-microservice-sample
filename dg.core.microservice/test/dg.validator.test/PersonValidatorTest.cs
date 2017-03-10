
using Xunit;
using FluentAssertions;
using dg.contract;

namespace dg.validator.test
{
    public class PersonValidatorTest
    {
        public PersonValidatorTest()
        {
        }

        [Fact]
        public void GivenFirstNameEmpty_WhenValidate_ShouldReturnErrors()
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
