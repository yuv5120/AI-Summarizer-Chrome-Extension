using Dapper;
using Microsoft.Data.SqlClient;
using AISummarizer.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAISummarizationService, MockAISummarizationService>();
builder.Services.AddSingleton<PdfExtractionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

app.MapGet("/api/summaries", async (int? userId) =>
{
    using var connection = new SqlConnection(connectionString);
    var parameters = new { UserId = userId };
    var summaries = await connection.QueryAsync("GetSummaries", parameters, commandType: System.Data.CommandType.StoredProcedure);
    return Results.Ok(summaries);
})
.WithName("GetSummaries")
.WithOpenApi();

app.MapPost("/api/summarize/webpage", async (SummarizeWebRequest request, IAISummarizationService aiService) =>
{
    var summary = await aiService.SummarizeAsync(request.SourceText, request.SummaryType);
    
    using var connection = new SqlConnection(connectionString);
    var parameters = new DynamicParameters();
    parameters.Add("@UserId", request.UserId);
    parameters.Add("@SourceUrl", request.SourceUrl);
    parameters.Add("@SourceText", request.SourceText);
    parameters.Add("@SummaryText", summary);
    parameters.Add("@SummaryType", request.SummaryType);

    var newId = await connection.ExecuteScalarAsync<int>("InsertSummary", parameters, commandType: System.Data.CommandType.StoredProcedure);
    
    return Results.Ok(new { Id = newId, Summary = summary, Type = request.SummaryType });
})
.WithName("SummarizeWebpage")
.WithOpenApi();

app.MapPost("/api/summarize/pdf", async (HttpContext context, IAISummarizationService aiService, PdfExtractionService pdfService) =>
{
    var form = await context.Request.ReadFormAsync();
    var file = form.Files.GetFile("pdf");
    if (file == null || file.Length == 0) return Results.BadRequest("No PDF file uploaded.");
    
    var summaryType = form["summaryType"].ToString() ?? "Short";
    var userIdString = form["userId"].ToString();
    int? userId = string.IsNullOrEmpty(userIdString) ? null : int.Parse(userIdString);

    using var stream = file.OpenReadStream();
    var sourceText = pdfService.ExtractTextFromPdf(stream);
    
    var summary = await aiService.SummarizeAsync(sourceText, summaryType);

    using var connection = new SqlConnection(connectionString);
    var parameters = new DynamicParameters();
    parameters.Add("@UserId", userId);
    parameters.Add("@SourceUrl", file.FileName);
    parameters.Add("@SourceText", sourceText);
    parameters.Add("@SummaryText", summary);
    parameters.Add("@SummaryType", summaryType);

    var newId = await connection.ExecuteScalarAsync<int>("InsertSummary", parameters, commandType: System.Data.CommandType.StoredProcedure);

    return Results.Ok(new { Id = newId, Summary = summary, Type = summaryType });
})
.WithName("SummarizePdf")
.WithOpenApi();

app.Run();

public record SummarizeWebRequest(int? UserId, string SourceUrl, string SourceText, string SummaryType);
