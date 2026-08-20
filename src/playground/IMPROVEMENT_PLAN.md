# Bicep Playground Improvement Plan

## Purpose

Improve the playground's correctness, privacy, reliability, accessibility, responsiveness, and startup performance while minimizing npm dependencies and keeping each pull request focused and reviewable.

This plan favors:

- Native browser APIs over npm packages.
- React state and focused hooks over a state-management dependency by default.
- Semantic HTML and local CSS over component libraries.
- Small pull requests with measurable acceptance criteria.
- Compatibility with existing shared links and playground workflows.

## Desired Outcome

The completed work should provide:

- Deterministic compilation output and diagnostics.
- No template source in telemetry.
- Recoverable loading and error states without loss of editor content.
- Accessible and responsive source and output editors.
- A materially smaller JavaScript bundle and optimized WASM deployment.
- Fewer production and development dependencies.
- Automated coverage for races, error paths, accessibility, and shared links.

## Progress Tracker

**Last updated:** 2026-08-19

| Pull request | Phase | Status      | Notes                                                                                          |
| ------------ | ----- | ----------- | ---------------------------------------------------------------------------------------------- |
| PR 1         | A     | Complete    | Reliability, privacy, lifecycle, and recoverable error handling are implemented.               |
| PR 1         | B     | Complete    | The .NET compiler runs in a module Web Worker and all worker-hosted tests pass.                |
| PR 2         | —     | Not started | Removes Bootstrap and React-Bootstrap, then completes responsive and accessibility work.       |
| PR 3         | —     | Not started | Replaces Pako while preserving existing links.                                                 |
| PR 4         | —     | Not started | Optimizes WASM and Monaco and removes the static-copy plugin.                                  |
| PR 5         | —     | Optional    | Use only for remaining test consolidation or a demonstrably justified state-management change. |

### Completed in the current working tree

- Shared-link content is consumed before Application Insights initialization.
- Page-view and telemetry URL fields have query strings and fragments removed.
- Bicep source state updates before compilation completes.
- Compilation is debounced and stale results are rejected by request ID and Monaco model version.
- Monaco callbacks use the latest props, including the selected quickstart `sourcePath`.
- Monaco event subscriptions, editors, models, language providers, timers, and fetches have explicit cleanup.
- Loading samples and decompiling files no longer unmounts the editors.
- Sample fetch, clipboard, decompile, compiler startup, and compiler invocation errors are surfaced in accessible UI.
- Compiler startup has a timeout and a reload-based retry that cannot bootstrap Blazor twice.
- Sample requests are abortable, uploads are limited to 10 MB, and the file input is reset after use.
- WASM compilation and semantic-token generation are serialized around the shared in-memory file system.
- Compiler exceptions are returned as errors instead of being rendered as successful ARM output.
- E2E coverage was added for sample-fetch failure/content preservation and compiler download failure/retry.
- Clipboard-denial E2E coverage was added and passes.
- Invalid-decompile preservation coverage exposed and verified the `null` interop result path.
- The existing local-module and immediate Copy Link E2E workflows pass with the new lifecycle.
- The .NET runtime now starts inside a Vite module Web Worker through `_framework/dotnet.js`.
- Compilation, diagnostics, semantic tokens, decompilation, and recursive quickstart module loading run through typed worker messages.
- The main browser thread no longer loads `blazor.webassembly.js` or executes compiler work.
- The worker client rejects pending requests on worker failure and terminates cleanly on application disposal.
- The latest matching compilation is reused between semantic tokens and template emission.
- A large-template heartbeat E2E test verifies that main-thread timers continue advancing during compiler work.

### Current design decisions

- **Jotai was not added.** The current PR remains understandable with React state, refs, and focused lifecycle logic. Re-evaluate after PR 2 splits the toolbar and workspace; add Jotai only if that split creates substantial cross-component coordination or prop drilling.
- **No new npm dependencies were added.**
- Retry reloads the page rather than injecting the Blazor bootstrap script a second time.
- Compilation requests are serialized in WASM for correctness. Parallelism should not be reintroduced until each compilation has an isolated file workspace.

### Validation completed

