USE [TextEditor]
GO
/****** Object:  Table [dbo].[Text]    Script Date: 8/27/2019 1:56:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Text](
    [Id] [uniqueidentifier] NOT NULL,
    [Text] [nvarchar](max) NOT NULL,
    [CreationDate] [datetime] NOT NULL,
    [LastVisited] [datetime] NOT NULL,
    [Title] [nvarchar](max) NOT NULL,
CONSTRAINT [PK_Text] PRIMARY KEY CLUSTERED
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
