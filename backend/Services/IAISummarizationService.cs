namespace AISummarizer.Backend.Services;

public interface IAISummarizationService
{
    Task<string> SummarizeAsync(string text, string summaryType);
}

public class MockAISummarizationService : IAISummarizationService
{
    public async Task<string> SummarizeAsync(string text, string summaryType)
    {
        // Simulate API delay
        await Task.Delay(1000);

        return summaryType.ToLower() switch
        {
            "short" => "This is a brief, mock summary of the provided content.",
            "keypoints" => "• First key point\n• Second key point\n• Third key point",
            "flashcards" => "Front: What is this?\nBack: A mock flashcard.\n---\nFront: Concept?\nBack: Mock concept.",
            _ => "A mock summary of the content."
        };
    }
}