- `npm run lint` — passed. Note that TSX lint coverage remains a planned PR 2 task.
- `npx tsc -b --force` — passed.
- `dotnet build ..\Bicep.Wasm\Bicep.Wasm.csproj --configuration Release --nologo` — passed with zero warnings.
- `dotnet build ..\Bicep.Playground.E2ETests\Bicep.Playground.E2ETests.csproj --configuration Release --nologo` — passed with zero warnings.
- `npm run build` — passed.
- `npx vite build` — passed after the final frontend changes.
- Complete playground E2E suite with worker hosting — 9 passed, 0 failed.

The production build still reports unoptimized WASM because the local `wasm-tools` workload is unavailable. Installing and enforcing that workload remains PR 4 scope.

### Next-session starting point

1. Review and publish the combined PR 1.
2. Begin PR 2 with native toolbar and layout replacement after PR 1 is ready for review.
3. Keep the React-facing `DotnetInterop` interface stable.
4. Carry the explicitly deferred worker protocol and cancellation items below into PR 5 unless production feedback raises their priority.

## Dependency Strategy

### Current runtime dependencies

| Dependency                           | Decision                | Rationale                                                                                                                                          |
| ------------------------------------ | ----------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| `react`                              | Keep                    | Core rendering library.                                                                                                                            |
| `react-dom`                          | Keep                    | Required by React browser rendering.                                                                                                               |
| `monaco-editor`                      | Keep and narrow imports | Core editing experience, but the current package entry point includes unnecessary languages and workers.                                           |
| `@microsoft/applicationinsights-web` | Keep initially          | Existing telemetry integration. It must be initialized safely and may be lazy-loaded later.                                                        |
| `bootstrap`                          | Remove                  | The playground uses a small subset that can be implemented with semantic HTML and local CSS.                                                       |
| `react-bootstrap`                    | Remove                  | Native controls and small local components can replace the toolbar, menu, spinner, and layout primitives.                                          |
| `pako`                               | Remove                  | Modern browsers provide `CompressionStream`, `DecompressionStream`, `TextEncoder`, and `TextDecoder`. Existing shared links must remain decodable. |

The default target is **four runtime dependencies**:

1. `react`
2. `react-dom`
3. `monaco-editor`
4. `@microsoft/applicationinsights-web`

### Jotai decision

Jotai is the only approved optional runtime addition. It should not be added preemptively.

Start with focused hooks plus `useReducer`. Add Jotai only if the first implementation demonstrates at least one of these problems:

- State must be synchronized across the toolbar, both editors, URL handling, and async compilation through substantial prop drilling.
- Async compilation state requires multiple ad hoc contexts or duplicated coordination logic.
- State transitions become difficult to test independently of the component tree.
- Adding a new workflow requires unrelated parent components to understand editor internals.

If Jotai is adopted:

- Use primitive and derived atoms rather than a monolithic store.
- Keep Monaco editor instances, DOM nodes, timers, and abort controllers outside persisted atoms.
- Keep external effects in hooks or services rather than atom write functions where practical.
- Do not add another state-management or server-state package.
- Record the dependency-count trade-off and the concrete code it replaces in the pull request description.

The target with Jotai would be five runtime dependencies. If `useReducer` and focused hooks remain clear, do not add it.

### Development dependency review

Development dependencies should also be reduced where doing so does not weaken validation:

- Remove `@types/pako` with `pako`.
- Replace `vite-plugin-static-copy` with Vite's `publicDir` configuration or a small Node standard-library build step.
- Evaluate removing `eslint-plugin-prettier` and `eslint-config-prettier`. Formatting and linting should be separate if the repository already supplies a formatter workflow.
- Keep TypeScript, ESLint, Vite, the React Vite plugin, and required type packages.
- Do not add a component library, CSS-in-JS library, form library, request library, utility library, or test runner solely for this project.

## Pull Request Sequence

### PR 1, Phase A: Correctness, privacy, and editor lifecycle

#### Scope

- [x] Process and sanitize shared-link URLs before Application Insights initialization.
- [x] Add a telemetry initializer that strips query strings and fragments from URL-bearing telemetry.
- [x] Update Bicep source state immediately when the editor changes.
- [x] Debounce compilation and assign each request a model version or monotonically increasing request ID.
- [x] Ignore stale compilation results.
- [x] Keep editors mounted while samples load or files decompile.
- [x] Replace the global loading boolean with explicit operation state.
- [x] Dispose Monaco editors, models, and event subscriptions on unmount.
- [x] Ensure Monaco listeners use the latest callback and `sourcePath`.
- [x] Add startup timeout, script-load failure handling, and a retryable initialization error.
- [x] Handle fetch, clipboard, and file-reading failures without `alert()`.
- [x] Reset the file input after each decompile attempt.
- [x] Serialize WASM compilation access to shared in-memory files.

