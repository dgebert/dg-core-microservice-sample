using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using NSubstitute;
using FluentAssertions;
using dg.api.controllers;
using dg.dataservice;
using dg.contract;


namespace dg.unittest.controller
{
    public class PeopleControllerTest
    {
        IPeopleService _mockPeopleService;

        public PeopleControllerTest()
        {
            _mockPeopleService = Substitute.For<IPeopleService>();
        }

        [Fact]
		public async Task GetPerson_Ok()
        {
            var p = new Person();
            _mockPeopleService.Get(1).Returns(p);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.GetPerson(1);

            var okResult = result.As<OkObjectResult>();
            var person = okResult.Value.As<Person>();
            person.ShouldBeEquivalentTo(p);
        }

        [Fact]
        public async Task GetPerson_NotFound()
        {
            Person p = null;
            _mockPeopleService.Get(1).Returns(p);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.GetPerson(1);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAll_Should_Return_Some()
        {
            var people = new List<Person> {
                new Person { FirstName = "Frank" },
                new Person {FirstName = "Bill" }
            };
            _mockPeopleService.GetAll().Returns(people);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.GetAllPeople();

            var okResult = result.As<OkObjectResult>();
            var peopleResult = okResult.Value.As<List<Person>>();
            peopleResult.Should().BeEquivalentTo(people);
        }

        [Fact]
        public async Task GetAll_Should_Return_None()
        {
            var people = new List<Person>();
            _mockPeopleService.GetAll().Returns(people);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.GetAllPeople();

            var okResult = result.As<OkObjectResult>();
            var peopleResult = okResult.Value.As<List<Person>>();
            peopleResult.Should().BeEquivalentTo(people);
        }
    }
}
