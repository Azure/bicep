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

**Undo** and **Redo** sit in their own panel below the control bar. While the designer has focus, Ctrl/Cmd+Z undoes,
Ctrl/Cmd+Shift+Z redoes, and Ctrl+Y also redoes on Windows/Linux. These shortcuts do not intercept
text fields or the source editor. Right-click opens no menu outside text fields: VS Code's default
Cut/Copy/Paste menu does nothing on the graph, so the designer suppresses it.
Creation, node moves, and Reset Layout share one session-local history. Each drag and reset is
one step; pan, zoom, and focus are not. Layout undo/redo animates node positions (or snaps under
reduced motion) without editing Bicep or moving the camera.
Undoing an independent resource creation also preserves surviving node positions and the camera.
A resource creation can be undone only while its declaration is exactly as the designer inserted it
and nothing references it, and redone only while its name is free. The language server checks this
with every graph update, so Undo and Redo are enabled only when they will work; edits elsewhere in
the file do not affect them. Failed actions are logged, not shown as canvas notifications. The
editor's native history remains separate. See
[Undo and redo](./docs/undo-redo.md) for the interaction and conflict policy.

The bottom-center creation dock is gated by the experimental resource-editing setting. Currently it
enables resource creation only: the Resources button opens a compact popover. Unimplemented tools
are not shown. With editing disabled, viewing, local repositioning, focus, pan/zoom, source
navigation, status, export, and layout undo remain available. The extension also rejects creation
and source replay while the setting is off, including requests started before it was turned off. Module
editing does not implicitly use this resource-only setting; it requires its own opt-in decision before
implementation.
Each resource shows its API version as a pill: quiet text at rest, with pill chrome revealed on row
hover or focus, and kept when the selection differs from the host default. Clicking it (or pressing
Arrow keys, Enter, or Space
while it is focused) opens a version list; versions load on first focus or open. Resources are added
only by dragging the icon/type area onto the canvas, which inserts the selected version where it is
dropped; clicking or pressing keys on a resource does not insert. A press becomes a drag only after
the pointer moves 4px, so clicks never flash a drag preview, and Escape during a drag cancels only the drag.
The preview follows pointer movement without rerendering the resource card on every move. Choices
survive browsing, searching, and closing the palette until the provider catalog changes. Escape or
toggling Resources closes the popover and restores focus to the dock. The passive top-left target
scope remains visible independently of creation enablement. The palette offers only resource types
that can be deployed at the opened file's `targetScope`, since resources are inserted there as
top-level declarations; changing the scope refreshes the catalog and resets version choices.

The version list opens immediately and shows progress, or an error with retry, until versions
arrive. It renders in the top layer so the palette's scroll area never clips it; Escape closes only
the list, and scrolling or resizing dismisses it.

Browsing the palette has **Featured**, **Recent**, and **All** views, chosen with tabs below the
search box (arrow keys move between them). Featured lists the common providers, Recent the types
dropped onto the canvas this session, and All every provider alphabetically. The chosen view
survives closing the palette. Search always covers the whole catalog and hides the tabs.

