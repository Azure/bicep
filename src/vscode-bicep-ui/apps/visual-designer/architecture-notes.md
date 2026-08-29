# Visual Designer Architecture

This describes how the app is structured and why. The code currently matches it; where the two
disagree, the code is wrong.

Apply it incrementally when changing related areas. Do not treat it as a mandate for one large rewrite.

## Layers

| Layer      | Path            | Contains                                                                         |
| ---------- | --------------- | -------------------------------------------------------------------------------- |
| `app`      | `src/app/`      | Composition root: provider stack, wiring, global style. No product logic.        |
| `features` | `src/features/` | User-facing capabilities. Owns product state and Bicep vocabulary.               |
| `ui`       | `src/ui/`       | Workflow-neutral visual primitives and theme. Knows nothing about Bicep.         |
| `lib`      | `src/lib/`      | Workflow-neutral infrastructure: headless graph engine, transport, policy, math. |

Allowed dependency directions:

```text
app       -> features, ui, lib
features  -> ui, lib, other features (barrel only, acyclic)
ui        -> lib
lib       -> lib
```

Forbidden:

- `lib -> features`, `lib -> ui`, `lib -> app`
- `ui -> features`, `ui -> app`
- cycles between features

Feature-to-feature imports are permitted but discouraged. They must go through the target feature's
`index.ts`, and they must import a component, a derived atom, or an action atom — never a raw writable
atom that both features write. When two features need the same visual element, move the element to
`ui/` rather than importing across the boundary.

The distinction between `lib` and `features` is **not** "logic vs. UI". It is "would this still make
sense in an app that had nothing to do with Bicep?" A headless graph engine and a motion-preference
policy would. A pending-resource reconciler would not.

## Naming

These rules are enforced by review, not tooling. Where a rule and the code disagree, the code is wrong.

- **Components**: PascalCase; the filename equals the exported component name. One primary component
  per file. A file named for a plural or a category must be split, which is why `ControlPrimitives.tsx`
  became `ui/Surface.tsx` and `ui/IconButton.tsx`.
- **Non-components** (hooks, atoms, utils, types): kebab-case.
- **Folders**: kebab-case.
- **Prefer one-word folders under `features/` and `lib/`**, and treat a compound name as a prompt to
  check whether the folder is doing two jobs or borrowing a qualifier to dodge a collision. The
  features `palette`, `controls`, `export`, `status` and `devtools` pass, as do the modules `graph`,
  `messaging`, `accessibility` and `utils`. `resource-palette` did not earn its prefix — there is one
  palette — so it is `palette`, and its components dropped the matching prefix with it.
- **Accuracy in the shared layer outranks brevity in the feature layer.** `features/deployment-graph`
  keeps its qualifier because the one-word alternative would require renaming `lib/graph`, and `graph`
  is precisely what that module is about. The qualifier is also not a dodge: "deployment graph" is the
  Bicep product's own name for this concept, not a webview coinage. It is the JSON-RPC method
  `bicep/getDeploymentGraph` in `Bicep.Cli/Rpc/ICliJsonRpcProtocol.cs` and it appears in the public API
  surface of `Azure.Bicep.RpcClient`. A two-word name that names one real concept is fine; a two-word
  name invented to avoid a collision is the smell.
