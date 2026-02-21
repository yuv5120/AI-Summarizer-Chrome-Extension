USE AISummarizer;
GO

EXEC tSQLt.NewTestClass 'SummaryTests';
GO

CREATE PROCEDURE SummaryTests.[test InsertSummary stores summary logic correctly]
AS
BEGIN
    -- Arrange
    EXEC tSQLt.FakeTable 'Summaries';

    -- Act
    DECLARE @NewId INT;
    EXEC InsertSummary 
        @UserId = 1,
        @SourceUrl = 'http://example.com',
        @SourceText = 'Full article text',
        @SummaryText = 'Short summary',
        @SummaryType = 'Short';

    -- Assert
    SELECT UserId, SourceUrl, SourceText, SummaryText, SummaryType 
    INTO #Actual 
    FROM Summaries;

    SELECT TOP(0) * INTO #Expected FROM #Actual;
    INSERT INTO #Expected (UserId, SourceUrl, SourceText, SummaryText, SummaryType)
    VALUES (1, 'http://example.com', 'Full article text', 'Short summary', 'Short');

    EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual';
END
GO
