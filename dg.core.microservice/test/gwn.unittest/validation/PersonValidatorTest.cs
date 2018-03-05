using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;
using NSubstitute;
using gwn.contract;
using gwn.validation;
using gwn.dataservice;

namespace gwn.unittest.validation
{
    public class PersonValidatorTest
    {
        IPeopleService _peopleService;

        public PersonValidatorTest()
        {
            _peopleService = Substitute.For<IPeopleService>();
            _peopleService.GetAll().Returns((new List<Person>()));
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

        [Fact]
        public void GivenEmailIsInvalid_WhenValidate_ShouldFailWithError()
        {
            var person = new Person()
            {
                FirstName = "Bruce",
                LastName = "Willis",
                Email = "somebody@gmail"

            };
            var validator = BuildPersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.EmailInvalidFormat.ToString());
        }


        public static IEnumerable<object[]> SamplePeopleWithSameEmail => new List<object[]>
        {
            new object[]
                {new Person(1, "Bruce", "Willis", "bwillis@gmail.com", "214-345-9999", new DateTime(1970, 11, 14))},
            new object[]
                {new Person(1, "Bill", "Walton", "bwillis@gmail.com", "111-999-2333", new DateTime(1985, 1, 1))},
            new object[]
                {new Person(1, "Brad", "Watson", "bwillis@gmail.com", "111-222-3333", new DateTime(1985, 1, 1))}
        };
    

       // [Fact]
        [Theory]
        [MemberData(nameof(SamplePeopleWithSameEmail))]
        public void GivenEditedPerson_EqualToPersonBeingValidated_WithSameEmail_WhenValidate_ShouldSucceed(Person p)
        {
            var person = new Person()
            {
                Id = 1,
                FirstName = "Bruce",
                LastName = "Willis",
                Email = "bwillis@gmail.com",
                PhoneNumber = "214-345-9999",
                BirthDate = new DateTime(1970, 11, 14)
            };

            _peopleService = Substitute.For<IPeopleService>();
            _peopleService.GetAll().Returns(new List<Person> { person });
           var validator = BuildPersonValidator();

            //act
            var result = validator.Validate(person);

            //assert
            result.IsValid.Should().BeTrue();
        }

         [Fact]
        public void GivenDifferentPersonExistsWithSameEmail_WhenValidate_ShouldFailWithError()
        {
            var personInDb = new Person()
            {
                Id = 1,
                FirstName = "Bruce",
                LastName = "Willis",
                Email = "brianw@gmail.com",
                PhoneNumber = "214-345-9999",
                BirthDate = new DateTime(1970, 11, 14)
            };

            _peopleService = Substitute.For<IPeopleService>();
            _peopleService.GetAll().Returns(new List<Person> { personInDb });
            var validator = BuildPersonValidator();

            var newPerson = new Person()
            {
                Id = 4,
                FirstName = "Brian",
                LastName = "Walker",
                Email = "brianw@gmail.com",
                PhoneNumber = "214-345-3456",
                BirthDate = new DateTime(1972, 8, 21)
            };

            //act
            var result = validator.Validate(newPerson);

            //assert
            result.Errors.Should().NotBeEmpty();
            result.Errors.First().ErrorCode.Should().Be(PersonValidator.ErrorCode.EmailNotUnique.ToString());
        }

        private PersonValidator BuildPersonValidator()
        {
            return new PersonValidator(_peopleService);
        }
    }
}
