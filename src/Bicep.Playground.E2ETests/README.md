# End 2 End Tests for Playground with Playwright

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [Node.js and npm](https://nodejs.org/) (if you use any JS tooling)
- Install Playwright (only once or when updated):

   ```bash
   dotnet build
   pwsh ./bin/Debug/net10.0/playwright.ps1 install
   ```

## Getting Started

### Debug Mode

Opens a browser and shows which steps are tested.
This mode is good during development.

1. Run playground app from project folder (src/playground):

   ```bash
   npm run build
   npm run preview
   ```

2. Run tests per cmd or in your IDE:

   ```bash
   dotnet test 
   ```

### Release Mode

Doesn't open a browser. Runs test headless. This mode is used in CI.

1. Run playground app from project folder (src/playground):

   ```bash
   npm run dev
   ```

2. Run tests per cmd or in your IDE:

   ```bash
   dotnet test -c Release
   ```
