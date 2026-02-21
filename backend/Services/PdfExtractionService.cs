using System.Text;
using UglyToad.PdfPig;

namespace AISummarizer.Backend.Services;

public class PdfExtractionService
{
    public string ExtractTextFromPdf(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);
        var textBuilder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            textBuilder.AppendLine(page.Text);
        }

        return textBuilder.ToString();
    }
}
