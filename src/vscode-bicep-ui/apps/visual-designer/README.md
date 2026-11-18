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
text fields or the source editor. VS Code's default Cut/Copy/Paste menu does nothing on the graph,
so the designer suppresses it outside text fields, and offers a node-specific menu instead.
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

The bottom-center dock holds the Select and Hand tools and, when the experimental resource-editing
setting is on, the Resources tool, whose button opens a compact popover. The setting controls only
the Resources tool; everything else, including selection, works with it off. Unimplemented tools are
not shown. The extension also rejects creation and source replay while the setting is off, including
requests started before it was turned off. Module
editing does not implicitly use this resource-only setting; it requires its own opt-in decision before
implementation.

**Select (V)** and **Hand (H)** start the dock. With
Select, a click selects one node, Shift/Ctrl/Cmd+click adds or removes a node, and dragging on the
background draws a marquee that selects the nodes it touches (a container only when enclosed, and
then with everything inside it); Shift+marquee adds to the selection. Alt/Option+drag, or
Ctrl/Cmd+drag, draws a marquee even from a node, such as inside a module's frame, to select some of
the module's nodes. Dragging a node moves it, or the whole selection when it is one of several
selected nodes, as one undo step. With Hand, a left drag pans from anywhere and never selects or
moves nodes; holding Space switches to Hand until it is released, so trackpads can pan without
changing tools. A right drag pans in either tool. The designer draws its own cursors, colored like
the dock's icons (the icon color outlining the dock's surface color): the Codicon `cursor` arrow as
the default and for Select, a pointing hand over buttons and other clickable controls, and an
outlined open hand for Hand and palette drag handles that closes while panning or dragging. Text
fields keep the text cursor. Select shows no grab indicator when a node is pressed, and the active
tool is shown without an accent color. Shortcuts ignore text fields,
composition, and modifier keys, and Space keeps its normal behavior on buttons and other controls.
Selection is independent of focus and paint order. Clicking blank canvas or pressing Escape clears it.
A right click on a node opens its context menu with **Go to Source** when it leaves exactly one node
selected; there is no menu while several nodes are selected. Shift+F10 or the Context Menu key opens
the menu for a single selected node; Escape dismisses it and returns focus to the canvas without
clearing selection. Double-click still opens source in Select until inline rename is implemented;
Hand does not reveal source.

Each resource shows its API version as a pill: quiet text at rest, with pill chrome revealed on row
hover or focus, and kept when the selection differs from the host default. Clicking it (or pressing
Arrow keys, Enter, or Space
while it is focused) opens a version list; versions load on first focus or open. Resources are added
only by dragging the icon/type area onto the canvas, which inserts the selected version where it is
dropped; clicking or pressing keys on a resource does not insert. A press becomes a drag only after
the pointer moves 4px, so clicks never flash a drag preview, and Escape during a drag cancels only the drag.
The preview follows pointer movement without rerendering the resource node on every move. Choices
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

A floating **dev panel** at the top left stands in for the extension host. Its **Host** section sets
the document target scope, toggles **Resource editing** and **Match color theme**, picks the motion
policy, and replaces the resource catalog; **Graphs** loads the sample graphs, marking the current one;
and **Mutations** changes the current graph (nodes, modules, and edges). The panel collapses to a small
**DEV** tab, and remembers that. The playground's own theme picker switches between VS Code's
built-in color themes. Query parameters set the initial state, and simulate host behavior the panel
does not cover:

| Parameter                                | Purpose                                                                               |
| ---------------------------------------- | ------------------------------------------------------------------------------------- |
| `resourceEditing=false`                  | Start with resource editing off, which hides the Resources tool                       |
| `targetScope=tenant`                     | Initial document scope (`resourceGroup`, `subscription`, `managementGroup`, `tenant`) |
| `catalogDelay=5000`                      | Delay the resource type catalog response in milliseconds                              |
| `versionsDelay=1000`                     | Delay per-type API-version responses in milliseconds                                  |
| `versionFailures=1`                      | Fail the first N version requests to exercise retry                                   |
| `layoutDelay=1500`                       | Delay layout responses in milliseconds, holding layouts and Reset Layout open         |
| `motionPolicy=reduce`                    | Use reduced motion for graph layout and designer undo/redo in the fake host           |
| `withholdGraphUpdatesAfterCreation=true` | Return empty graph updates while a designer-created resource is in source             |
| `skipGraphUpdateAfterUndo=true`          | Withhold an undone creation's removal until the graph changes again                   |
| `sourceReplay=unavailable`               | Report every resource creation as no longer replayable, as if edited in the file      |
| `catalogSize=2300`                       | Add N synthetic types shaped like the real Azure catalog, for profiling at scale      |
| `matchColorTheme=true`                   | Start with `bicep.visualizer.matchColorTheme` on                                      |

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

Feature-to-feature imports go through the target feature's `index.ts` and must remain acyclic.

### Source layout

