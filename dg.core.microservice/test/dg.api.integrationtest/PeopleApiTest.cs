using dg.contract;
using dg.dataservice;
using dg.test.infrastructure;
using dg.unittest.api;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace dg.api.integrationtest
{
    public class PeopleApiTest : IClassFixture<TestServerFixture>, IDisposable
    {
        private TestServerFixture _fixture;

        public PeopleApiTest(TestServerFixture fixture)
        {
            _fixture = fixture;

            _fixture.SetUpDb();
        }

        public void Dispose()
        {
            _fixture.TearDownDb();
        }

        [Fact]
        public async Task GivenInvalidPerson_WhenCreate_ShouldReturnBadRequest()
        {
            // This hits validator and fails
            var p = new Person()
            {
                FirstName = "supercalifragilisticexpialidocious",
                LastName = "Willis"
            };

            var response = await _fixture.Client.PostAsync("people", p);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            ValidationResult result = response.GetResult<ValidationResult>();
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task GivenNoPersonExists_WhenGet_ShouldReturnPerson()
        {
            var response = await _fixture.Client.GetAsync("people/1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenPersonExists_WhenGet_ShouldReturnNotFound()
        {
            using (var db = _fixture.GetDb())
            {
                try
                {
                    var p = new PeopleBuilder().Build();
                    var personEntity = p.ToPersonEntity();
                    db.Person.Add(personEntity);
                    db.SaveChanges();

                    var uri = string.Format("people/{0}", personEntity.Id);
                    var response = await _fixture.Client.GetAsync(uri);

                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                    var personInDb = response.GetResult<Person>();
                    personInDb.ShouldBeEquivalentTo(p);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                    // TODO: Handle failure
                }
            }
        }

        [Fact]
        public async Task GivenNoPeopleExist_WhenGetMany_ShouldReturnEmpty()
        {
            var response = await _fixture.Client.GetAsync("people");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var peopleInDb = response.GetResult<List<Person>>();
            peopleInDb.Should().BeEmpty();
        }

        [Fact]
        public async Task GivenPeopleExist_WithSomeDeleted_WhenGetMany_ShouldReturn_PeopleNotDeleted()
        {
            using (var db = _fixture.GetDb())
            {
                try
                {
                    int idThreshold = 4;
                    var people = new PeopleBuilder().BuildMany(10);
                    foreach (var p in people)
                    {
                        // delete people with id > totalDeleted
                        var personEntity = p.ToPersonEntity();
                        personEntity.IsDeleted = p.Id < idThreshold;
                        db.Person.Add(personEntity);
                    }
                    db.SaveChanges();

                    var response = await _fixture.Client.GetAsync("people");

                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                    var peopleInDb = response.GetResult<List<Person>>();

                    var notDeletedPeople = people.Where(p => p.Id >= idThreshold);
                    peopleInDb.ShouldBeEquivalentTo(notDeletedPeople);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                    // TODO: Handle failure
                }
            }
        }

        [Fact]
        public async Task GivenPeopleExist_WhenGetMany_ShouldReturnPerson()
        {
            using (var db = _fixture.GetDb())
            {
                try
                {
                    var people = new PeopleBuilder().BuildMany(10);
                    foreach (var p in people)
                    {
                        var personEntity = p.ToPersonEntity();
                        db.Person.Add(personEntity);
                    }
                    db.SaveChanges();

                    var response = await _fixture.Client.GetAsync("people");

                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                    var peopleInDb = response.GetResult<List<Person>>();
                    peopleInDb.ShouldBeEquivalentTo(people);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                    // TODO: Handle failure
                }
            }
        }

        [Fact]
        public async Task GivenPersonDoesNotExist_WhenAdd_ShouldExistInDb()
        {
            using (var db = _fixture.GetDb())
            {
                try
                {
                    var person = new PeopleBuilder().Build();

                    var response = await _fixture.Client.PostAsync("people", person);

                    response.StatusCode.Should().Be(HttpStatusCode.OK);
                    var personInDb = response.GetResult<Person>();
                    personInDb.ShouldBeEquivalentTo(person);

                    var uri = string.Format("people/{0}", person.Id);
                    response = await _fixture.Client.GetAsync(uri);
                    personInDb = response.GetResult<Person>();
                    personInDb.ShouldBeEquivalentTo(person);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                    // TODO: Handle failure
                }
            }
        }

        [Fact]
        public async Task GivenPersonExist_WhenAdd_ShouldReturn409()
        {
            using (var db = _fixture.GetDb())
            {
                try
                {
                    var person = new PeopleBuilder().Build(1);
                    db.Person.Add(person.ToPersonEntity());
                    db.SaveChanges();

                    var newPerson = new PeopleBuilder().Build(1);
                    var response = await _fixture.Client.PostAsync("people", person);

                    response.StatusCode.Should().Be(HttpStatusCode.Conflict);  // Conflict = 409);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                    // TODO: Handle failure
                }
            }
        }
    }
}