#### Suggested structure

```text
src/
  components/
    EditorWorkspace.tsx
    OperationStatus.tsx
    PlaygroundToolbar.tsx
  hooks/
    useCompilation.ts
    usePlaygroundState.ts
  services/
    quickstartClient.ts
    telemetry.ts
```

Names may change to match the final responsibilities. Avoid extracting components that only wrap one element without adding behavior or clarity.

#### State-management checkpoint

Implement the first pass with React built-ins and focused hooks. Before merging, document whether Jotai would remove meaningful coordination complexity. Do not combine a speculative Jotai migration with the correctness fixes.

Current decision: React state and refs remain sufficient for PR 1, so Jotai was not added. Re-evaluate after component extraction in PR 2.

#### Tests

- [ ] A slower old compilation cannot overwrite a newer result.
- [x] Copy Link uses the latest editor text even while compilation is running.
- [x] A changed sample `sourcePath` is used by the Monaco callback.
- [ ] A sample fetch failure preserves source, output, cursor position, and focus. Source preservation and visible error feedback are covered; output, cursor, and focus assertions remain.
- [x] A decompile failure preserves editor content and displays an accessible error.
- [x] Clipboard rejection displays an accessible error.
- [ ] A Blazor initialization timeout displays retry UI.
- [x] A Blazor script download failure displays retry UI and Retry recovers.
- [ ] Telemetry never receives a URL query string or fragment.
- [ ] Concurrent compilation requests cannot corrupt the WASM in-memory workspace.

#### Acceptance criteria

- No shared Bicep source is emitted through page-view telemetry.
- JSON and diagnostics always correspond to the latest accepted editor version.
- Loading and failure paths do not unmount the editors or discard user content.
- No Monaco editor, model, or listener remains after unmount.
- All error paths provide visible, non-blocking, accessible feedback.

### PR 1, Phase B: Compiler Web Worker and compilation reuse

This is the highest-priority performance change. It addresses UI freezing; it does not primarily reduce compiler wall-clock time or initial WASM transfer size.

