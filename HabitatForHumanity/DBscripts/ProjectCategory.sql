ALTER TABLE Project
ADD categoryId int;
GO

ALTER TABLE Organization
ADD comments varchar(max);
GO

CREATE TABLE [dbo].[ProjectCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [categoryType] VARCHAR(MAX) NOT NULL
)