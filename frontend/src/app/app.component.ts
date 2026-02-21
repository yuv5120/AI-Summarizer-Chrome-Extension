import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  summaryType = 'Short';
  summaryResult = '';
  isLoading = false;
  statusMessage = '';
  isError = false;

  private API_URL = 'http://localhost:5033/api/summarize';

  onSummaryTypeChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.summaryType = target.value;
  }

  async summarizeCurrentPage() {
    this.isLoading = true;
    this.statusMessage = 'Extracting page content...';
    this.isError = false;
    this.summaryResult = '';

    try {
      if (typeof chrome !== 'undefined' && chrome.tabs) {
        // Execute script in active tab to get innerText
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
        if (tab.id) {
          const results = await chrome.scripting.executeScript({
            target: { tabId: tab.id },
            func: () => document.body.innerText
          });

          if (results && results[0]) {
            const text = results[0].result as string;
            await this.callApiWebpage(text, tab.url || '');
          } else {
            throw new Error('Could not extract text from this page.');
          }
        }
      } else {
        // Web mode fallback
        await this.callApiWebpage('This is test content because Chrome extension API is not available.', window.location.href);
      }
    } catch (err: any) {
      this.isError = true;
      this.statusMessage = err.message || 'Summarization failed.';
      this.isLoading = false;
    }
  }

  async onFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      const file = target.files[0];
      await this.callApiPdf(file);
    }
  }

  private async callApiWebpage(text: string, url: string) {
    this.statusMessage = 'Summarizing...';
    try {
      const response = await fetch(`${this.API_URL}/webpage`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          sourceUrl: url,
          sourceText: text.substring(0, 15000), // Limit text to prevent massive payloads
          summaryType: this.summaryType
        })
      });

      if (!response.ok) throw new Error('API Error');
      const data = await response.json();
      this.summaryResult = data.summary;
      this.statusMessage = 'Success!';
    } catch (error) {
      this.isError = true;
      this.statusMessage = 'Failed to reach API server.';
    }
    this.isLoading = false;
  }

  private async callApiPdf(file: File) {
    this.isLoading = true;
    this.statusMessage = 'Uploading and processing PDF...';
    this.isError = false;
    this.summaryResult = '';

    const formData = new FormData();
    formData.append('pdf', file);
    formData.append('summaryType', this.summaryType);

    try {
      const response = await fetch(`${this.API_URL}/pdf`, {
        method: 'POST',
        body: formData
      });

      if (!response.ok) throw new Error('API Error');
      const data = await response.json();
      this.summaryResult = data.summary;
      this.statusMessage = 'Success!';
    } catch (error) {
      this.isError = true;
      this.statusMessage = 'Failed to process PDF.';
    }
    this.isLoading = false;
  }

  copyToClipboard() {
    if (this.summaryResult) {
      navigator.clipboard.writeText(this.summaryResult);
      this.statusMessage = 'Copied to clipboard!';
      setTimeout(() => this.statusMessage = '', 3000);
    }
  }
}
