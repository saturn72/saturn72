CREATE TABLE [dbo].[SettingEntry]
(
	[Id]				BIGINT		IDENTITY(1,1)		NOT NULL,
	[Name]				NVARCHAR(MAX)					NOT NULL,
	[Value]				NVARCHAR(MAX)					NOT NULL,
	[RowVersion]		ROWVERSION						NOT NULL, 
	PRIMARY KEY (Id)
)
