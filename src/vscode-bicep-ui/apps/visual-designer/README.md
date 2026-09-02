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

`ControlBar` and `Palette` consume them through `useCanvasActions`.

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
