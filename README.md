# AI Summarizer Chrome Extension

This project is an Enterprise-grade Chrome Extension that leverages an AI backend to summarize web pages and uploaded PDF files, offering multiple output formats (Short, Key Points, Flashcards) with local and cloud storage capabilities.

## Architecture
- **Backend:** C# .NET 8 Web API utilizing Dapper, Minimal APIs, and UglyToad.PdfPig for PDF Extraction.
- **Frontend:** Angular compiled via `manifest.json` for Chrome Extension popup output, styled with Glassmorphism CSS.
- **Database:** MS SQL Server containerized with Docker, thoroughly tested using `tSQLt`.

## How to Run

### 1. Backend API
1. Have the .NET 8 SDK installed.
2. Navigate to `/backend`.
3. Run `dotnet run`. The API will start on `http://localhost:5033/`.

### 2. Database
Run a local MS SQL Server 2022 image (via Docker) on port `1433`. Execute the scripts found in `database/init.sql` to initialize the tables (`Users`, `Summaries`, `Flashcards`) and stored procedures.

### 3. Frontend Extension
1. Ensure Node.js and npm are installed.
2. Navigate to `/frontend` and run `npm install` then `npm run build`.
3. Open Google Chrome and go to `chrome://extensions`.
4. Enable **Developer mode** (top right corner).
5. Click **Load unpacked** and select `/frontend/dist/frontend/browser`.
6. Click on the extension to summarize your current page or an uploaded PDF!
