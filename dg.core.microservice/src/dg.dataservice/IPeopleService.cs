
using System.Collections.Generic;
using dg.contract;

namespace dg.dataservice
{
    public interface IPeopleService
    {
        List<Person> GetAll();
        Person Get(int id);
        Person Create(Person p);
        Person Update(Person p);
        bool Delete(int id);
        Person Find(Person p);

        Person FindByEmail(string email);
    }
}
