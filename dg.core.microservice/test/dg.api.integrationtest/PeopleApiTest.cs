using dg.contract;
using dg.unittest.api;
using dg.test.infrastructure;
using FluentAssertions;
using FluentValidation.Results;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using dg.dataservice;

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

            var p = new Person()
            {
                FirstName = "supercalifragilisticexpialidocious",
                LastName = "Willis"
            };

            var stringContent = _fixture.BuildRequestContent(p);
            var response = await _fixture.Client.PostAsync("people", stringContent);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            ValidationResult result = response.GetResult<ValidationResult>(); 
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task GivenPersonExists_WhenGet_ShouldReturnPerson()
        {
            using (var db = _fixture.GetDb())
            {
                // transaction will auto-rollback when disposed if either commands fails
                //using (var transaction = db.Database.BeginTransaction())
                //{
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
              //  }
            }
        }
    }
}
