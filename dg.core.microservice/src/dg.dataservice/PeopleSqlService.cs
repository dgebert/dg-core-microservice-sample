using System;
using System.Linq;
using System.Collections.Generic;

using dg.repository.Models;

using PersonContract = dg.contract.Person;
using PersonEntity = dg.repository.Models.Person;


namespace dg.dataservice
{
    public class PeopleSqlService : IPeopleService
    {
        private PeopleContext _db;

        public PeopleSqlService(PeopleContext dbContext)
        {
            _db = dbContext;
        }

        public List<PersonContract> GetAll()
        {
            var people = _db.Person.Where(p => !p.IsDeleted)
                                     .OrderBy(p => p.LastName)
                                    .Select(p => p.ToPersonContract())
                                    .ToList();
            return people;
        }

        public PersonContract Get(int id)
        {
            var personEntity = Find(id);
            return personEntity == null ? null : personEntity.ToPersonContract();
        }

        public PersonContract Update(PersonContract person)
        {
            var personInDb = Find(person.Id);
            if (personInDb == null)
            {
                return null;  // fail - 404 notfound
            }

            personInDb.UpdateFrom(person);
            _db.SaveChanges();

            return personInDb.ToPersonContract();
        }

        public PersonContract Create(PersonContract person)
        {
            var personEntity = person.ToPersonEntity();
            _db.Person.Add(personEntity);
            _db.SaveChanges();
            return personEntity.ToPersonContract();
        }

        public bool Delete(int id)
        {
            var personInDb = Find(id);
            if (personInDb == null)
            {
                return false;  // fail - 404 notfound
            }

            personInDb.IsDeleted = true;
            _db.SaveChanges();
            return true;
        }

        private PersonEntity Find(int id)
        {
            return _db.Person.FirstOrDefault(p => p.Id == id);
        }

    }
}
