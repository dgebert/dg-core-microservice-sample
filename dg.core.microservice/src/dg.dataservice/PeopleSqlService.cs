using System;
using System.Linq;
using System.Collections.Generic;

using dg.repository.Models;

using PersonContract = dg.contract.Person;
using PersonEntity = dg.repository.Models.Person;
using dg.contract;

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

        // not on interface, used for testing (soft) Delete
        public PersonContract GetIgnoreDelete(int id)
        {
            var personEntity = Find(id, respectDelete : false);
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
            if (Find(person.ToPersonEntity()) != null)
            {
                return null;
            }

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


        public PersonContract Find(PersonContract p)
        {
            var pe = Find(p.ToPersonEntity());
            if (pe == null)
            {
                return null;
            }
            return pe.ToPersonContract();
        }

        private PersonEntity Find(int id, bool respectDelete = true)
        {
            return respectDelete ?
                 _db.Person.FirstOrDefault(p => p.Id == id && !p.IsDeleted) :
                  _db.Person.FirstOrDefault(p => p.Id == id);
        }

        private PersonEntity Find(PersonEntity pe)
        {
            var similarPerson = _db.Person.FirstOrDefault(p =>
                                    string.Equals(p.FirstName, pe.FirstName, StringComparison.CurrentCultureIgnoreCase) &&
                                    string.Equals(p.LastName, pe.LastName, StringComparison.CurrentCultureIgnoreCase) &&
                                    string.Equals(p.Email, pe.Email, StringComparison.CurrentCultureIgnoreCase) &&
                                    p.BirthDate.Equals(pe.BirthDate));
            return similarPerson;                          
        }

        public PersonContract FindByEmail(string email)
        {
            var pe = _db.Person.FirstOrDefault(p => string.Equals(p.Email, email, StringComparison.CurrentCultureIgnoreCase));
            if (pe == null)
            {
                return null;
            }
            return pe.ToPersonContract();
        }
    }
}
