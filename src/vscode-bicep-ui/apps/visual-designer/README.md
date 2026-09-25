# Bicep Visual Designer

The visual designer is a React webview for inspecting and editing a Bicep deployment graph. It
supports pan and zoom, source navigation, graph export, and experimental resource creation.

Production runs inside the `vscode-bicep` extension. Development mode runs in a browser against the
fake host in `src/devtools`.

## Development

Use Node.js 22 or later. Install workspace dependencies from `src/vscode-bicep-ui`:

```bash
npm ci
npm run build
```

Run app commands from `apps/visual-designer`:

```bash
npm run dev
npm run build
npm run lint
npm run test
npm run e2e:install
npm run e2e
```

`npm run dev` loads a fake extension host. E2E tests use query parameters such as `catalogDelay` to
make loading and concurrency states deterministic.

The bottom-center creation dock is gated by the experimental resource-creation setting. Resources
opens a compact popover; Modules and Notes are keyboard-discoverable, disabled coming-soon tools.
Each resource shows its API version as a pill: quiet text at rest, with pill chrome revealed on row
hover or focus, and kept when the selection differs from the host default. Clicking it (or pressing
Arrow keys, Enter, or Space
while it is focused) opens a version list; versions load on first focus or open. Resources are added
only by dragging the icon/type area onto the canvas, which inserts the selected version where it is
dropped; clicking or pressing keys on a resource does not insert. A press becomes a drag only after
the pointer moves 4px, so clicks never flash a drag preview, and Escape during a drag cancels only the drag. Choices
survive browsing, searching, and closing the palette until the provider catalog changes. Escape or
toggling Resources closes the popover and restores focus to the dock. The passive top-left target
scope remains visible independently of creation enablement. The palette offers only resource types
that can be deployed at the opened file's `targetScope`, since resources are inserted there as
top-level declarations; changing the scope refreshes the catalog and resets version choices.

The version list opens immediately and shows progress, or an error with retry, until versions
arrive. It renders in the top layer so the palette's scroll area never clips it; Escape closes only
the list, and scrolling or resizing dismisses it.

Fake-host controls include **Document target scope** and **Change catalog**. Query parameters:

| Parameter                | Purpose                                                                               |
| ------------------------ | ------------------------------------------------------------------------------------- |
| `resourceCreation=false` | Hide the entire creation dock                                                         |
| `targetScope=tenant`     | Initial document scope (`resourceGroup`, `subscription`, `managementGroup`, `tenant`) |
| `catalogDelay=5000`      | Delay type/search responses in milliseconds                                           |
| `versionsDelay=1000`     | Delay per-type API-version responses in milliseconds                                  |
| `versionFailures=1`      | Fail the first N version requests to exercise retry                                   |
| `catalogSize=2300`       | Add N synthetic types shaped like the real Azure catalog, for profiling at scale      |

The fake catalog includes stable, preview, and preview-only fixtures with per-type deployment scopes,
filtered by the selected document target scope like the language server.

## Architecture

| Area           | Responsibility                                                            |
| -------------- | ------------------------------------------------------------------------- |
| `src/app`      | App-wide store, host environment, synchronization, theme, and composition |
| `src/features` | Product capabilities and Bicep-specific state                             |
| `src/hooks`    | Cross-cutting document and motion-policy synchronization                  |
| `src/lib`      | Reusable graph and math libraries with no Bicep protocol knowledge        |
| `src/ui`       | Workflow-neutral components, motion tokens, and theme                     |
| `src/devtools` | Development-only fake host and controls                                   |
| `src/utils`    | Small shared helpers that do not belong to a library                      |

Dependency direction is enforced by ESLint:

```text
app       -> features, hooks, lib, ui, utils, devtools
devtools  -> features, hooks, lib, ui, utils
features  -> hooks, lib, ui, utils, other feature barrels
ui        -> lib, utils
hooks     -> lib, utils
lib       -> lib, utils
utils     -> utils
```

Feature-to-feature imports go through the target feature's `index.ts` and must remain acyclic.

