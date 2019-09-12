
USE [TextEditor]
GO

/****** Object:  Table [dbo].[User_Text]    Script Date: 8/27/2019 1:50:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User_Text](
	[Id_User] [uniqueidentifier] NOT NULL,
	[Id_Text] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_User_Text] PRIMARY KEY CLUSTERED 
(
	[Id_Text] ASC,
	[Id_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [User_Text]
ADD FOREIGN KEY ([Id_Text]) REFERENCES [Text]([Id]);

ALTER TABLE [User_Text]
ADD FOREIGN KEY ([Id_User]) REFERENCES [User]([Id]);
