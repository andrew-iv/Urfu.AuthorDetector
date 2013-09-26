
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/21/2013 00:04:57
-- Generated from EDMX file: C:\Users\andrew-iv\documents\visual studio 2012\Projects\Urfu.AuthorDetector\Urfu.AuthorDetector.DataLayer\Statistics.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AuthorDetector];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_PostAuthor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Posts] DROP CONSTRAINT [FK_PostAuthor];
GO
IF OBJECT_ID(N'[dbo].[FK_ThemePost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Posts] DROP CONSTRAINT [FK_ThemePost];
GO
IF OBJECT_ID(N'[dbo].[FK_ForumTheme]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Themes] DROP CONSTRAINT [FK_ForumTheme];
GO
IF OBJECT_ID(N'[dbo].[FK_ForumAuthor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Authors] DROP CONSTRAINT [FK_ForumAuthor];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Posts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Posts];
GO
IF OBJECT_ID(N'[dbo].[Themes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Themes];
GO
IF OBJECT_ID(N'[dbo].[Authors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Authors];
GO
IF OBJECT_ID(N'[dbo].[ForumSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ForumSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Posts'
CREATE TABLE [dbo].[Posts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NULL,
    [IdOnForum] int  NOT NULL,
    [Author_Id] int  NOT NULL,
    [Theme_Id] int  NULL
);
GO

-- Creating table 'Themes'
CREATE TABLE [dbo].[Themes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [IdOnForum] int  NOT NULL,
    [Forum_Id] int  NOT NULL
);
GO

-- Creating table 'Authors'
CREATE TABLE [dbo].[Authors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identity] nvarchar(max)  NOT NULL,
    [DisplayName] nvarchar(max)  NULL,
    [Forum_Id] int  NOT NULL
);
GO

-- Creating table 'ForumSet'
CREATE TABLE [dbo].[ForumSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ForumUrl] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Posts'
ALTER TABLE [dbo].[Posts]
ADD CONSTRAINT [PK_Posts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Themes'
ALTER TABLE [dbo].[Themes]
ADD CONSTRAINT [PK_Themes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Authors'
ALTER TABLE [dbo].[Authors]
ADD CONSTRAINT [PK_Authors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ForumSet'
ALTER TABLE [dbo].[ForumSet]
ADD CONSTRAINT [PK_ForumSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Author_Id] in table 'Posts'
ALTER TABLE [dbo].[Posts]
ADD CONSTRAINT [FK_PostAuthor]
    FOREIGN KEY ([Author_Id])
    REFERENCES [dbo].[Authors]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PostAuthor'
CREATE INDEX [IX_FK_PostAuthor]
ON [dbo].[Posts]
    ([Author_Id]);
GO

-- Creating foreign key on [Theme_Id] in table 'Posts'
ALTER TABLE [dbo].[Posts]
ADD CONSTRAINT [FK_ThemePost]
    FOREIGN KEY ([Theme_Id])
    REFERENCES [dbo].[Themes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ThemePost'
CREATE INDEX [IX_FK_ThemePost]
ON [dbo].[Posts]
    ([Theme_Id]);
GO

-- Creating foreign key on [Forum_Id] in table 'Themes'
ALTER TABLE [dbo].[Themes]
ADD CONSTRAINT [FK_ForumTheme]
    FOREIGN KEY ([Forum_Id])
    REFERENCES [dbo].[ForumSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ForumTheme'
CREATE INDEX [IX_FK_ForumTheme]
ON [dbo].[Themes]
    ([Forum_Id]);
GO

-- Creating foreign key on [Forum_Id] in table 'Authors'
ALTER TABLE [dbo].[Authors]
ADD CONSTRAINT [FK_ForumAuthor]
    FOREIGN KEY ([Forum_Id])
    REFERENCES [dbo].[ForumSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ForumAuthor'
CREATE INDEX [IX_FK_ForumAuthor]
ON [dbo].[Authors]
    ([Forum_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------