
using System;

namespace gwn.contract
{
    public class Person
    {
        public Person() { }

        public Person(int id, string first, string last, string email, string phone, DateTime birthDate)
        {
            Id = id;
            FirstName = first;
            LastName = last;
            Email = email;
            PhoneNumber = phone;
            BirthDate = BirthDate;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set;  }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }


        public override string ToString()
        {
            return string.Format("Person [{0}, {1} {2}, {3}, {4}]",
                                 Id, FirstName, LastName, Email, PhoneNumber, BirthDate.ToString("MM/dd/yyyy"));
        }
    }
}
