# Bicep Playground Improvement Plan

## Goal

Improve the playground's correctness, privacy, accessibility, responsiveness,
and startup performance without breaking shared links or core compiler
workflows.

## Status

| Phase                            | Status   | Outcome                                                                                             |
| -------------------------------- | -------- | --------------------------------------------------------------------------------------------------- |
| Reliability and worker migration | Complete | Compiler work runs in a Web Worker with request ordering, recovery, and privacy-safe telemetry.     |
| Native interface                 | Complete | Bootstrap was removed in favor of accessible, responsive React and CSS.                             |
| Refresh and version links        | Complete | Refresh icon sizing was corrected, releases link to tags, and development versions link to commits. |
| Build and bundle optimization    | Complete | WASM is native-relinked, Monaco and telemetry are deferred, and static copying was removed.         |

Pako remains intentionally. Its small size does not justify the compatibility
risk of changing the existing shared-link compression format.

## Current Architecture

- The .NET runtime and Bicep compiler run in a module Web Worker.
- Compilation and semantic-token requests are serialized and superseded work is
  discarded.
- Monaco starts from `editor.api`; required contributions, languages, and
  workers are imported explicitly.
- Application Insights loads after the application renders and shared-link
  content is removed from the URL.
- Release builds publish `Bicep.Wasm` with native relinking, invariant
  globalization, and invariant timezone data.
- Optimized `_framework` assets are staged through Node standard-library
  scripts instead of `vite-plugin-static-copy`.

## Measured Results

| Asset                          |   Before |    After | Reduction |
| ------------------------------ | -------: | -------: | --------: |
| Initial application JavaScript | 4,358 kB |   285 kB |       93% |
| Native .NET WASM runtime       | 3,002 kB | 1,340 kB |       55% |

The optimized production preview also shows an observed compile-time
improvement. No percentage is recorded because compilation timing varies by
template, browser, and machine.

## Build Guardrails

The production build verifies that:

- Initial JavaScript and native WASM stay within reviewed size budgets.
- Only the compiler, editor, and JSON workers are emitted.
- The optimized .NET runtime is present.
- Invariant globalization remains active and ICU files are absent.

These are review guardrails, not permanent limits. A future language or editor
feature may update a budget or worker allowlist in the same pull request with
its measured impact.

## Validation

The optimized Release artifact is validated with:

- Playground lint and TypeScript checks.
- Bicep WASM unit tests.
- Production Vite build and asset-budget checks.
- Playwright tests against the production preview.
- Production dependency audit.

The browser suite covers startup and worker recovery, compilation,
decompilation, diagnostics, semantic highlighting, local modules, shared links,
telemetry privacy, accessibility, and responsive behavior.

## Remaining Guidance

- Keep new runtime dependencies exceptional and explicitly justified.
- Preserve the current shared-link format.
- Add a browser regression for any issue found only in the trimmed or
  native-relinked WASM build.
- Measure startup and bundle impact when adding Monaco contributions or
  workers.
