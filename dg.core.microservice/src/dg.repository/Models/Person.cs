using System;
using System.Collections.Generic;

namespace dg.repository.Models
{
    public partial class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedOn { get; set; }
      
    }
}
