using PersonContract = dg.contract.Person;
using PersonEntity = dg.repository.Models.Person;

namespace dg.dataservice
{
    public static class PersonExtensions
    {
        public static PersonContract ToPersonContract (this PersonEntity p)
        {
            return new PersonContract
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
            };
        }

        public static PersonEntity ToPersonEntity(this PersonContract p)
        {
            return new PersonEntity
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                ModifiedOn = System.DateTime.UtcNow,
                IsDeleted = false
            };
        }

        public static PersonEntity UpdateFrom(this PersonEntity target, PersonContract source)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.BirthDate = source.BirthDate;
            target.Email = source.Email;
            target.ModifiedOn = System.DateTime.UtcNow;
       //     target.IsDeleted = source.IsDeleted;
            return target;
        }
    }
}
