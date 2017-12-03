ALTER TABLE Project
ADD categoryId int;
GO


CREATE TABLE [dbo].[ProjectCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [categoryType] VARCHAR(MAX) NOT NULL
)