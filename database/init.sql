CREATE DATABASE AISummarizer;
GO

USE AISummarizer;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Summaries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL FOREIGN KEY REFERENCES Users(Id),
    SourceUrl NVARCHAR(2000) NULL,
    SourceText NVARCHAR(MAX) NULL,
    SummaryText NVARCHAR(MAX) NOT NULL,
    SummaryType NVARCHAR(50) NOT NULL, -- Short, KeyPoints, Flashcards
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);
GO

CREATE TABLE Flashcards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SummaryId INT FOREIGN KEY REFERENCES Summaries(Id) ON DELETE CASCADE,
    FrontText NVARCHAR(1000) NOT NULL,
    BackText NVARCHAR(MAX) NOT NULL
);
GO

-- PROCEDURE: InsertSummary
CREATE PROCEDURE InsertSummary
    @UserId INT = NULL,
    @SourceUrl NVARCHAR(2000) = NULL,
    @SourceText NVARCHAR(MAX) = NULL,
    @SummaryText NVARCHAR(MAX),
    @SummaryType NVARCHAR(50)
AS
BEGIN
    INSERT INTO Summaries (UserId, SourceUrl, SourceText, SummaryText, SummaryType)
    VALUES (@UserId, @SourceUrl, @SourceText, @SummaryText, @SummaryType);
    
    SELECT SCOPE_IDENTITY() AS NewSummaryId;
END
GO

-- PROCEDURE: GetSummaries
CREATE PROCEDURE GetSummaries
    @UserId INT = NULL
AS
BEGIN
    SELECT * FROM Summaries WHERE (@UserId IS NULL OR UserId = @UserId) ORDER BY CreatedAt DESC;
END
GO

-- PROCEDURE: DeleteSummary
CREATE PROCEDURE DeleteSummary
    @Id INT
AS
BEGIN
    DELETE FROM Summaries WHERE Id = @Id;
END
GO
