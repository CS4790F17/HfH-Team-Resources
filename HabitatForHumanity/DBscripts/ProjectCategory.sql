ALTER TABLE Project
ADD categoryId int NULL;
GO


CREATE TABLE [dbo].[ProjectCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [categoryType] VARCHAR(50) NOT NULL
)