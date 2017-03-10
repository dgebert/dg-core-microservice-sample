
delete from Person

insert into Person (Id, FirstName, LastName, Email, PhoneNumber, BirthDate, ModifiedOn ) 
  values(1, 'Joe', 'Plumber', 'joe.plumber@aol.com', '214-567-9780', '10/25/1965 2:00:00 PM', getdate())
  
insert into Person (Id, FirstName, LastName, Email, PhoneNumber, BirthDate, ModifiedOn ) 
  values(2, 'Jake', 'Reily', 'jake.reilly@yahoo.com', '817-345-6232', '02/11/1970 2:00:00 PM', getdate())

insert into Person (Id, FirstName, LastName, Email, PhoneNumber, BirthDate, ModifiedOn ) 
  values(3, 'Michele', 'Haley', 'michele.haley@gmail.com', '817-545-9787', '04/15/1972 2:00:00 PM', getdate())
