USE [TextEditor]
GO

/****** Object:  Table [dbo].[Word_WordType]    Script Date: 8/20/2019 10:09:22 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Word_WordType](
	[Id_Word] [uniqueidentifier] NOT NULL,
	[Id_WordType] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Word_WordType] PRIMARY KEY CLUSTERED 
(
	[Id_Word] ASC,
	[Id_WordType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Word_WordType]  WITH CHECK ADD FOREIGN KEY([Id_Word])
REFERENCES [dbo].[Word] ([Id])
GO

ALTER TABLE [dbo].[Word_WordType]  WITH CHECK ADD FOREIGN KEY([Id_WordType])
REFERENCES [dbo].[WordType] ([Id])
GO


