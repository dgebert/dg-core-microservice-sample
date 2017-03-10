using dg.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dg.dataservice
{
    public interface IPeopleService
    {
        List<Person> GetAll();
        Person Get(int id);
        Person Create(Person p);
        Person Update(Person p);
        bool Delete(int id);
    }
}