### Source layout

```text
src/
  app/
    App.tsx
    AppEnvironment.tsx
    GlobalStyle.ts
  features/
    canvas/
      components/
      context/
      hooks/
      __tests__/
      api.ts
      atoms.ts
      graph-layout.ts
      graph-model.ts
      graph-update-coordinator.ts
      types.ts
    controls/
    export/
    palette/
    dock/
    status/
  hooks/
  lib/
    graph/
    math/
  ui/
  utils/
```

Feature folders contain only the surfaces they need:

| Surface       | Contents                                               |
| ------------- | ------------------------------------------------------ |
| `components/` | React components                                       |
| `context/`    | Feature-scoped React contexts and consumer hooks       |
| `hooks/`      | Reusable hooks and orchestration                       |
| `api.ts`      | Host message descriptors, payloads, and bound API hook |
| `atoms.ts`    | Feature-owned Jotai state and actions                  |
| `types.ts`    | Shared feature vocabulary                              |
| `__tests__/`  | Unit tests for root-level feature modules              |

Components use PascalCase filenames. Hooks, non-component files, and folders use kebab-case.

### Public boundaries

Each feature, library, and `src/hooks` exposes one barrel:

- Import other modules through `@/features/*`, `@/lib/*`, `@/ui`, `@/hooks`, or `@/utils`.
- Use relative imports within the same module.
- Export only symbols intended for other modules.

### App environment and state

`AppEnvironment` owns the Jotai store, real or fake message channel, document synchronization, motion
policy synchronization, and theme. `PanZoomProvider` remains in `App` because it belongs to the canvas
composition.

Use Jotai for shared observable state and local React state for component-local interaction. Prefer
derived and action atoms over exposing writable atoms across feature boundaries.

`Canvas` publishes these imperative actions through `CanvasActionsContext`:

```ts
interface CanvasActions {
  createResource(resourceType, clientPoint?): Promise<void>;
  canPlaceResourceAt(clientPoint): boolean;
  resetGraphLayout(): Promise<void>;
}
```

`ControlBar` and `Dock` consume them through `useCanvasActions`.

## Canvas reconciliation

The canvas keeps a client replica of the server graph and requests layout after React has measured
node sizes.

| Module                        | Responsibility                                                          |
| ----------------------------- | ----------------------------------------------------------------------- |
| `graph-model.ts`              | Client graph, patch application, measured projection, render comparison |
| `graph-layout.ts`             | Layout invalidation, response extraction, and viewport centering        |
| `graph-update-coordinator.ts` | Update/layout ordering, coalescing, and mutation serialization          |
| `use-canvas-controller.ts`    | API, model, coordinator, placement, and Jotai integration               |
| `use-apply-graph.ts`          | Reconcile graph nodes and edges                                         |
| `use-apply-graph-layout.ts`   | Reveal and animate server-computed positions                            |

The coordinator enforces these rules:

- Reconcile before layout.
- A reset layout takes precedence over automatic layout.
- A `graphChanged` layout response schedules reconciliation and retries the same layout mode.
- Source mutations run serially.
- An update response that overlaps a mutation is discarded and fetched again.
- A response superseded by a document notification is discarded, including its target-scope metadata.
- Request promises settle when all currently pending work has completed.

See [Architecture](./docs/architecture.md) for graph synchronization, layout, and resource creation.

## Testing

- Vitest covers atoms, graph model/layout behavior, export state, and coordinator ordering.
- Playwright covers canvas interaction, resource creation, catalog loading, search, and export.
- E2E assertions should poll animated state rather than sample positions immediately.

Lint runs with zero warnings and rejects unused disable directives.

## Current limitations

- Graph update and layout responses share one `GraphPatch` union.
- Resource-creation failure UI is not covered by the fake-host E2E suite.
- Webview and extension protocol declarations are not generated from a shared schema.
- Long resource lists are not virtualized.

## Further reading

- [Architecture](./docs/architecture.md)
- [Project instructions](./.github/instructions/)
