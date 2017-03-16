﻿
using System.Collections.Generic;
using dg.contract;

namespace dg.test.infrastructure
{
    public class PeopleBuilder
    {
        public List<Person> BuildMany(int n, string suffix = null)
        {
            var people = new List<Person>();
            for (var i = 1; i <= n; i++)
            {
                var p = Build(i);
                people.Add(p);
            }
            return people;
        }

        public Person Build(int i = 1, bool isDeleted = false, string suffix = null)
        {
            suffix = suffix ?? string.Empty;
            var p = new Person
            {
                Id = i,
                FirstName = string.Format("First_{0}_{1}", i, suffix),
                LastName = string.Format("Last_{0}_{1}", i, suffix),
                Email = string.Format("somebody_{0}_{1}@gmail.com", i, suffix),
                BirthDate = new System.DateTime(1970 + i, i, i),
                PhoneNumber = string.Format("2{0}4-5{0}2{0}-4{0}5{0}", i),
                ModifiedOn = System.DateTime.UtcNow,
                ModifiedBy = string.Format("TestBuilder_{0}", suffix)
            //    IsDeleted = isDeleted
            };
            return p;
        }
        //public Person Build
    }
}