- Name a feature for the capability it delivers, not the surface it draws on. `features/canvas` would
  fail twice over: it describes the substrate rather than the capability, and it collides with the
  `Canvas` component that `lib/graph` exports. `visualization`, `visualizer` and `designer` are
  likewise unavailable — they name the entire app (`bicep.visualizer`, "Open Bicep Visualizer", "Bicep
  visual designer"), so using one for a single feature would imply the others are outside it.
- Do not prefix a file with the folder that already contains it: inside `palette/`, a file named
  `resource-palette-utils.ts` should just be `utils.ts`, and `use-palette-drag.ts` becomes
  `use-drag.ts`. Components follow the same rule but keep whatever their exported name needs to stand
  alone at the call site: `ResourcePalette.tsx` becomes `Palette.tsx` because `<Palette>` still reads,
  while `components/nodes/NodeContentProvider.tsx` keeps its `Node` because `<ContentProvider>` would
  be meaningless where it is mounted.
- Put shared domain vocabulary in the feature's `types.ts`, alongside `atoms.ts` at the feature root.
  Component props stay in the file that declares the component, because they are already colocated
  with the only code that owns them — `PaletteProps` lives in `Palette.tsx`, not `types.ts`. A single
  type used in one place does not need a home of its own: `ExportBackgroundMode` sits beside the export
  atoms that consume it.
- Name a thing for what it is in the domain, not for its visual container or its layer. Prefer
  `ResourceNodePreview` over `ResourcePreviewCard`: it is the preview of a resource node, and "card"
  describes a border radius. Drop meaningless qualifiers such as the `Visual` in
  `VisualResourceTypeReference` and `useApplyVisualGraph`.
- `lib/graph` owns the generic node containers (`BaseNode`, `AtomicNode`, `CompoundNode`).
  `features/deployment-graph/components/nodes/` owns the Bicep content rendered inside them. Both may
  use "Node"; the folder disambiguates. Prefer `ResourceNode` / `ModuleNode` over `ResourceDeclaration`
  / `ModuleDeclaration`, because these render graph nodes, not source declarations.

## Folder structure inside a feature

**Every feature has the same shape.** A reader who opens one feature should be able to guess where
things are in any other, so features and `lib` modules organise their contents the same way:

| Folder        | Holds                                                                     |
| ------------- | ------------------------------------------------------------------------- |
| `components/` | Components, including any that are only used inside the feature.          |
| `hooks/`      | Reusable `use-*` hooks.                                                   |
| `utils/`      | Pure helpers with no React dependency.                                    |
| `api.ts`      | The host protocol this feature uses: method constants and payload shapes. |
| `atoms.ts`    | Feature state. Splits into `atoms/` only when it holds distinct concerns. |
| `types.ts`    | Shared domain vocabulary.                                                 |

Files that belong to the feature as a whole stay at its root next to `index.ts`, alongside
concept-named files such as `animations.ts` for shared transition constants. Only include the folders
and files a feature actually needs, and add one when its first member arrives rather than scaffolding
it empty.

This is deliberately uniform rather than minimal. A `components/` folder holding one file carries no
information on its own, and grouping by file type does split some cohesive subsystems: the graph sync
pipeline now spans `hooks/use-graph-update.ts` and `utils/layout-invalidation.ts`. The trade is
accepted because predictability across features is worth more than locally optimal grouping, and
because `lib/graph` already used `atoms/`, `components/`, `hooks/` — so the alternative was not "no
type folders" but "type folders in `lib`, flat features", which is the worse kind of inconsistency.

Concept subfolders are still allowed **inside** a type folder when they name a real seam.
`components/nodes/` is the interchangeable content plugged into `lib/graph`'s node containers through
`renderContent`; those four files share a preview and a commit transition, and the grouping survives
because it says something the surrounding folder does not.

Colocate tests in a `__tests__/` folder beside the code they cover, at whatever depth that code lives.

## Shape

Abbreviated: folders whose contents are unremarkable are shown by name only.

```text
src/
  app/                             # composition root
    App.tsx
    AppProviders.tsx
    GlobalStyle.ts

  features/
    deployment-graph/              # the Bicep deployment graph surface
      components/
        DeploymentGraphView.tsx    # canvas subtree, update loop, client-coordinate contract
        PendingResourceLayer.tsx
        ResourceCreationError.tsx
        nodes/                     # content rendered inside lib/graph node containers
          NodeContentProvider.tsx  # registers the renderers below
          ResourceNode.tsx
          ModuleNode.tsx
          ResourceNodePreview.tsx
      hooks/
        use-graph-update.ts        # the update state machine
        use-apply-graph.ts
      utils/
        layout-invalidation.ts
        graph-equality.ts
      api.ts                       # graph, layout and resource-creation messages
      animations.ts
      atoms.ts

    palette/
      components/                  # Palette, PaletteContent, PaletteControls,
                                   # PaletteDragOverlay, ResourceTypeGroups
      hooks/                       # use-drag, use-resource-type-catalog,
                                   # use-resource-type-search,
                                   # use-resource-creation-enablement
      api.ts                       # enablement and resource-type catalog messages
      atoms.ts
      types.ts                     # resource-type vocabulary

    controls/                      # components/ControlBar, hooks/use-reset-layout, atoms.ts
    export/                        # components/, utils/capture-element.ts, atoms.ts
    status/                        # components/StatusBar, atoms.ts
    devtools/                      # components/, hooks/, fakes/

  lib/
    accessibility/                 # motion policy state and host synchronisation
    graph/                         # atoms/, components/, hooks/, viewport.ts
    host.ts                        # webview lifecycle: ready, document-changed
    utils/                         # math/, text.ts, errors.ts

  ui/                              # shared primitives, flat
    IconButton.tsx
    Surface.tsx
    MotionAwareProgressBar.tsx
    theme/
```

Create folders as their contents move. Do not scaffold empty directories.

## Ownership

### `features/deployment-graph`

Owns everything Bicep-specific about the graph surface:

- resource and module node presentation, and the mapping from generic node kind to that presentation;
- the notify-then-request graph update state machine, including single-in-flight and dirty-flag
  convergence;
- patch application to the `lib/graph` atoms, layout centering, and layout invalidation;
- pending resource placement, preview rendering, and reconciliation to canonical nodes;
- the per-node creation transition and the creation error surface;
- fit-view and reset-layout behavior.

It exposes a narrow contract to other features, stated in **client coordinates**:

```ts
createResource(resourceType: ResourceTypeReference, clientPoint?: Point): Promise<void>
canPlaceAt(clientPoint: Point): boolean
```

Omitting `clientPoint` means "use the feature's default placement", which is how keyboard activation
should create a resource. `features/deployment-graph` owns what that default is; today the viewport-center rule
lives in `App.tsx` instead.

Client coordinates are the boundary on purpose. Converting a pointer position into a graph position
needs the canvas rect and the pan/zoom transform, both of which are graph knowledge; handing those to a
caller would push the geometry into whichever feature happened to ask. `canPlaceAt` covers the one
question the palette legitimately has: whether a pointer release landed on the graph surface at all,
which decides between creating and silently cancelling a drag.

`useGraphUpdate` is a single-instance state machine: it holds the client-side mirror of the server's
canonical graph, and a second instance would diverge from the first and corrupt patch application.
Instantiate it in `DeploymentGraphView` and pass its actions down as explicit props. Do not expose it
as a free `useCanvasActions()`-style hook that any component may call, because the plain-hook form of
that API silently creates a second state machine. If a context-backed accessor is ever needed, it must
wrap one provider-held instance.

This feature publishes user-facing status through a `features/status` action atom rather than writing
`errorCountAtom` and `hasNodesAtom` directly, so `features/status` keeps sole ownership of how status
is derived.

`PendingResourceLayer` positions a `ResourceNodePreview` per pending operation. `PaletteDragOverlay`
renders the same component, so `ResourceNodePreview` is the single definition of what an
about-to-exist resource looks like.

### `features/palette`

Owns resource discovery and selection: feature enablement, namespace and resource-type loading,
search, palette interaction state, and pointer/keyboard initiation. It does not own graph patches,
pending-node reconciliation, placement math, or source edits.

It works in client coordinates throughout. It should not import `viewportToGraphPoint`, hold a canvas
DOM handle, or read the pan/zoom transform; it hands `features/deployment-graph` a client point and lets it
decide where that lands in the graph.

It owns `ResourceTypeReference` in its `types.ts`, and `deployment-graph` imports it through the
barrel to type the creation request — the graph creates what the palette selected. Catalog vocabulary
lives in the same file, and `PaletteProps` sits in `Palette.tsx` beside the component that takes it.

### Other features

- `features/controls`: toolbar composition and action availability. Graph, export, and status commands
  are provided by their owning features; the control bar only arranges them.
- `features/export`: export state, preview, capture, and output options.
- `features/status`: user-facing graph and diagnostic status.
- `features/devtools`: development-only controls, fake data, and the fake message channel.

## Shared infrastructure

### `lib/graph`

A headless, Bicep-agnostic graph engine, and it must stay one. It owns the canonical node and edge
atoms, boxes, bounds and focus; the generic node containers; `Canvas`, `Graph`, `CanvasBackground`,
`EdgeLayer`, `EdgeMarkerDefs` and `StraightEdge`; and the fit-view, drag and measurement hooks.

Keep the name. `graph` is what this module is actually about — nodes, edges and layout — and renaming
it to free the word for a feature would trade accuracy in the shared layer for brevity in one folder
name. The Bicep feature carries the qualifier instead, which is also where the qualifier is true.

Bicep enters only through `nodeConfigAtom`, which is dependency injection working as intended. It
carries two things: `renderContent`, which maps a generic node kind to Bicep node content, and
`onNodeActivate`, which decides what a double-click means. `NodeContentProvider` fills both, hydrating
the config during render rather than in an effect — the default `renderContent` throws, so an effect
would be too late if a node mounted in the first pass. Scoping the write to the store from context also
keeps it out of module scope, where an earlier version ran at import time against the default store and
could not be undone or scoped to a test store.

`onNodeActivate` exists because the engine had in fact grown host knowledge: `AtomicNode` and
`CompoundNode` each carried an identical double-click handler that cast node data to
`{ range, filePath }` and sent `revealFileRange` / `revealNodeSource` notifications directly. The
layer rule could not catch it, because `lib/graph -> lib/messaging` was a legal `lib -> lib` edge. A
second, narrower lint zone now forbids `lib/graph` from importing any messaging module at all, so the
claim at the top of this section is machine-checked rather than aspirational.

`viewportToGraphPoint` belongs here, not in the palette. It converts client coordinates using the
pan/zoom transform, which is engine knowledge the palette merely consumes.

### `lib/accessibility`

Owns cross-cutting accessibility policy: the effective motion preference and its synchronization with
VS Code settings. This is policy infrastructure with no user-facing surface of its own, so it is `lib`
rather than a feature. Placing it in `lib` is also what lets `ui/MotionAwareProgressBar` read it
without a `ui -> features` violation.

Component-specific keyboard and ARIA behavior stays with the component.

### Protocol declarations

Each feature declares the host protocol it uses in its own `api.ts`. Measured across the app, this is
what the code already wanted: of the symbols in the former shared `messages.ts`, all but two belonged
to exactly one feature. `deployment-graph` owns the graph update, layout and resource-creation
messages; `palette` owns enablement and the resource-type catalog; `status` owns the problems-panel
notification; `lib/accessibility` owns motion policy.

`lib/host.ts` holds what no feature owns: `ready` (the webview mounted) and `documentDidChange` (a
broadcast that several features independently react to).

Each `api.ts` has three layers:

|                                     | Example                   |
| ----------------------------------- | ------------------------- |
| Descriptor, named for the operation | `createResource`          |
| Outgoing payload, suffixed `Params` | `CreateResourceParams`    |
| Incoming payload, suffixed `Result` | `CreateResourceResult`    |
| Method on the feature's API hook    | `api.createResource(...)` |

A descriptor and the API method that sends it may share a name, and often should — they are the same
operation named at two levels, and `channel.request(createResource, params)` supplies the verb from
context. This is safe rather than merely tolerable: an object property is not a binding, so
`createResource: (params) => channel.request(createResource, params)` resolves the argument to the
module-level descriptor exactly as intended.

Shadowing would only occur where a file both imports a descriptor and binds that name locally, and the
API hooks removed that possibility: components call `api.revealNodeSource(id)` and no longer import
descriptors at all. Only `api.ts` and the fake host reference them, plus the subscription sites that
must pass one to `useNotification`.

`Params`/`Result` replace an earlier mix of `Payload`, `Request` and `Params` for the same idea.
Domain vocabulary keeps its own name: `loadResourceTypeCatalog` resolves to `ResourceTypeCatalog`, not
a `...Result` envelope, because the catalog is a shared type in `types.ts` rather than a shape that
exists only to be a response.

Notification names follow direction rather than a single tense. `documentDidChange` is an event the
host announces; `revealNodeSource` is a command the webview sends. That distinction is worth keeping.

The split also removes a name collision. `@vscode-bicep-ui/messaging` is the transport — the channel
and its hooks — and an app-local module called `messaging` alongside it invited confusion about which
was which. The package owns _how_ to talk; each feature owns _what it says_; `lib/host` owns the
lifecycle in between.

A feature's `api.ts` is exported through its barrel, because the protocol is part of its public
contract. `features/devtools` is the one legitimate cross-feature consumer: it fakes the entire host,
so it must implement every feature's messages.

Descriptors are partly redundant for requests, and that is accepted. A request reached only through
its API hook could just as well inline the method string there, since the hook's own signature already
states the params and result. They are kept because two consumers cannot go through the hook:
subscriptions, which are declarative and lifecycle-bound
(`useNotification(documentDidChangeMessage, handler)`), and the fake host, which matches nine incoming
methods against `descriptor.method`. Having one way to declare every message is worth a line per
message over having two.

### `ui`

Workflow-neutral visual primitives with more than one consumer, plus theme. Keep components directly
under `ui/` while the set is small.

Do not introduce `ui/primitives/`. It fails the same test as `components/`, and more sharply: `ui` is
_defined_ as workflow-neutral primitives, so the folder restates the layer's own name and partitions
nothing. It also never becomes correct with growth — at fifteen components the useful split is
`forms/`, `overlays/` or `menus/`, by concept, and every one of those is still a primitive.

The resulting asymmetry between loose `.tsx` files and a `theme/` folder is intentional and
informative: `theme/` is a cohesive non-component subsystem, and the loose files are components. That
distinction is real, so the shape reflects it. Symmetry is not itself a goal.

`ui/theme` owns theme tokens, theme objects, the styled-components module augmentation, and VS Code
theme synchronization. This makes `ui` stateful, which is allowed: theme is read by `app`, `features`
and `ui` alike, and depends on nothing above it.

`Surface` and `IconButton` are deliberately named for what they are rather than where they came from.
As `ControlSurface` and `ControlButton` in `features/controls` they were already being used by the
palette launcher, so the "Control" prefix pointed at a layer they did not belong to.

Their theme tokens have not followed yet: both still read `theme.controlBar.*`, so the palette launcher
styles itself from control-bar tokens, and `Palette` hand-rolls a second floating-panel style from
raw `var(--vscode-*)` values at a different radius. Unifying those on one `Surface` with neutrally
named tokens is the kind of duplication `ui/` exists to remove.

Semantic cards, palette rows, status messages and export panels stay with their features.

### `lib/utils`

Generic and dependency-free: `math/`, `text.ts`, and `errors.ts`. Bicep-shaped helpers such as
deployment-graph equality belong to `features/deployment-graph`.

## State ownership

| State                                                     | Owner                       |
| --------------------------------------------------------- | --------------------------- |
| Canonical graph nodes, edges, boxes, bounds, focus        | `lib/graph`                 |
| Node content renderer registration                        | `features/deployment-graph` |
| Pending resource placement and canonical-node correlation | `features/deployment-graph` |
| Per-node creation transition                              | `features/deployment-graph` |
| Palette interaction and resource catalog state            | `features/palette`          |
| Export workflow state                                     | `features/export`           |
| User-facing graph status                                  | `features/status`           |
| Effective motion policy                                   | `lib/accessibility`         |
| Active theme                                              | `ui/theme`                  |

Use Jotai for shared state that benefits from isolated subscriptions. Keep transient state local when
it has one owner. Across a boundary, expose derived values and action atoms rather than raw writable
atoms.

Start with `atoms.ts`. Split into `atoms/` with an `index.ts` re-export only once it holds distinct
state concerns, and split it along the same concepts as the surrounding folders rather than into one
file per atom.

## Public surface

Each feature and each `lib` module exposes exactly one entry point: its `index.ts`.

- Import through the barrel: `@/lib/graph`, `@/features/export`.
- Do not deep-import across a boundary. A feature's `api.ts` is reached through its barrel, and
  `@/lib/utils/math/geometry` is `@/lib/utils`.
- Deep imports within the same module are fine.
- Feature barrels export the intended surface, not `export *` over every file. The `lib` and `ui`
  barrels keep `export *`: they are broad shared surfaces with many legitimate consumers, so
  enumerating them would be churn without a boundary benefit.

## Enforcement

Structure rules that are not machine-checked decay. `src/vscode-bicep-ui/eslint.config.mjs` carries an
import-boundary rule scoped to this app, built on the core `no-restricted-imports` rule with flat-config
`files` zones, so it needs no extra plugin:

- `src/lib/**` may not import `src/features/**`, `src/ui/**`, `src/app/**`
- `src/ui/**` may not import `src/features/**`, `src/app/**`
- `src/features/**` may not import `src/app/**`
- `src/lib/graph/**` may not import any messaging module

The last one is not a layer rule. It exists because the layer rules alone permit `lib -> lib`, which
let the graph engine acquire host-protocol knowledge unnoticed. When a module's value depends on it
_not_ knowing something, say so explicitly rather than trusting the layer diagram to imply it.

The rules are registered at `error`. The lint script also runs with `--max-warnings 0` and
`--report-unused-disable-directives`, so neither a warning nor a stale suppression can accumulate.

There was no boundary rule at all before this, which is why two `lib -> features` imports were able to
land.

## Deliberate non-moves

- **`StraightEdge` stays in `lib/graph`.** It computes a segment between two box centers and reads a
  theme token. It has no Bicep knowledge, so moving it into a feature would relocate generic code into
  the product layer and invert the dependency rule.
- **`Canvas` and `CanvasBackground` stay in `lib/graph`.** They are generic pan/zoom surfaces.
  `DeploymentGraphView` is the Bicep composition that mounts them, and it is named for the capability
  rather than the surface precisely so the two do not blur together.
- **Generic geometry stays in `lib/utils/math`.** `Point`, `Box` and box-segment intersection are
  ordinary math with several consumers. Only transform-aware conversion lives in `lib/graph`.
- **`useResetLayout` stays beside `ControlBar`.** It is a generic in-flight dedupe wrapper with no
  graph knowledge and a single consumer, so it is neither graph-sync behaviour nor shared
  infrastructure.
- **Graph actions stay explicit props.** They are drilled exactly one level, from `DeploymentGraphView`
  to `ControlBar` and `Palette`. Props keep the dependency visible and testable, and a
  free-standing `useCanvasActions()` hook would invite a second `useGraphUpdate` instance, which is a
  correctness bug rather than a style preference.

## Known tensions

Places where the structure is a considered compromise rather than an ideal, recorded so they are not
rediscovered as bugs.

**`features/deployment-graph` is much larger than any other feature.** It holds around eighteen files
while the rest hold three to ten, and it owns presentation, host sync, mutation and placement. The
cleaner decomposition would add a fourth layer between `lib` and `features` for logic that is
Bicep-aware but headless — the sync pipeline, protocol mapping and graph equality — leaving `features`
strictly user-facing. That is rejected here only on size: a four-layer model for a sixty-file app costs
more in indirection and ceremony than it returns. If the sync pipeline keeps growing, promoting
`hooks/use-graph-update.ts` and `utils/` into a `src/domain/` layer is the intended next step rather
than a reversal.

**`ui/MotionAwareProgressBar` reads global state.** A primitive that reaches into an atom is not
really a primitive. The purer shape is a `ProgressBar` taking an `animated` prop, with callers reading
the motion policy. With a single consumer today the wrapper is a reasonable convenience, but a second
consumer with different needs should trigger the split rather than another variant.

**`features/status` is thin** — three files behind one status bar. It stays separate because
`features/controls` derives action availability from it and merging the two would create the
bidirectional coupling the layering rules exist to prevent.

## Tests

Run `npm run build`, `npm run lint` and `npm test` for any change; run `npm run e2e` when touching
graph updates, placement, or app composition.

Most behavioural coverage is end-to-end in `e2e/` (Playwright), not unit tests: pointer placement,
keyboard placement, failed edits, concurrent document changes and pending-to-canonical reconciliation
all live in `e2e/resource-creation.spec.ts`. The unit tests cover the two pieces with logic worth
isolating, `features/deployment-graph/__tests__/atoms.test.ts` and
`features/deployment-graph/utils/__tests__/layout-invalidation.test.ts`.

Tests live in a `__tests__/` folder beside the code they cover, at whatever depth that code lives.
`tsconfig.app.json` already excludes them from the app build.

Prefer assertions that cannot race. The dev fake channel delays resource-catalog responses to exercise
loading states, and its `catalogDelay` query parameter lets a test hold that state open rather than
competing with the default timing.

## Possible next steps

Not required, and not worth doing without a reason:

- Fold the legacy `DeploymentGraph` shape out of `deployment-graph/api.ts` once the position-preserving apply path no longer needs it.
- Give `Surface` and `IconButton` neutral theme tokens. Both still read `theme.controlBar.*`, so the
  palette launcher styles itself from control-bar tokens, and `Palette` hand-rolls a second
  floating-panel style from raw `var(--vscode-*)` values at a different radius. Unifying those is the
  kind of duplication `ui/` exists to remove.
- Replace the two module-scope `getDefaultStore()` handles in `features/deployment-graph/hooks` with
  the store from context, so the sync pipeline can be driven by a scoped store in tests.
- Extract `lib/graph` into `packages/` **when a second consumer appears, not before**. It is already
  prepared: a clean barrel, a documented injection seam (`nodeConfigAtom`), and no Bicep knowledge,
  enforced by lint. Deliberately not done yet, because today the visual designer is the only consumer —
  neither `deploy-pane` nor `resource-type-explorer` references a graph concept — and a package would
  cost real friction: `packages/components` resolves through `dist/`, with no source alias in the app's
  vite config, so every engine edit would need a package rebuild. The likely second consumer is the
  Bicep playground, which is a larger job than a file move: `src/playground` sits outside this npm
  workspace and shares none of the engine's runtime dependencies (jotai, styled-components, motion).
  The engine's own dependency on `lib/utils` geometry has to be resolved at the same time.

## Related documents

This file is the single source of truth for module structure, dependency direction and naming.

- `.github/instructions/state-management.instructions.md` covers Jotai conventions and defers to this
  file for layout. Keep it that way: add atom guidance there, structural guidance here.
- `resource-creation-design.md` and `visual-graph-protocol.md` describe behaviour and protocol, not
  structure. Their file-path references predate this layout and are stale in places.
