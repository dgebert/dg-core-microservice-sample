using System.Linq;
using Xunit;
using FluentAssertions;
using NSubstitute;
using dg.contract;
using dg.validator;
using dg.dataservice;


namespace dg.unittest.validator
{
    public class PersonValidatorTest
    {
        IPeopleService _peopleService;

        public PersonValidatorTest()
        {
            _peopleService = Substitute.For<IPeopleService>();
            _peopleService.FindByEmail(Arg.Any<string>()).Returns((Person)null);
        }

        [Fact]
        public void GivenFirstNameEmpty_WhenValidate_ShouldReturnErrors()
        {
            var person = new Person()
            {
                FirstName = string.Empty,
                LastName = "Willis",
                Email = "somebody@gmail.com"
            };
            var validator = BuildPersonValidator();

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
                LastName = "Willis",
                Email = "somebody@gmail.com"
            };
            var validator = BuildPersonValidator();
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
                LastName = "Willis",
                Email = "somebody@gmail.com"
            };
            var validator = BuildPersonValidator();

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
                LastName = "Willis",
                Email = "somebody@gmail.com"

            };
            var validator = BuildPersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.FirstNameHasInvalidChars.ToString());
        }

        private PersonValidator BuildPersonValidator()
        {
            return new PersonValidator(_peopleService);
        }
    }
}
