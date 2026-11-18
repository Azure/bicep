# Bicep Playground

The Bicep Playground runs the Bicep compiler in a .NET WebAssembly worker and
uses Monaco for the Bicep and generated ARM template editors.

## Prerequisites

- The .NET SDK pinned by [`../../global.json`](../../global.json).
- Node.js and npm.
- The .NET WebAssembly tools workload:

  ```powershell
  dotnet workload install wasm-tools
  ```

The workload is required because the playground development and production
builds explicitly enable native runtime relinking when publishing `Bicep.Wasm`.
It is not required for normal solution builds, packs, or tests.

Install the npm dependencies before running the playground:

```powershell
npm ci
```

## Development

Build and stage the optimized WASM assets, then start the Vite development
server:

```powershell
npm run dev
```

Create a production build:

```powershell
npm run build
```

Run the checks:

```powershell
npm run lint
npx tsc -b
npm run test:e2e
```

## Interactive console

Select the **Console** workspace tab to evaluate Bicep expressions in the browser.
The **Editor** tab contains the Bicep and generated ARM template editors.
Workspace tabs share the header row. The sample selector and **Decompile** are
shown only in **Editor** mode. **Copy Link** and the theme switch are available
in both modes.
Compilation status, timing, and diagnostic counts are shown only in **Editor**
mode.
The tabs use `#editor` and `#console` URL routes, so either workspace can be
bookmarked or opened directly. Browser Back/Forward switches workspaces without
clearing the current editor or console session. Existing shared-code links still
open their contents in **Editor** mode. Newly copied editor links use
`#editor/<compressed-source>`; legacy links without the `editor/` prefix remain
supported. Console history links use `#console/<compressed-history>`.
Invalid shared payloads display an error without replacing the current editor
content or console session.
For example, run `var greeting = 'hello'`, then `toUpper(greeting)`. The console
also retains `type` and `func` declarations and supports redefining declarations.
The console input reuses the Monaco editor, including syntax highlighting and
high-contrast support. Submitted commands use Bicep highlighting, and results use
JSON highlighting. All follow the playground's light/dark theme. Compiler and
evaluation errors appear in the transcript without replacing the last valid
declaration.

- **Enter** or **Run** evaluates one expression or declaration.
- **Shift+Enter** inserts a newline for multiline expressions and declarations.
- **Up/Down** at the first/last line recalls input history.
- **Tab** moves focus out of the console input.
- **Clear output** clears the transcript but retains declarations and history.
- **Reset session** clears declarations, history, input, and output.

In **Console** mode, **Copy Link** includes the submitted command history in a
compressed URL fragment. Opening the link automatically starts a fresh console
session and replays commands in order, restoring declarations and recomputing
results and diagnostics. Compiler errors do not stop replay; a worker or
transport failure stops replay and reports that the session was not fully
restored.

The link includes commands even after **Clear output**, but **Reset session**
removes the previous history from future links. Unsubmitted input is not shared.
Commands support Unicode and multiline input. Anyone with the link can read
and replay the commands, so do not share secrets or sensitive data. Restoring a
link does not change the Bicep editor.

Switching between workspace tabs preserves editor content, console input, history,
and declarations. Use Tab to focus the workspace buttons and Enter or Space to
select a workspace.
Reloading the page starts
a new session. If the compiler worker crashes and restarts, the console reports
that its session is unavailable; use **Reset session** to continue.

The console is independent of the Bicep editor: editing, loading samples, and
sharing editor links do not change or include console state. It does not deploy resources
or provide Azure credentials or deployment context. File-loading functions do not
have access to files on your computer.

## Production build guardrails

The production build runs [`scripts/check-build.mjs`](./scripts/check-build.mjs)
to detect accidental startup-size regressions. It verifies:

- The initial JavaScript bundle and native .NET WASM runtime remain within
  their size budgets.
- Only the explicitly supported compiler and Monaco workers are emitted.
- The optimized .NET WASM runtime is present.
- Invariant globalization remains enabled and ICU assets are not emitted.

The budgets and worker allowlist are guardrails rather than permanent limits.
When a new language or editor feature intentionally requires another worker or
increases startup size, update the corresponding limit or allowlist in the same
pull request and document the measured impact. This keeps intentional growth
possible while preventing unused Monaco features or unoptimized WASM assets
from returning unnoticed.
