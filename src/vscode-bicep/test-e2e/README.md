# VS Code E2E Tests

The E2E suite runs inside a real VS Code Extension Development Host using Vitest.

## Architecture

```mermaid
flowchart TD
    A[Node.js launcher] -->|@vscode/test-electron| B[VS Code]
    B --> C[Extension host]
    C --> D[test-e2e/index.ts]
    D --> E[Vitest controller]
    E <--> F[In-process custom pool]
    F --> G[E2E test modules]
    G --> H[Real vscode API]
```

The main constraint is that the real `vscode` module exists only inside VS Code's extension host.
Vitest normally executes tests in worker threads or child processes, where that module is unavailable.
The custom pool preserves Vitest's controller/worker protocol while executing its worker runtime in the
extension-host process.

The suite runs serially because tests share mutable VS Code state such as editors, settings, the clipboard,
webviews, and the language server.

## Components

| File                                | Responsibility                                          |
| ----------------------------------- | ------------------------------------------------------- |
| `run-tests.ts`                      | Downloads and launches the requested VS Code versions   |
| `index.ts`                          | Exports the `run()` function loaded by VS Code          |
| `test-runner.ts`                    | Converts framework results into test-run success/fail   |
| `vitest/vitest-test-runner.ts`      | Activates Bicep and starts Vitest in the extension host |
| `vitest/extension-host-pool.ts`     | Provides the in-process Vitest worker transport         |
| `vitest/extension-host-reporter.ts` | Adapts Vitest's verbose reporter to extension-host I/O  |
| `vitest/setup.ts`                   | Bridges imports of `vscode` to the real host API        |
| `vscode-runtime.test.ts`            | Verifies tests are running in the extension host        |

## Launch Flow

`run-tests.ts` executes in regular Node.js. For both the minimum supported VS Code version and stable, it:

1. Downloads or reuses VS Code.
2. Installs the .NET runtime extension dependency.
3. Launches an Extension Development Host.
4. Loads the compiled `out/test-e2e/index.js` entry point.
5. Waits for its exported `run()` promise.

Rejecting that promise causes `@vscode/test-electron` to return exit code `1`.

`npm run build:e2e` compiles the launcher and extension-host bootstrap. Once running, Vitest discovers and
transforms the TypeScript test modules directly from `test-e2e/**/*.test.ts`.

## Vitest Runner

`VitestTestRunner` obtains the real API with `require("vscode")`, stores it temporarily on `globalThis`, and
activates the Bicep extension before starting Vitest. The setup file maps both named and default imports of
`vscode` to that object.

The relevant configuration is:

```ts
{
  globals: true,
  include: ["test-e2e/**/*.test.ts"],
  pool: createExtensionHostPool(),
  fileParallelism: false,
  isolate: false,
}
```

The custom pool connects Vitest's controller and worker runtime with in-memory callbacks. Requests and
responses are passed through `structuredClone()` to preserve the value semantics of a real process boundary.

## Extension-Host Compatibility

### `vscode` imports

Vite's module graph cannot resolve VS Code's synthetic CommonJS module by itself. `vitest/setup.ts` mocks the
module with the API object captured from the extension host. This is the real API, not a test double.

### `navigator`

VS Code exposes `navigator` through a migration getter that throws when older Node-oriented code probes it.
Vite's startup dependencies perform that probe. The runner temporarily shadows `navigator` while Vitest starts
and restores the original property descriptor in `finally`.

### Reporter output

The reporter subclasses Vitest's public `VerboseReporter`, preserving its result formatting and timings. Test
console output is buffered until the run ends so it does not interleave with result lines.

`@microsoft/vscode-azext-utils` currently publishes ESM files with references to source maps that are absent
from its package. The reporter suppresses only Vite warnings that identify that package, an `ENOENT` failure,
and a missing `.js.map`. Other transform and source-map errors remain visible.

## Commands

Compile the E2E sources:

```sh
npm run build:e2e
```

Run against prepared/package-layout servers:

```sh
npm run test:e2e
```

For local development, build the language and MCP servers first:

```sh
dotnet build ../Bicep.LangServer/Bicep.LangServer.csproj
dotnet build ../Bicep.McpServer/Bicep.McpServer.csproj
npm run testlocal:e2e
```

`testlocal:e2e` builds the extension and E2E bootstrap, validates both local DLL paths, and passes them into the
Extension Development Host through `BICEP_LANGUAGE_SERVER_PATH` and `BICEP_MCP_SERVER_PATH`.

Both E2E commands run the suite against:

- the minimum version from `engines.vscode`;
- stable VS Code.

Unit and E2E tests both use Vitest. Unit tests run in regular Node.js through `npm run test:unit`; only E2E tests
need the custom Extension Host pool described here.

## Writing Tests

- Put E2E tests under `test-e2e/` with a `.test.ts` suffix.
- Use the global Vitest APIs (`describe`, `test`, `expect`, and hooks), matching the existing files.
- Import `vscode` normally; the setup bridge supplies the real extension-host API.
- Keep tests independent and clean up editors, settings, clipboard contents, temporary files, and webviews.
- Poll observable state for asynchronous VS Code UI transitions instead of relying on fixed sleeps.
- Resolve the extension log with `getE2eLogPath()` rather than from `__dirname`; Vitest evaluates source files
  from a different directory than the compiled launcher.

## Troubleshooting

### No tests are discovered

Confirm the file ends in `.test.ts`, `npm run build:e2e` succeeds, and the include pattern in
`vitest-test-runner.ts` still matches the file. The runner deliberately fails when no test modules are found.

### `Cannot find package 'vscode'`

The test escaped the extension-host bridge. Confirm the custom pool is selected, the setup file ran, and the
test was not started directly with the normal Vitest CLI.

### `PendingMigrationError` involving `navigator`

Confirm `startVitest()` remains inside `withoutNavigator()` and that no Vite imports were moved ahead of that
boundary.

### A local server DLL is missing

Run the corresponding build reported by the launcher:

```sh
dotnet build ../Bicep.LangServer/Bicep.LangServer.csproj
dotnet build ../Bicep.McpServer/Bicep.McpServer.csproj
```

### A webview opens but its test times out

Check that the test reads `bicep-e2e.log` through `getE2eLogPath()`. A module-relative path can point outside
the extension root when Vitest evaluates the TypeScript source.

### The extension host exits with code 1 without a visible test failure

Inspect `.vscode-test/user-data/logs/` and confirm the reporter received a nonzero failure count or an unhandled
run error.

## Maintenance

The custom-pool API is experimental. Keep Vitest upgrades deliberate and validate the full suite against both
supported VS Code targets after upgrading Vitest, Vite, or `@vscode/test-electron`.
