CREATE TABLE [dbo].[RefreshToken]
(
	[Id]					BIGINT			IDENTITY(1,1)	Primary Key	NOT NULL,
	[ClientId]				NVARCHAR(MAX)					NOT NULL,
	[Hash]					NVARCHAR(MAX)					NOT NULL,
	[ProtectedTicket]		NVARCHAR(MAX)					NOT NULL,
	[IssuedOnUtc]			DATETIME2						NOT NULL,
	[ExpiresOnUtc]			DATETIME2						NOT NULL,
	[RowVersion]			ROWVERSION						NOT NULL, 
)
