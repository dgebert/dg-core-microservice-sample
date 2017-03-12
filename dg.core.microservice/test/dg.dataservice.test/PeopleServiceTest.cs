
using Microsoft.EntityFrameworkCore;
using dg.repository.Models;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace dg.dataservice.test
{
    public class PeopleServiceTest
    {
        DbContextOptions<PeopleContext> _options;

        public PeopleServiceTest()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            _options = new DbContextOptionsBuilder<PeopleContext>()
                             .UseInMemoryDatabase(databaseName: "People")
                             .UseInternalServiceProvider(serviceProvider)
                             .Options;
        }

        [Fact]
        public void GivenPersonDoesNotExist_WhenGetById_ShouldReturnNull()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var person = service.Get(1);
                person.Should().BeNull();
            }
        }

        [Fact]
        public void GivenPersonExists_WhenGetById_ShouldReturnPerson()
        {
            Person p = null;
            int id = 1;
            using (var db = new PeopleContext(_options))
            {
                p = CreatePerson(id);
                db.Person.Add(p);
                db.SaveChanges();
            }

            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var person = service.Get(id);
                person.Should().NotBeNull();
                person.ShouldBeEquivalentTo(p);
            }
        }

        [Fact]
        public void GivenPersonExists_ButIsDeleted_WhenGetById_ShouldReturnNull()
        {
            Person p = null;
            int id = 1;
            using (var db = new PeopleContext(_options))
            {
                bool isDeleted = true;
                p = CreatePerson(id, isDeleted);
                db.Person.Add(p);
                db.SaveChanges();
            }

            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var person = service.Get(id);
                person.Should().BeNull();
            }
        }


        [Fact]
        public void GivenNoPeopleExist_WhenGetAll_ShouldReturnEmpty()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var allPeople = service.GetAll();
                allPeople.Should().BeEmpty();
            }
        }

        [Fact]
        public void GivenManyPeopleExist_WhenGetAll_ShouldReturnAll()
        {
            int total = 9;
            using (var db = new PeopleContext(_options))
            {
                for (var i =1; i <= total; i++)
                {
                    var p = CreatePerson(i);
                    db.Person.Add(p);
                }
                db.SaveChanges();
            }

            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var allPeople = service.GetAll();
                allPeople.Should().NotBeEmpty();
                allPeople.Count.Should().Be(total);
            }
        }

        public void GivenManyPeopleExist_SomeDeleted_WhenGetAll_ShouldReturnAll_ThatAreNotDeleted()
        {
            int total = 9;
            int deleteAfterId = 6;

            using (var db = new PeopleContext(_options))
            {
                for (var i = 1; i <= total; i++)
                {
                    var isDeleted = i > deleteAfterId;
                    var p = CreatePerson(i, isDeleted);
                    db.Person.Add(p);
                }
                db.SaveChanges();
            }

            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);

                var allPeople = service.GetAll();
                allPeople.Should().NotBeEmpty();
                allPeople.Count.Should().Be(total - deleteAfterId);
            }
        }


        [Fact]
        public void GivenPersonDoesNotExist_WhenCreatePerson_ShouldAddPersonToDb()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);
                var p = CreatePerson().ToPersonContract();
                service.Create(p);

                var personInDb = service.Get(p.Id);
                personInDb.ShouldBeEquivalentTo(p);
            }
        }


        [Fact]
        public void GivenPersonExists_WhenUpdate_ShouldUpdate()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);
                var p = CreatePerson().ToPersonContract();
                service.Create(p);

                p.LastName = p.LastName + " Updated";
                var updateResult = service.Update(p);
                updateResult.Should().NotBeNull();

                var personInDb = service.Get(p.Id);
                personInDb.ShouldBeEquivalentTo(p);
            }
        }

        [Fact]
        public void GivenPersonDoesNotExist_WhenUpdate_ShouldReturnNull_WithNoUpdate()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);
                var p = CreatePerson().ToPersonContract();
                var id = p.Id;
                service.Create(p);

                // Id should be readonly in reality, but this is to simulate a bad update
                var personInDb = service.Get(p.Id);
                personInDb.Id = 999;
                personInDb.LastName = p.LastName + " Updated"; 
                var updateResult = service.Update(personInDb);

                updateResult.Should().BeNull();

                var originalPerson = service.Get(id);
                originalPerson.ShouldBeEquivalentTo(p);
            }
        }

        [Fact]
        public void GivenPersonExists_WhenDelete_ShouldBeDeleted_AndNotFound()
        {
            using (var db = new PeopleContext(_options))
            {
                var service = new PeopleSqlService(db);
                var p = CreatePerson().ToPersonContract();
                service.Create(p);

                var result = service.Delete(p.Id);

                result.Should().BeTrue();
                var originalPerson = service.Get(p.Id);
                originalPerson.Should().BeNull();
                var deletedPerson = service.GetIgnoreDelete(p.Id);
                deletedPerson.ShouldBeEquivalentTo(p);
            }
        }

        private Person CreatePerson(int i = 1, bool isDeleted = false)
        {
            var p = new Person
            {
                Id = i,
                FirstName = "First_ " + i,
                LastName = "Last_ + " + i,
                Email = string.Format("somebody_{0}@gmail.com", i),
                BirthDate = new System.DateTime(1970 + i, i, i),
                PhoneNumber = string.Format("2{0}4-5{0}2{0}-4{0}5{0}", i),
                ModifiedOn = System.DateTime.UtcNow,
                IsDeleted = isDeleted
            };
            return p;
        }
    }
}
