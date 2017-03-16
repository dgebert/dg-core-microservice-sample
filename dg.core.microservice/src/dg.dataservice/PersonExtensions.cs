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
                PhoneNumber = p.PhoneNumber,
                BirthDate = p.BirthDate,
                ModifiedOn = p.ModifiedOn,
                ModifiedBy = p.ModifiedBy
            };
        }

        public static PersonEntity ToPersonEntity(this PersonContract p)
        {
            PersonEntity pe = new PersonEntity
            {
                Id = p.Id,
            };

            return pe.UpdateFrom(p);
        }

        public static PersonEntity UpdateFrom(this PersonEntity target, PersonContract source)
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.BirthDate = source.BirthDate;
            target.Email = source.Email;
            target.PhoneNumber = source.PhoneNumber;
            target.ModifiedOn = source.ModifiedOn == System.DateTime.MinValue ? System.DateTime.UtcNow : source.ModifiedOn;
            target.ModifiedBy = source.ModifiedBy;
            return target;
        }
    }
}
