CREATE TABLE [dbo].[Person]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] VARCHAR(100) NOT NULL, 
    [LastName] VARCHAR(100) NOT NULL, 
    [Email] VARCHAR(100) NOT NULL, 
	[PhoneNumber] VARCHAR(25) NULL,
    [BirthDate] DATETIME2 NOT NULL, 
    [IsDeleted] BIT NOT NULL default 0, 
    [ModifiedOn] DATETIME2 NOT NULL, 
  
)
