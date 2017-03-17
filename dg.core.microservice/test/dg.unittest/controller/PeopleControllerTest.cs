using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using NSubstitute;
using FluentAssertions;
using dg.api.controllers;
using dg.dataservice;
using dg.contract;
using dg.test.infrastructure;
using Microsoft.AspNetCore.Http;

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
        public async Task GetAll_NotEmpty_Ok()
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
        public async Task GetAll_Empty_Ok()
        {
            var people = new List<Person>();
            _mockPeopleService.GetAll().Returns(people);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.GetAllPeople();

            var okResult = result.As<OkObjectResult>();
            var peopleResult = okResult.Value.As<List<Person>>();
            peopleResult.Should().BeEquivalentTo(people);
        }

        [Fact]
        public async Task AddPerson_Ok()
        {
            var p = new PeopleBuilder().Build();
            _mockPeopleService.Find(p).Returns((Person)null);
            _mockPeopleService.Create(p).Returns(p);

            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.AddPerson(p);

            var okResult = result.As<OkObjectResult>();
            var personResult = okResult.Value.As<Person>();
            personResult.ShouldBeEquivalentTo(p);
        }

        [Fact]
        public async Task AddDuplicatePerson_Conflict409()
        {
            var p = new PeopleBuilder().Build();
            _mockPeopleService.Find(p).Returns(p);

            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.AddPerson(p);

            var conflictResult = result.As<StatusCodeResult>();
            conflictResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        }

        [Fact]
        public async Task UpdatePerson_NotFound()
        {
            var p = new PeopleBuilder().Build();
            _mockPeopleService.Update(p).Returns((Person)null);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.Update(p);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdatePerson_Ok()
        {
            var p = new PeopleBuilder().Build();
            _mockPeopleService.Update(p).Returns(p);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.Update(p);

            var okResult = result.As<OkObjectResult>();
            var personResult = okResult.Value.As<Person>();
            personResult.ShouldBeEquivalentTo(p);
        }


        [Fact]
        public async Task DeletePerson_NotFound()
        {
            _mockPeopleService.Delete(1).Returns(false);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.Delete(1);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeletePerson_Ok()
        {
            _mockPeopleService.Delete(1).Returns(true);
            var controller = new PeopleController(_mockPeopleService);

            var result = await controller.Delete(1);

            result.Should().BeOfType<OkResult>();
        }
    }
}