Reference implementation guidance: [Run .NET on Web Workers](https://learn.microsoft.com/aspnet/core/client-side/dotnet-on-webworkers?view=aspnetcore-10.0).

#### Important constraint

Do not load `blazor.webassembly.js` inside a worker. The current Blazor host assumes a browser-page environment and initializes through `WebAssemblyHostBuilder` plus `IJSRuntime`.

The worker must instead import `_framework/dotnet.js`, call `dotnet.create()`, obtain the main assembly exports with `getAssemblyExports`, and invoke a worker-compatible exported .NET façade. This requires a deliberate host refactor in `Bicep.Wasm`; it is not only a TypeScript file move.

#### Target architecture

```text
React / Monaco main thread
  |
  | CompilerRequest / CompilerResponse messages
  v
CompilerWorkerClient
  |
  | postMessage
  v
compiler.worker.ts
  |
  | dotnet.create() + getAssemblyExports()
  v
Bicep.Wasm exported CompilerHost
  |
  +-- compilation and diagnostic generation
  +-- semantic tokens
  +-- decompilation
  +-- quickstart module loading and cache
```

Keep the existing `DotnetInterop` interface as the React-facing boundary where practical. `CompilerWorkerClient` should implement it so most components do not know whether .NET runs in-process or in a worker.

#### Message protocol

Define a shared, dependency-free TypeScript protocol using discriminated unions.

Requests:

- `compile`: request ID, document revision, source content, optional `sourcePath`.
- `semanticTokens`: request ID, document revision, source content, optional `sourcePath`.
- `decompile`: request ID and JSON content.
- `dispose`: explicit worker shutdown.

Responses:

- `ready`: worker and .NET exports are initialized.
- `result`: request ID, document revision, and typed result.
- `error`: request ID when available, stable error code, and user-safe message.
- `fatal`: startup or worker-level failure requiring restart.

Every request must have exactly one terminal response. The main-thread client must remove settled requests from its pending map and reject all pending promises if the worker crashes or is terminated.

Do not send telemetry, Monaco objects, DOM objects, or application state through the protocol. Send plain structured-clone-compatible data only.

#### Implementation slices

##### Slice A: Prove the worker host

1. Add a module worker created through Vite:
   `new Worker(new URL("./compiler.worker.ts", import.meta.url), { type: "module" })`.
2. Pass an absolute framework base URL derived from `document.baseURI` so development, preview, and the GitHub Pages subpath resolve the same assets.
3. Dynamically import `_framework/dotnet.js` inside the worker.
4. Call `dotnet.create()`, `getConfig()`, and `getAssemblyExports()`.
5. Export one minimal .NET smoke-test method and call it through the request protocol.
6. Verify worker startup in `vite dev`, `vite preview`, and the packaged GitHub Pages layout.

The slice is complete only when the main thread never loads or executes `blazor.webassembly.js`.

##### Slice B: Refactor the .NET host

1. Replace page-oriented Blazor startup with a worker-compatible .NET WebAssembly entry point.
2. Move service registration currently performed in `Program.Main` into a compiler-host factory that can be created once per worker.
3. Replace the `DotNetObjectReference`/`JSInvokable` bootstrap with a static exported façade suitable for `getAssemblyExports`.
4. Preserve typed result contracts for compilation, diagnostics, semantic tokens, and decompilation.
5. Replace `IJSRuntime` quickstart callbacks with a worker-safe import or worker-owned fetch service.
6. Keep quickstart network access pinned to the existing commit and validate paths before fetching.
7. Return user-safe errors across the boundary; keep stack traces inside worker diagnostics or development logging.

Prefer a small number of coarse-grained exported methods. Avoid a generic reflection-based RPC surface.

##### Slice C: Move all compiler operations

1. Implement `CompilerWorkerClient` with request IDs, a pending-promise map, startup timeout, and `terminate()`.
2. Route compilation, semantic tokens, decompilation, and recursive quickstart module loading through the worker.
3. Render the application shell and Monaco immediately while the worker initializes.
4. Disable compiler-dependent commands until the `ready` response, but keep source editing available.
5. Replace the current page reload retry with worker termination and recreation.
6. Remove the main-thread script injection and global `InteropInitialize`/`LoadQuickstartsFile` callbacks.
7. Remove the WASM semaphore once all compiler access is confined to one worker queue and no shared state can be entered concurrently.

##### Slice D: Eliminate duplicate compilation

Moving work to a worker keeps the UI responsive, but compiling the same revision twice remains wasteful.

1. [x] Cache the most recent compilation by exact source content and `sourcePath`.
2. [x] Derive emitted JSON, diagnostics, and semantic tokens from the same cached compilation.
3. [ ] Prefer a coarse-grained `analyze` operation that returns JSON, diagnostics, and semantic tokens for one revision.
4. [ ] Let the Monaco semantic-token provider read the matching cached token result and signal Monaco when a newer analysis is ready.
5. [x] Cache downloaded quickstart module text in the worker-owned in-memory file system.
6. [ ] Bound module caches by entry count or total bytes. The compilation cache is already bounded to one entry and is released with the worker.

Do not key correctness only on a non-cryptographic content hash. Include the source text or verify equality on a hash match.

##### Slice E: Resilience and observability

1. Reject pending requests on `error` and `messageerror`.
2. Detect worker startup timeout separately from operation timeout.
3. Allow one explicit restart without creating multiple .NET runtimes.
4. Add performance marks for worker startup, queue wait, module loading, compilation, emission, and total response time.
5. Report durations and error categories without source content, module contents, URLs containing fragments, or generated ARM JSON.
6. Show distinct UI states for worker initialization, analysis in progress, recoverable operation failure, and fatal worker failure.

#### Cancellation semantics

Request IDs and document revisions prevent stale results from being applied, but they do not stop CPU work already executing in .NET.

The first worker version should:

- Debounce before posting.
- Coalesce queued analyses so only the newest not-yet-started revision runs.
- Drop stale responses on the main thread.
- Keep the UI responsive while an unavoidable in-flight analysis finishes.

Do not claim true cancellation in this PR. A worker cannot process a cancel message while synchronous compiler work is occupying its event loop. True in-flight cancellation requires compiler cancellation-token support, a safe shared-memory cancellation signal, or terminating and paying the cost to recreate the .NET runtime.

#### Tests

- [ ] Worker protocol resolves, rejects, and removes each pending request exactly once.
- [x] Worker startup succeeds from the packaged preview base path.
- [x] Worker startup failure displays retry UI; Retry creates a replacement worker/runtime through page reload.
- [x] Compilation, diagnostics, semantic tokens, decompilation, and local quickstart modules match current behavior.
- [x] A stale response cannot replace output or diagnostics for a newer revision through the existing request and model-version guards.
- [ ] Rapid edits coalesce queued analysis requests. The React debounce prevents most redundant compile messages, but worker-queue coalescing is deferred.
- [ ] A worker crash after successful startup rejects pending operations and recovers without a page reload.
- [ ] No Bicep source or ARM JSON crosses telemetry boundaries.
- [x] A timer heartbeat continues advancing while a large template compiles.
- [ ] Toolbar focus, typing, scrolling, and Copy Link remain responsive during compilation. Timer responsiveness is covered; direct interaction assertions remain.

#### Performance acceptance criteria

- No compiler or decompiler operation executes on the main browser thread.
- No .NET runtime is initialized on the main browser global scope.
- The quickstart responsiveness E2E test observes no heartbeat gap of 250 ms or more across sample selection, model replacement, compilation, and result rendering.
- Compiler work creates no main-thread execution because the page global scope never initializes .NET.
- Matching semantic-token and template requests reuse one source-path-aware cached compilation.
- Switching templates may still take time to produce output, but it does not freeze typing, focus, scrolling, or toolbar interaction.
- Worker restart does not leak a previous worker, pending promise, event listener, or .NET runtime.
- No npm runtime dependency is added.

#### Risks and mitigations

| Risk                                                                 | Mitigation                                                                                                             |
| -------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| Worker asset URLs fail under the GitHub Pages subpath.               | Pass an absolute framework base URL from the page and test the packaged relative-base layout.                          |
| Current Blazor APIs depend on `window`, `document`, or `IJSRuntime`. | Use the `dotnet.js` worker host and a worker-compatible exported .NET façade rather than loading Blazor in the worker. |
| Large strings incur structured-clone and serialization cost.         | Measure transport separately; keep protocol coarse-grained and avoid sending duplicate source/results.                 |
| Semantic tokens trigger duplicate compilation.                       | Return all derived analysis results from one cached compilation.                                                       |
| A stale long-running compilation delays the latest result.           | Debounce and coalesce before execution; defer true in-flight cancellation until supported safely.                      |
| Worker failure strands promises.                                     | Reject and clear every pending request on worker errors and termination.                                               |
| Multiple retries load multiple runtimes.                             | Centralize worker ownership and terminate the old worker before replacement.                                           |

#### PR 1 Phase B definition of done

- The worker-host architecture above is implemented without new npm dependencies.
- All existing playground E2E tests pass.
- The responsiveness, startup-retry, stale-result, local-module, semantic-token, compilation, and decompilation workflows pass.
- The responsiveness test enforces a maximum 250 ms heartbeat gap for the complete quickstart-switch workflow.
- The plan's progress tracker records any deferred cancellation or caching work precisely.

### PR 2: Native UI, accessibility, and responsive layout

#### Scope

1. Replace Bootstrap and React-Bootstrap components with semantic HTML and local CSS.
2. Remove `bootstrap` and `react-bootstrap`.
3. Implement an accessible sample-template picker using native controls and a small React component.
4. Add a responsive CSS Grid workspace:
   - Two columns on sufficiently wide screens.
   - One column with two rows on narrow screens.
   - No horizontal page overflow.
5. Allow toolbar controls to wrap without clipping.
6. Add `lang` and viewport metadata.
7. Add `header`, `nav`, `main`, and named editor sections.
8. Give the editors distinct accessible labels.
9. Add labels for file selection and sample filtering.
10. Announce loading, success, and errors with appropriate live regions.
11. Preserve visible focus indicators and logical keyboard order.
12. Respect reduced-motion and forced-colors preferences.

#### Native sample picker guidance

Prefer a native `<select>` if its search and navigation behavior provides an acceptable workflow. If filtering hundreds of templates requires a custom menu:

- Use a labeled input and listbox pattern.
- Support Arrow keys, Home, End, Enter, Escape, and focus return.
- Keep active-option and selected-option semantics distinct.
- Do not add a combobox or accessibility package solely for this control.

#### Tests

- Keyboard-only toolbar and sample selection.
- Distinct editor names in the accessibility tree.
- Loading and errors are announced.
- No horizontal overflow at 320, 375, 768, and 1280 CSS pixels.
- Both editors remain usable at 200% zoom.
- Focus returns predictably after menu close, error dismissal, and file selection.
- Automated accessibility scan for the initial, loading, menu-open, and error states.

#### Acceptance criteria

- `bootstrap` and `react-bootstrap` are absent from `package.json` and the lockfile.
- The page remains usable from 320 CSS pixels through large desktop widths.
- Core workflows are keyboard accessible.
- The initial page and exercised states have no serious automated accessibility violations.

### PR 3: Shared-link modernization and native compression

#### Scope

1. Replace Pako encoding with:
   - `TextEncoder`
   - `CompressionStream("deflate")`
   - Base64url output safe for URL fragments
2. Replace Pako decoding with:
   - Base64url decoding
   - `DecompressionStream("deflate")`
   - `TextDecoder`
3. Retain decoding compatibility for existing Pako-generated links.
4. Version the new share-link payload so future codecs can coexist.
5. Add compressed and decompressed size limits.
6. Detect malformed or unsupported links and display a recoverable error.
7. Make Copy Link asynchronous and expose progress, success, and failure.
8. Remove `pako` and `@types/pako`.

#### Compatibility approach

Use a small version prefix for new fragments. If the prefix is absent, decode with the legacy byte-to-character behavior using native decompression. This preserves existing links without retaining Pako.

Do not silently treat malformed links as an empty document.

#### Tests

- Existing legacy links continue to open.
- New links round-trip ASCII, accented characters, CJK text, and emoji.
- Malformed Base64, invalid compressed data, and unsupported versions show errors.
- Oversized compressed and expanded payloads are rejected.
- Copy Link works through the plain-text clipboard fallback when `ClipboardItem` is unavailable.

#### Acceptance criteria

- `pako` and `@types/pako` are absent from `package.json` and the lockfile.
- Existing shared links remain supported.
- Unicode source round-trips exactly.
- Invalid or oversized links cannot lock the UI or consume unbounded memory.

### PR 4: Build and bundle optimization

#### Scope

1. Install `wasm-tools` in every workflow that produces deployable playground output.
2. Fail production builds when optimized WASM publishing is unavailable.
3. Import Monaco from its narrow ESM editor API.
4. Include only the JSON contribution and required Bicep registration.
5. Emit only the editor and JSON workers.
6. Replace `vite-plugin-static-copy` with Vite `publicDir` configuration or a Node standard-library copy step.
7. Remove `vite-plugin-static-copy`.
8. Lazy-load Application Insights after URL sanitization and initial rendering if telemetry requirements permit.
9. Pre-sort the generated quickstart list and avoid sorting during every render.
10. Memoize or defer sample filtering and cap unfiltered rendering if measurements justify it.
11. Add build budgets to CI.
12. Update the DOMPurify override and transitive NanoID version to non-vulnerable releases.

#### Initial budgets

Record a fresh optimized baseline in the pull request, then enforce budgets that prevent regression. Initial targets:

- Initial application JavaScript: at least 40% smaller than the current 4.42 MB minified output.
- No TypeScript, CSS, or HTML Monaco worker emitted.
- No unused Monaco language bundles in the initial application chunk.
- Optimized WASM publish confirmed in build logs.
- No high-severity `npm audit` findings.

Budgets should be based on generated files rather than local preview transfer behavior, which varies with server compression.

#### Tests and measurements

- Production build succeeds from a clean checkout.
- The deployed application initializes with compressed assets enabled.
- Bicep syntax coloring, diagnostics, semantic tokens, and JSON editing still work.
- Bundle report contains only expected Monaco workers and language contributions.
- `npm audit --omit=dev` has no unresolved high-severity findings.

#### Acceptance criteria

- Production workflows install and use `wasm-tools`.
- `vite-plugin-static-copy` is absent from the manifest and lockfile.
- Monaco output meets the agreed bundle budget.
- Dependency advisories are resolved or have a documented upstream exception.

### PR 5: Test hardening and optional state decision

This PR is only needed for work that would make the earlier pull requests too broad.

#### Scope

1. Consolidate race, error, accessibility, and responsive tests added by earlier PRs.
2. Add a dependency-count check or documented budget.
3. Add bundle-budget reporting to pull requests.
4. Decide whether the `useReducer` design remains clear.
5. Adopt Jotai only if the decision criteria in this document are met.

#### Acceptance criteria

- Happy paths and negative paths are both covered.
- The final runtime dependency count is four, or five with a documented Jotai justification.
- No redundant state-management abstraction remains.

## Cross-Cutting Implementation Rules

### Error handling

- Do not use `alert()` for application errors.
- Do not turn exceptions into successful-looking editor output.
- Do not silently replace malformed input with an empty document.
- Error messages should identify the failed operation and offer a next action.
- Telemetry should contain error categories and correlation data, not template source.

### Async behavior

- Every async operation that can overlap must define ordering or cancellation behavior.
- Fetches should use `AbortController`.
- Compilation should use model versions or request IDs.
- Component unmount must cancel timers and prevent stale state updates.

### State

- Keep source text as the authoritative user document.
- Treat compiled JSON and diagnostics as derived state tagged with the source revision.
- Keep operation status separate from document state.
- Do not store Monaco instances in serializable application state.
- Do not persist Bicep source outside the URL unless a separate product decision explicitly approves it.

### Accessibility

- Prefer native semantic controls.
- Every control and editor must have a stable accessible name.
- Loading and error messages must not depend on color or motion.
- Focus must remain predictable across asynchronous operations.

### Performance

- Measure before and after each dependency or bundle change.
- Do not add memoization without a measured or structurally clear benefit.
- Do not add virtualization unless sample rendering is shown to be a bottleneck after simpler changes.
- Keep compiler work lazy and avoid duplicate compilations for the same revision.

## Dependency Budget

| Stage                    | Runtime dependencies                                                             | Expected count |
| ------------------------ | -------------------------------------------------------------------------------- | -------------: |
| Current                  | Application Insights, Bootstrap, Monaco, Pako, React, React-Bootstrap, React DOM |              7 |
| After native UI          | Application Insights, Monaco, Pako, React, React DOM                             |              5 |
| After native compression | Application Insights, Monaco, React, React DOM                                   |              4 |
| Optional Jotai outcome   | Application Insights, Jotai, Monaco, React, React DOM                            |              5 |

Any new runtime dependency should:

1. Solve a demonstrated problem that is not reasonably addressed by browser or React APIs.
2. Replace more complexity than it introduces.
3. Have acceptable bundle, maintenance, accessibility, and security characteristics.
4. Be explicitly justified in the pull request.
5. Not duplicate an existing dependency or project utility.

## Recommended Order

1. PR 1: Correctness, privacy, and editor lifecycle.
2. PR 1: Compiler Web Worker and compilation reuse.
3. PR 2: Native UI, accessibility, and responsive layout.
4. PR 3: Shared-link modernization and native compression.
5. PR 4: Build and bundle optimization.
6. PR 5 only if test consolidation or a justified Jotai migration remains.

Correctness and privacy come first. Main-thread compiler isolation follows because it has the largest effect on interaction responsiveness. Dependency removal is then split between UI and share-link work so each change remains reviewable and reversible. Build optimization follows once behavior is protected by tests.

## Definition of Done

The improvement effort is complete when:

- Telemetry cannot receive shared source through the URL.
- Compilation results cannot become stale or cross-contaminate concurrent work.
- Loading and failure paths preserve user content and focus.
- Existing and new shared links round-trip correctly, including Unicode.
- The playground is keyboard accessible and responsive at 320 CSS pixels.
- Bootstrap, React-Bootstrap, Pako, and the static-copy plugin are removed.
- Production WASM is optimized.
- Compiler and decompiler work runs in a Web Worker rather than on the main browser thread.
- JSON, diagnostics, and semantic tokens reuse one compilation per accepted document revision.
- Monaco emits only required code and workers.
- The runtime dependency count is four, or five with a documented Jotai justification.
- Targeted unit and E2E tests cover critical happy paths, negative paths, and races.
