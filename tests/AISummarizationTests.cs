using Xunit;
using AISummarizer.Backend.Services;
using System.Threading.Tasks;

namespace AISummarizer.Tests;

public class AISummarizationTests
{
    private readonly IAISummarizationService _service;

    public AISummarizationTests()
    {
        _service = new MockAISummarizationService();
    }

    [Fact]
    public async Task SummarizeAsync_ReturnsShortSummary_ForShortType()
    {
        var result = await _service.SummarizeAsync("Some long text that needs to be summarized.", "Short");
        Assert.Equal("This is a brief, mock summary of the provided content.", result);
    }

    [Fact]
    public async Task SummarizeAsync_ReturnsKeyPoints_ForKeyPointsType()
    {
        var result = await _service.SummarizeAsync("Some long text.", "KeyPoints");
        Assert.Contains("First key point", result);
    }

    [Fact]
    public async Task SummarizeAsync_ReturnsFlashcards_ForFlashcardsType()
    {
        var result = await _service.SummarizeAsync("Some long text.", "Flashcards");
        Assert.Contains("Front", result);
        Assert.Contains("Back", result);
    }
}
