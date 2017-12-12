CREATE TABLE [dbo].[ClientApp]
(
	[Id]					BIGINT			IDENTITY(1,1)	Primary Key	NOT NULL,
	[ClientId]				NVARCHAR(MAX)					NOT NULL,
	[Secret]				NVARCHAR(MAX)					,
	[Name]					NVARCHAR(MAX)					NOT NULL,
	[ApplicationType]		INT								NOT NULL,
	[Active]				BIT				Default(0)						NOT NULL,
	[RefreshTokenLifeTime]	INT							NOT NULL,
	[AllowedOrigin]			NVARCHAR(MAX)					NOT NULL,
	[RowVersion]			ROWVERSION						NOT NULL, 
)