
using System;
using System.Collections.Generic;

using dg.contract;
using dg.dataservice;

namespace dg.test.infrastructure
{
    public class MockPeopleService : IPeopleService
    {
        public Person Create(Person p)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Person Get(int id)
        {
            throw new NotImplementedException();
        }

        public List<Person> GetAll()
        {
            throw new NotImplementedException();
        }

        public Person Update(Person p)
        {
            throw new NotImplementedException();
        }
    }
}
