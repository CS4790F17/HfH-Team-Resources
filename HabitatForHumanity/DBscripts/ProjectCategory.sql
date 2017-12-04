ALTER TABLE Project
ADD categoryId int NOT NULL;
GO


CREATE TABLE [dbo].[ProjectCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [categoryType] VARCHAR(50) NOT NULL
)