# VS Code E2E Tests

The E2E suite runs inside a real VS Code Extension Development Host using Vitest.

## Architecture

```mermaid
flowchart TD
    A[Node.js launcher] -->|@vscode/test-electron| B[VS Code]
    B --> C[Extension host]
    C --> D[tests/e2e/runner/extension-host.ts]
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

## Layout

- `*.test.ts` files at this directory's root are E2E specifications.
- `examples/` contains Bicep files used by those specifications.
- `utils/` contains small utilities shared by multiple E2E specifications.
- `runner/` contains the Node.js launcher and Extension Host Vitest infrastructure.
- Unit tests for runner and utility code live in owner-local `__tests__/` directories.

## Components

| File                       | Responsibility                                            |
| -------------------------- | --------------------------------------------------------- |
| `runner/launch.ts`         | Downloads and launches the requested VS Code versions     |
| `runner/extension-host.ts` | Activates Bicep and runs Vitest inside the Extension Host |
| `runner/pool.ts`           | Provides the in-process Vitest worker transport           |
| `runner/setup.ts`          | Bridges imports of `vscode` to the real host API          |
| `vscode-runtime.test.ts`   | Verifies tests are running in the Extension Host          |

## Launch Flow

`runner/launch.ts` executes in regular Node.js. For both the minimum supported VS Code version and stable, it:

1. Downloads or reuses VS Code.
2. Installs the .NET runtime extension dependency.
3. Launches an Extension Development Host.
4. Loads the compiled `out/tests/e2e/runner/extension-host.js` entry point.
5. Waits for its exported `run()` promise.

Rejecting that promise causes `@vscode/test-electron` to return exit code `1`.

`npm run build:e2e` compiles the launcher and extension-host bootstrap. Once running, Vitest discovers and
transforms the TypeScript test modules directly from `tests/e2e/*.test.ts`.

## Vitest Runner

`runner/extension-host.ts` obtains the real API with `require("vscode")`, stores it temporarily on `globalThis`,
and activates the Bicep extension before starting Vitest. The setup file maps both named and default imports of
`vscode` to that object.

The relevant configuration is:

```ts
{
  globals: true,
  include: ["tests/e2e/*.test.ts"],
  pool: createExtensionHostPool(),
  fileParallelism: false,
}
```

The custom pool connects Vitest's controller and worker runtime with in-memory callbacks. Requests and
responses are passed through `structuredClone()` to preserve the value semantics of a real process boundary.

## Extension-Host Compatibility

### `vscode` imports

Vite's module graph cannot resolve VS Code's synthetic CommonJS module by itself. `runner/setup.ts` mocks the
module with the API object captured from the extension host. This is the real API, not a test double.

### `navigator`

VS Code exposes `navigator` through a migration getter that throws when older Node-oriented code probes it.
Vite's startup dependencies perform that probe. The runner temporarily shadows `navigator` while Vitest starts
and restores the original property descriptor in `finally`.

### Output

The runner uses Vitest's built-in verbose reporter and injects stdout/stderr streams that forward output through
the Extension Host console.

## Commands

Compile the E2E sources:

```sh
npm run build:e2e
```

Run against prepared/package-layout servers:

```sh
npm run test:e2e
```

For local development, run the dedicated orchestration script:

```sh
node ./scripts/run-e2e-tests.mjs
```

The script reuses `setup-development.mjs` to install extension and UI dependencies, build the UI, and build the
language and MCP servers. It then builds the extension bundle and E2E bootstrap before launching the Extension
Development Host with `BICEP_LANGUAGE_SERVER_PATH` and `BICEP_MCP_SERVER_PATH`.

To prepare dependencies without running tests:

```sh
node ./scripts/setup-development.mjs
```

Both E2E commands run the suite against:

- the minimum version from `engines.vscode`;
- stable VS Code.

Unit and E2E tests both use Vitest. Unit tests run in regular Node.js through `npm run test:unit`; only E2E tests
need the custom Extension Host pool described here.

## Writing Tests

- Put E2E tests under `tests/e2e/` with a `.test.ts` suffix.
- Use the global Vitest APIs (`describe`, `test`, `expect`, and hooks), matching the existing files.
- Import `vscode` normally; the setup bridge supplies the real extension-host API.
- Keep tests independent and clean up editors, settings, clipboard contents, temporary files, and webviews.
- Poll observable state for asynchronous VS Code UI transitions instead of relying on fixed sleeps.
- Prefer command completion or observable state over fixed sleeps. The asynchronous editor-paste workflow is the
  only remaining test path that uses the E2E log for completion signaling.

## Troubleshooting

### No tests are discovered

Confirm the file ends in `.test.ts`, `npm run build:e2e` succeeds, and the include pattern in
`runner/extension-host.ts` still matches the file. The runner deliberately fails when no test modules are found.

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

The visualizer and deployment commands resolve only after their webviews send READY. Check the webview's startup
path and message handler rather than adding a sleep or log-file poll.

### The extension host exits with code 1 without a visible test failure

Inspect `.vscode-test/user-data/logs/` and the injected stdout/stderr stream output for an unhandled run error.

## Maintenance

The custom-pool API is experimental. Keep Vitest upgrades deliberate and validate the full suite against both
supported VS Code targets after upgrading Vitest, Vite, or `@vscode/test-electron`.
