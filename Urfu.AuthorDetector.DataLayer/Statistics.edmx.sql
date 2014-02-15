
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 02/10/2014 22:17:55
-- Generated from EDMX file: C:\Users\andrew-iv\Documents\Visual Studio 2012\Projects\Urfu.AuthorDetector2\Urfu.AuthorDetector.DataLayer\Statistics.edmx
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
IF OBJECT_ID(N'[dbo].[FK_ClassifierVersionClassifierResult]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClassifierResultSet] DROP CONSTRAINT [FK_ClassifierVersionClassifierResult];
GO
IF OBJECT_ID(N'[dbo].[FK_ClassifierResultClassifierParams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ClassifierParamsSet] DROP CONSTRAINT [FK_ClassifierResultClassifierParams];
GO
IF OBJECT_ID(N'[dbo].[FK_ClassifierVersionBayesClassifierTest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BayesClassifierTestSet] DROP CONSTRAINT [FK_ClassifierVersionBayesClassifierTest];
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
IF OBJECT_ID(N'[dbo].[ClassifierVersionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClassifierVersionSet];
GO
IF OBJECT_ID(N'[dbo].[ClassifierResultSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClassifierResultSet];
GO
IF OBJECT_ID(N'[dbo].[ClassifierParamsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClassifierParamsSet];
GO
IF OBJECT_ID(N'[dbo].[BayesClassifierTestSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BayesClassifierTestSet];
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
    [IdOnForum] bigint  NOT NULL,
    [Forum_Id] int  NOT NULL
);
GO

-- Creating table 'Authors'
CREATE TABLE [dbo].[Authors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Identity] nvarchar(max)  NOT NULL,
    [DisplayName] nvarchar(max)  NULL,
    [IdOnForum] bigint  NULL,
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

-- Creating table 'ClassifierVersionSet'
CREATE TABLE [dbo].[ClassifierVersionSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ClassifierResultSet'
CREATE TABLE [dbo].[ClassifierResultSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoundCount] int  NOT NULL,
    [TestsPerRound] int  NOT NULL,
    [MessageCount] int  NOT NULL,
    [LearningCount] int  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Result] float  NOT NULL,
    [ClassifierVersion_Id] int  NOT NULL
);
GO

-- Creating table 'ClassifierParamsSet'
CREATE TABLE [dbo].[ClassifierParamsSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [ClassifierResult_Id] int  NOT NULL
);
GO

-- Creating table 'BayesClassifierTestSet'
CREATE TABLE [dbo].[BayesClassifierTestSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Success] bit  NOT NULL,
    [FirstToAll] float  NOT NULL,
    [FirstToSecond] float  NULL,
    [MessageCount] int  NOT NULL,
    [MessagesLength] int  NULL,
    [ClassifierVersion_Id] int  NOT NULL
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

-- Creating primary key on [Id] in table 'ClassifierVersionSet'
ALTER TABLE [dbo].[ClassifierVersionSet]
ADD CONSTRAINT [PK_ClassifierVersionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClassifierResultSet'
ALTER TABLE [dbo].[ClassifierResultSet]
ADD CONSTRAINT [PK_ClassifierResultSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ClassifierParamsSet'
ALTER TABLE [dbo].[ClassifierParamsSet]
ADD CONSTRAINT [PK_ClassifierParamsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BayesClassifierTestSet'
ALTER TABLE [dbo].[BayesClassifierTestSet]
ADD CONSTRAINT [PK_BayesClassifierTestSet]
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

-- Creating foreign key on [ClassifierVersion_Id] in table 'ClassifierResultSet'
ALTER TABLE [dbo].[ClassifierResultSet]
ADD CONSTRAINT [FK_ClassifierVersionClassifierResult]
    FOREIGN KEY ([ClassifierVersion_Id])
    REFERENCES [dbo].[ClassifierVersionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClassifierVersionClassifierResult'
CREATE INDEX [IX_FK_ClassifierVersionClassifierResult]
ON [dbo].[ClassifierResultSet]
    ([ClassifierVersion_Id]);
GO

-- Creating foreign key on [ClassifierResult_Id] in table 'ClassifierParamsSet'
ALTER TABLE [dbo].[ClassifierParamsSet]
ADD CONSTRAINT [FK_ClassifierResultClassifierParams]
    FOREIGN KEY ([ClassifierResult_Id])
    REFERENCES [dbo].[ClassifierResultSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClassifierResultClassifierParams'
CREATE INDEX [IX_FK_ClassifierResultClassifierParams]
ON [dbo].[ClassifierParamsSet]
    ([ClassifierResult_Id]);
GO

-- Creating foreign key on [ClassifierVersion_Id] in table 'BayesClassifierTestSet'
ALTER TABLE [dbo].[BayesClassifierTestSet]
ADD CONSTRAINT [FK_ClassifierVersionBayesClassifierTest]
    FOREIGN KEY ([ClassifierVersion_Id])
    REFERENCES [dbo].[ClassifierVersionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClassifierVersionBayesClassifierTest'
CREATE INDEX [IX_FK_ClassifierVersionBayesClassifierTest]
ON [dbo].[BayesClassifierTestSet]
    ([ClassifierVersion_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------