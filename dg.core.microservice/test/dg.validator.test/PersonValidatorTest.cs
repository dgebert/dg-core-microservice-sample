using System.Linq;
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
            var person = new Person()
            {
                FirstName = string.Empty,
                LastName = "Willis"
            };
            var validator = new PersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
        }


        [Fact]
        public void GivenFirstNameEmpty_WhenValidate_ShouldFailWithErrors()
        {
            var person = new Person()
            {
                FirstName = string.Empty,
                LastName = "Willis"
            };
            var validator = new PersonValidator();
            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.FirstNameRequired.ToString());
        }

        [Fact]
        public void GivenFirstTooLong_WhenValidate_ShouldFailWithErrors()
        {
            var person = new Person()
            {
                FirstName = "supercalifragilisticexpialidocious",
                LastName = "Willis"
            };
            var validator = new PersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.FirstNameInvalidLength.ToString());
        }


        [Fact]
        public void GivenFirstHasInvalidChars_WhenValidate_ShouldFailWithErrors()
        {
            var person = new Person()
            {
                FirstName = "Not@Valid&",
                LastName = "Willis"
            };
            var validator = new PersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.FirstNameHasInvalidChars.ToString());
        }
    }
}
