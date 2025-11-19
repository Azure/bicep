# End 2 End Tests for Playground with Playwright

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [Node.js and npm](https://nodejs.org/) (if you use any JS tooling)

## Getting Started

1. Install Playwright (only once or when updated):

   ```bash
   dotnet build
   pwsh ./bin/Debug/net10.0/playwright.ps1 install
   ```

2. Run playground app from project folder (src/playground):

   ```bash
   npm run dev
   ```

3. Run tests per cmd or in your IDE:

   ```bash
   dotnet test 
   ```