```text
src/
  app/
    App.tsx
    AppEnvironment.tsx
    GlobalStyle.ts
  core/
    atoms/            document, graph, history, pending-changes, settings
    components/
    context/
    hooks/
    __tests__/
    api.ts
    graph-layout.ts
    graph-model.ts
    graph-update-coordinator.ts
    node-positions.ts
    types.ts
    undo-history.ts
  features/
    canvas/
      components/
      hooks/
      atoms.ts
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

Each feature, library, and `src/core` exposes one barrel:

- Import other modules through `@/core`, `@/features/*`, `@/lib/*`, `@/ui`, or `@/utils`.
- Use relative imports within the same module.
- Export only symbols intended for other modules.

### App environment and state

`AppEnvironment` owns the Jotai store, real or fake message channel, theme, and the app-wide host
synchronization from `@/core`: the document, the resource-editing setting, and motion policy. `App` mounts `PanZoomProvider`, then `GraphActionsProvider` inside it, because graph
layout reads the viewport size and frames a newly opened graph. `Canvas`, `Controls`, and `Dock` are siblings
inside both.

Use Jotai for shared observable state and local React state for component-local interaction. Prefer
derived and action atoms over exposing writable atoms across feature boundaries.

`GraphActionsProvider` keeps the graph in step with the Bicep file and publishes the actions that
change it through `useGraphActions`:

```ts
interface GraphActions {
  resetGraphLayout(): Promise<void>;
  createResourceAt(resourceType, graphPoint): Promise<void>;
  toggleModuleCollapsed(moduleId): void;
  handleNodeDragStart(nodeId): void;
  handleNodeDrag(nodeId, dx, dy): void;
  handleNodeDragEnd(nodeId): void;
  undo(): Promise<void>;
  redo(): Promise<void>;
}
```

`ControlBar` uses them for Reset Layout, `HistoryBar` for Undo and Redo, and `Canvas` for node drags
and module collapse. The canvas feature adds only what depends on
its own element and camera: `useCanvasDropTarget` converts a palette drop's client point to a graph
point. It reads the canvas element from core's `canvasElementAtom`, so the palette can use it without
`Canvas` being an ancestor; export reads the same atom to capture the canvas.

`lib/graph` treats node IDs as opaque and knows nothing about Bicep, focus, or modules. Containment
comes from each node's `parentId`. `Graph` stacks nodes and edges as nested elements would be: a
parent, then the edges between its children, then each child, so edges are below the nodes they join
and above their parent's frame. It raises the nodes in `raisedNodeIdsAtom` above their siblings, the most
recently raised on top; the canvas passes core's, which holds the nodes focused, dragged, or opened, so
dragging raises a node without focusing it. A node that sits on a sibling container (their target boxes overlap) has its edges
drawn just above that container and below the nodes they join; only that node and the siblings it shares an edge
with are lifted above the container. It reports
gestures for the app to act on: `onNodeDragStart`, `onNodeDrag`, `onNodeDragEnd`, and
`onNodeDoubleClick` on `Graph`, and `onNodeClick` on `Viewport`. Node content marks its own controls
with `graphInteractiveProps`, so pressing one neither drags or focuses the node nor pans the canvas.

The app gives `lib/graph` a graph to show (`setGraphAtom`: nodes with a parent and data, and edges)
and where its nodes go (`nodePositionsAtom`: node centers and whether to animate there). The library
measures the nodes and owns how they are drawn on the way: springs to new positions, containers that
follow their children, morphs when a node becomes a container or stops being one, and fades for the
descendants of a container that opens or closes. A node keeps its state, and its element, while its ID
and parent stay the same.

## Graph synchronization

`src/core` keeps a client replica of the server graph and requests layout after React has measured
node sizes.

| Module                        | Responsibility                                                        |
| ----------------------------- | --------------------------------------------------------------------- |
| `graph-model.ts`              | Indexed client graph, collapse projection, render comparison          |
| `graph-layout.ts`             | Layout invalidation, viewport centering, and layout centers           |
| `node-positions.ts`           | The node positions map, including hidden nodes, and its pure rules    |
| `show-graph.ts`               | Hands the shown graph to `lib/graph`, placing new nodes provisionally |
| `graph-update-coordinator.ts` | Update/layout ordering, coalescing, and mutation serialization        |
| `undo-history.ts`             | Pure undo/redo stacks of layout and source steps                      |
| `GraphActionsProvider.tsx`    | Owns the coordinator, composes the hooks, and provides `GraphActions` |
| `use-graph-sync.ts`           | Graph updates, layout, and Reset Layout                               |
| `use-node-drag.ts`            | Moving dragged nodes and recording each drag                          |
| `use-module-collapse.ts`      | Collapse and expand                                                   |
| `use-resource-creation.ts`    | Placeholder, extension insertion, and creation history step           |
| `use-undo-redo.ts`            | Undo/redo of layout and resource-creation steps                       |

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

- Vitest covers atoms, graph model/layout behavior, export state, coordinator ordering, and color-theme derivation.
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
