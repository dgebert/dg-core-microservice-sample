using System;
using gwn.contract;

namespace gwn.validation
{
    public static class PersonExtensions
    {

        public static bool IsEquivalentTo(this Person person, Person otherPerson)
        {
            return string.Equals(person.FirstName, person.FirstName, StringComparison.CurrentCultureIgnoreCase) &&
                    string.Equals(person.LastName, person.LastName, StringComparison.CurrentCultureIgnoreCase) &&
                    person.HasSameEmail(otherPerson.Email) &&
                    person.BirthDate.Equals(person.BirthDate);
        }

        public static bool HasSameEmail(this Person person, string email)
        {
            return string.Equals(person.Email, email, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