The designer uses curated light, dark, and high-contrast palettes for the VS Code theme kind. The
`bicep.visualizer.matchColorTheme` setting (off by default) colors it from the active color theme
instead, keeping curated colors wherever a theme color is missing or lacks contrast. Export's
"Current" theme captures what is shown. See [Theme](./docs/architecture.md#theme).

Fake-host controls include **Document target scope**, **Change catalog**, and **Match color theme**.
The playground's own theme picker switches between VS Code's built-in color themes. Query parameters:

| Parameter                                | Purpose                                                                               |
| ---------------------------------------- | ------------------------------------------------------------------------------------- |
| `resourceEditing=false`                  | Hide the entire creation dock                                                         |
| `targetScope=tenant`                     | Initial document scope (`resourceGroup`, `subscription`, `managementGroup`, `tenant`) |
| `catalogDelay=5000`                      | Delay the resource type catalog response in milliseconds                              |
| `versionsDelay=1000`                     | Delay per-type API-version responses in milliseconds                                  |
| `versionFailures=1`                      | Fail the first N version requests to exercise retry                                   |
| `motionPolicy=reduce`                    | Use reduced motion for graph layout and designer undo/redo in the fake host           |
| `withholdGraphUpdatesAfterCreation=true` | Return empty graph updates while a designer-created resource is in source             |
| `skipGraphUpdateAfterUndo=true`          | Withhold an undone creation's removal until the graph changes again                   |
| `sourceReplay=unavailable`               | Report every resource creation as no longer replayable, as if edited in the file      |
| `catalogSize=2300`                       | Add N synthetic types shaped like the real Azure catalog, for profiling at scale      |
| `matchColorTheme=true`                   | Start with `bicep.visualizer.matchColorTheme` on                                      |
| `graphDelay=2000`                        | Hold the first graph back, as a language server that is still starting would          |
| `restoreDelay=1000`                      | Delay remote-module restores in milliseconds                                          |
| `restoreFailures=1`                      | Fail the first N remote-module restores to exercise retry                             |
| `restoreRefreshFailures=1`               | Fail N graph fetches after a successful restore to exercise refresh-only retry        |
| `moduleLayout=layoutFailed`              | Answer the first layout of a module on its own with `graphChanged` or `layoutFailed`  |
| `layoutDelay=800`                        | Delay layout responses in milliseconds                                                |
| `createDelay=3000`                       | Delay resource creation in milliseconds, which keeps its placeholder on the canvas    |

The fake catalog includes stable, preview, and preview-only fixtures with per-type deployment scopes,
filtered by the selected document target scope like the language server.

## Architecture

| Area           | Responsibility                                                             |
| -------------- | -------------------------------------------------------------------------- |
| `src/app`      | App-wide store, host environment, synchronization, theme, and composition  |
| `src/core`     | The graph of the Bicep file: host protocol, sync, layout, and undo history |
| `src/features` | Product capabilities and their UI                                          |
| `src/lib`      | Reusable graph and math libraries with no Bicep protocol knowledge         |
| `src/ui`       | Workflow-neutral components, motion tokens, and theme                      |
| `src/devtools` | Development-only fake host and controls                                    |
| `src/utils`    | Small shared helpers that do not belong to a library                       |

Dependency direction is enforced by ESLint:

```text
app       -> features, core, lib, ui, utils, devtools
devtools  -> features, core, lib, ui, utils
features  -> core, lib, ui, utils, other feature barrels
core      -> lib, utils
ui        -> lib, utils
lib       -> lib, utils
utils     -> utils
```

Feature-to-feature imports use public barrels and follow this one-way graph:

```text
dock     -> palette -> canvas -> export
controls ---------------------> export
status   (no feature dependencies)
```

The palette uses canvas drop coordinates and its resource preview; the canvas hosts export overlays;
controls only open export. ESLint checks these edges as well as the layer boundaries.

### Source layout

```text
src/
  app/
    App.tsx
    AppEnvironment.tsx
    GlobalStyle.ts
  core/
    atoms/            state and actions grouped by responsibility (see State modules below)
    components/
    context/
    hooks/
    __tests__/
    api.ts
    graph-layout.ts
    graph-model.ts
    graph-renderer.ts
    graph-update-coordinator.ts
    graph-view-state.ts
    node-positions.ts
    types.ts
    undo-history.ts
  features/
    canvas/
      components/
      hooks/
    controls/
    export/
    palette/
    dock/
    status/
  lib/
    graph/
    math/
  ui/
  utils/
```

`core` and feature folders contain only the surfaces they need:

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

Each feature, library, and `src/core` exposes a public barrel:

- Import other modules through `@/core`, `@/features/*`, `@/lib/*`, `@/ui`, or `@/utils`.
- Theme is a separate public surface at `@/ui/theme`; UI components and motion use `@/ui`.
- Use relative imports within the same module.
- Export only symbols intended for other modules, not every internal atom, hook, or component.
- Devtools uses the same public barrels for host contracts and shared graph calculations.

ESLint rejects private alias subpaths such as `@/core/atoms` or `@/features/export/atoms`.
An internal file can still export symbols to its siblings without making them part of the module's
public API: only the barrel defines that API.

### State modules

Atom files are named for the state they own, not for whether they contain data or actions. A concern
can contain stored state, derived reads, and the actions that preserve its invariants:

| Core module                                            | Owns                                                                                    |
| ------------------------------------------------------ | --------------------------------------------------------------------------------------- |
| [document](./src/core/atoms/document.ts)               | Host document URI, target scope, error count, and whether the first graph arrived       |
| [settings](./src/core/atoms/settings.ts)               | Host settings and derived feature/motion flags                                          |
| [graph](./src/core/atoms/graph.ts)                     | Graph snapshot, canvas/history commit, interaction activity, and Undo/Redo availability |
| [module-restores](./src/core/atoms/module-restores.ts) | Read-only restore progress/failures and request-owned begin/settle/reconcile actions    |
| [pending-changes](./src/core/atoms/pending-changes.ts) | Resource placeholders, expected placement, and removals awaiting a source update        |
| [history](./src/core/atoms/history.ts)                 | Undo/redo stacks, next steps, and host replay confirmations                             |
| [canvas](./src/core/atoms/canvas.ts)                   | The shared canvas element used for drops and export                                     |

Feature-local state stays in its feature: palette catalog choices and drag state, export options,
and derived status-bar intent. Private backing atoms protect graph-view and restore invariants;
consumers read them through read-only atoms and use the owning actions to change them.

### App environment and state

`AppEnvironment` owns the Jotai store, real or fake message channel, theme, and the app-wide host
synchronization from `@/core`: the document, the resource-editing setting, and motion policy. `App` mounts `PanZoomProvider`, then `GraphActionsProvider` inside it, because graph
layout reads the viewport size and fits the camera. `Canvas`, `Controls`, and `Dock` are siblings
inside both.

Use Jotai for shared observable state and local React state for component-local interaction. Prefer
derived and action atoms over exposing writable atoms across feature boundaries.

Use `useAtomValue` for values rendered by React and `useSetAtom` for writes to a known atom.
Use `useStore` when an event or async operation needs current state, accesses dynamically selected
node atoms, or explicitly subscribes to updates for DOM animation. `store.get` is a snapshot read,
not a React subscription; it must not replace `useAtomValue` for changing rendered values.
Multi-atom state transitions belong in action atoms rather than scattered imperative writes.

`GraphActionsProvider` keeps the graph in step with the Bicep file and publishes the actions that
change it through `useGraphActions`:

```ts
interface GraphActions {
  resetGraphLayout(): Promise<void>;
  createResourceAt(resourceType, graphPoint): Promise<void>;
  toggleModuleCollapsed(nodeId): void;
  handleNodeDragStart(nodeId): void;
  handleNodeDragEnd(nodeId): void;
  undo(): Promise<void>;
  redo(): Promise<void>;
}
```

`ControlBar` uses them for Reset Layout, and `HistoryBar` for Undo and Redo. The canvas feature adds only what depends on
its own element and camera: `useCanvasDropTarget` converts a palette drop's client point to a graph
point. It reads the canvas element from core's `canvasElementAtom`, so the palette can use it without
`Canvas` being an ancestor; export reads the same atom to capture the canvas.

## Graph synchronization

`src/core` keeps a client replica of the server graph and requests layout after React has measured
node sizes.

The state flow is **intent → pure view calculation → Jotai commit → measured layout → reveal**.
`graphViewAtom` is the read-only snapshot of the source, visible projection, and collapsed-module
snapshots. Both host updates and module toggles use `commitGraphViewAtom`; it updates that snapshot,
reconciles canvas nodes, and prunes history in one transaction. See
[Reading the graph lifecycle](./docs/architecture.md#reading-the-graph-lifecycle) for the entry points.

Read the part relevant to the change:

| Reviewing                                                 | Start here                                                                                                          |
| --------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| What the user sees after a source update or toggle        | [Pure graph-view rules](./src/core/graph-view-state.ts)                                                             |
| How that view reaches mounted nodes and history           | [Graph commit action](./src/core/atoms/graph.ts)                                                                    |
| Host refreshes, measurement, and scoped layout            | [Graph synchronization](./src/core/hooks/use-graph-sync.ts)                                                         |
| Async ordering, coalescing, retries, and source mutations | [Graph update coordinator](./src/core/graph-update-coordinator.ts)                                                  |
| Restore progress and retry                                | [Restore workflow](./src/core/hooks/use-module-restore.ts) and [restore atoms](./src/core/atoms/module-restores.ts) |
| Collapse/expand animations and deferred placement         | [Module transitions](./src/core/hooks/use-module-expansion.ts)                                                      |

The coordinator reconciles before layout, rejects superseded responses, and serializes source
mutations. Layout retries retain their scope; module layout can fall back to whole-graph layout.
See [Architecture](./docs/architecture.md#client-implementation) for the complete module map and
ordering rules rather than duplicating them here.

## Testing

- Vitest covers atoms, graph model/layout behavior, export state, coordinator ordering, color-theme derivation,
  and the ESLint module/feature boundary rules.
- Playwright covers canvas interaction, resource creation, catalog loading, palette views, search, theme matching, and export.
- E2E assertions should poll animated state rather than sample positions immediately.

Lint runs with zero warnings and rejects unused disable directives.

## Current limitations

- Webview and extension protocol declarations are not generated from a shared schema.
- Long resource lists are not virtualized.

Known issues are listed under [Errors and limitations](./docs/architecture.md#known-issues).

## Further reading

- [Architecture](./docs/architecture.md)
- [Proposed roadmap](./docs/roadmap.md)
- [Project instructions](./.github/instructions/)
