# Bicep Visual Designer

A React webview that renders a Bicep file's deployment graph and lets you edit it — pan and zoom the
canvas, reveal a node's source, export a diagram, and create resources from a palette.

It runs inside the `vscode-bicep` extension, which builds it and serves the bundle from
`out/visual-designer/`. In development it runs standalone against a fake extension host, so you can
work on it without launching VS Code.

## Getting started

Run everything from the workspace root (`src/vscode-bicep-ui`) so sibling packages resolve:

```bash
npm install
npm run build          # turbo: builds packages, then apps
```

Then, from `apps/visual-designer`:

```bash
npm run dev            # standalone dev server against the fake host
npm run test           # vitest unit tests
npm run e2e            # playwright end-to-end tests (npm run e2e:install first)
npm run lint
```

`npm run build` must run from the workspace root: `tsc -b` in the app cannot resolve
`@vscode-bicep-ui/*` on its own. `lint` runs with `--max-warnings 0` and
`--report-unused-disable-directives`, so warnings and stale suppressions both fail.

The dev server loads `devtools/`, a fake extension host that implements the whole protocol. Query
parameters drive it — `?catalogDelay=…` holds the palette's loading state open, for instance — which
is how e2e reaches states the real host would race.

## Project structure

| Layer      | Path            | Contains                                                                  |
| ---------- | --------------- | ------------------------------------------------------------------------- |
| `app`      | `src/app/`      | Composition root: provider stack, wiring, global style. No product logic. |
| `features` | `src/features/` | User-facing capabilities. Owns product state and Bicep vocabulary.        |
| `devtools` | `src/devtools/` | A fake extension host so the webview runs standalone. Dev-only.           |
| `hooks`    | `src/hooks/`    | Cross-cutting concerns, each owning its own host conversation.            |
| `ui`       | `src/ui/`       | Workflow-neutral primitives, motion tokens and theme. No Bicep knowledge. |
| `lib`      | `src/lib/`      | Reusable libraries: the headless graph engine and the math library.       |
| `utils`    | `src/utils/`    | Shared helpers belonging to no library: text casing, error messages.      |

```text
app       -> features, ui, hooks, lib, utils, devtools
devtools  -> features, ui, hooks, lib, utils
features  -> ui, hooks, lib, utils, other features (barrel only, acyclic)
ui        -> lib, utils
hooks     -> lib, utils
lib       -> lib, utils
utils     -> utils
```

Everything not listed is forbidden, including `ui -> hooks`, which keeps primitives taking props
rather than reaching into global state.

```text
src/
  app/                     # App, AppProviders, GlobalStyle
  features/
    canvas/                # the design surface: hydrates and edits the deployment graph
      components/          # CanvasView, PendingResourceLayer, nodes/
      hooks/               # use-graph-update (the update state machine), use-apply-graph
      utils/               # layout-invalidation, viewport
      api.ts atoms.ts types.ts
    palette/               # resource type catalog, search, drag-to-create
    controls/ export/ status/
  hooks/                   # use-document-sync, use-motion-policy-sync
  devtools/                # components/, hooks/, fakes/
  lib/
    graph/                 # atoms/, components/, hooks/, theme.ts
    math/                  # geometry/, comparison.ts
  ui/                      # components/, motion.ts, theme/
  utils/                   # text.ts, errors.ts
```

### What goes where

The line between `lib` and `features` is **not** "logic vs. UI". It is _would this still make sense in
an app that had nothing to do with Bicep?_ A headless graph engine would. A pending-resource
reconciler would not.

`lib` holds **libraries** — code with a subject of its own, which is why `geometry/` sits inside
`lib/math` rather than beside it. `utils` holds what is left when every library has taken its own.

A **cross-cutting concern is not a feature**, even when it owns protocol and state. Motion policy and
the document are consulted by the whole app and render nothing, so they live in `src/hooks/` as single
self-contained files: descriptor, atom and sync hook are one thought.

`devtools` is **not a feature** either. Features are slices of the product; devtools impersonates the
other side of the wire, which is why it is the one module allowed to import every feature's `api.ts`.
Only `app` may import it, and `loadDevAppShell` returns `undefined` in production so the chunk
tree-shakes away.

Feature-to-feature imports are fine — `palette` renders the node preview a dropped resource will
become — but they must go through the target's `index.ts` and must not form a cycle. Resolve a cycle
by putting each shared symbol with its real owner, not by forbidding the edge. The current shape:

```text
controls  -> canvas, export, status
palette   -> canvas
canvas    -> export, status
```

### Feature shape

Every feature and `lib` module organises its contents the same way, so a reader who opens one can
guess where things are in any other:

| Folder / file | Holds                                                                     |
| ------------- | ------------------------------------------------------------------------- |
| `components/` | Components, including any used only inside the feature.                   |
| `hooks/`      | Reusable `use-*` hooks.                                                   |
| `utils/`      | Pure helpers with no React dependency.                                    |
| `api.ts`      | The host protocol this feature uses: descriptors and payload shapes.      |
| `atoms.ts`    | Feature state. Splits into `atoms/` only when it holds distinct concerns. |
| `types.ts`    | Shared domain vocabulary.                                                 |

Include only what a feature needs, and add a folder when its first member arrives rather than
scaffolding it empty. Concept subfolders are allowed inside a type folder when they name a real seam:
`components/nodes/` is the content plugged into `lib/graph`'s node containers. Tests live in
`__tests__/` beside the code they cover, at whatever depth that is.

### Naming

- **Components**: PascalCase, filename equals the exported component. One primary component per file.
- **Everything else** (hooks, atoms, utils, types) and **folders**: kebab-case.
- **A hook file is named for the hook it exports** — `use-palette-drag.ts` exports `usePaletteDrag`.
  Worth a mechanical check when adding one: the drift is invisible at the import site.
- **Name a feature for the capability it delivers, not the data it displays.** `CanvasView`,
  `CanvasActions` and `useCanvasApi` name the surface the user works on; the graph they carry is named
  by the protocol types (`CanonicalGraph`, `RenderedGraph`). One set of words must not do both jobs.
- **Name a thing for what it is in the domain, not its visual container.** `ResourceNodePreview`, not
  `ResourcePreviewCard`; "card" describes a border radius.
- Prefer one-word folders, and treat a compound name as a prompt to check whether the folder is doing
  two jobs or dodging a collision. A two-word name for one real concept is fine.
- Do not prefix a file with the folder containing it. This yields to the hook-file rule above.

### Public surface

Each feature, `lib` module and `src/hooks/` exposes exactly one entry point: its `index.ts`.

- Import through the barrel: `@/lib/graph`, `@/features/export`, `@/hooks`.
- Do not deep-import across a boundary; `@/lib/math/geometry` is `@/lib/math`.
- **Within a module, import relatively** — never `@/its-own-name/...`. Reaching your own siblings
  through the barrel creates a cycle, and an aliased deep path sits one keystroke from the form that
  does.
- Feature barrels export an intended surface; `lib` and `ui` keep `export *`.
- Export only what crosses the boundary. Because `api.ts` is re-exported through the barrel,
  `noUnusedLocals` stops seeing a symbol once it is exported, so unused payload types accumulate
  silently.

### State

| State                                                     | Owner                          |
| --------------------------------------------------------- | ------------------------------ |
| Canonical graph nodes, edges, boxes, bounds, focus        | `lib/graph`                    |
| Node content registration, pending resources, transitions | `features/canvas`              |
| Palette interaction and resource catalog                  | `features/palette`             |
| Export workflow                                           | `features/export`              |
| User-facing graph status                                  | `features/status`              |
| Effective motion policy                                   | `hooks/use-motion-policy-sync` |
| Document being visualized                                 | `hooks/use-document-sync`      |
| Active theme                                              | `ui/theme`                     |

Jotai for shared state that benefits from isolated subscriptions; local state when it has one owner.
Across a boundary expose derived values and action atoms, never raw writable atoms.

**Canvas actions travel through context, not Jotai.** `CanvasView` publishes `CanvasActions`
(`createResource`, `canPlaceAt`, `resetLayout`) that `ControlBar` and `Palette` read with
`useCanvasActions`. Two constraints shape this:

- `useGraphUpdate` is a single-instance state machine holding the client's copy of the server's
  graph, so only `CanvasView` may call it. A hook that called it again would corrupt patch
  application. A hook that _reads what the single instance published_ is fine — that is the
  difference between the two shapes.
- Jotai is the wrong tool for callbacks. `set(atom, fn)` is read as an updater, so storing one
  requires `set(atom, () => fn)`; a registration atom is also null until the owner mounts, which
  pushes a null branch into every consumer. Atoms hold state — `graphControlAvailabilityAtom` derives
  three booleans from `hasNodes` and belongs in Jotai. Imperative machinery does not.

`useCanvasActions` throws outside a `CanvasView`, so misuse is a loud error rather than a silent
null.

### Protocol

Each feature declares the host messages it uses in its own `api.ts`, and exposes them through an API
hook (`useCanvasApi`, `usePaletteApi`) so callers make method calls instead of hand-assembling
messages. Descriptors are typed via `defineRequest` / `defineNotification` from
`@vscode-bicep-ui/messaging`, which owns _how_ to talk while each feature owns _what it says_.

Payloads are suffixed `Params` and `Result`. A descriptor and the API method that sends it share a
name deliberately — they are one operation named at two levels.

See [visual-graph-protocol.md](./docs/visual-graph-protocol.md) for the wire contract itself.

### Reconciliation

One pass answers two separate questions: whether the server's graph has moved on, and whether what
we display has been laid out. Three modules split that work:

| Module                        | Owns                                                                   |
| ----------------------------- | ---------------------------------------------------------------------- |
| `graph-update-coordinator.ts` | When each step runs. No React, no Jotai.                               |
| `use-graph-update.ts`         | The client's graph copy, measurement, patch application, Jotai writes. |
| `use-apply-graph.ts`          | Turning a graph into mounted nodes and edges.                          |

The coordinator tracks what is _owed_ — an update, and a layout that is `none`, `auto` or `reset` —
rather than what is running. Its rules:

- Reconcile before laying out, so a layout always applies to the current graph.
- A reset outranks an automatic layout, so Reset Layout is never downgraded.
- A `graphChanged` layout re-pends the update **and** the layout it abandoned, keeping its mode.
- Mutations run one at a time, and a reconciliation that overlaps one is abandoned and retried.
- Every request resolves only once the coordinator runs out of work, never when work is merely
  recorded. Callers gate on that promise — `useResetLayout` holds its deduplication lock for exactly
  as long as it — so resolving early lets a second click queue a second server layout.

Collapsing the update and layout questions into one flag is what previously let a `graphChanged`
response drop the layout it owed, leaving the graph hidden behind the visibility gate
`use-apply-graph` closes when most of the topology is replaced. Every rule guards an ordering hazard
that is impractical to force end to end, which is why the coordinator is React-free and unit tested
with controlled promises.

Most keystrokes stop early: `displayedGraphsEqual` compares exactly the fields the apply path reads,
so an edit the canvas cannot show costs nothing.

### Enforcement

Structure rules that are not machine-checked decay. `../../eslint.config.mjs` carries import-boundary
rules scoped to this app, built on core `no-restricted-imports` with flat-config `files` zones:

- each layer's forbidden imports, per the table above
- `src/lib/graph/**` may not import any messaging module

The last is not a layer rule. `lib/graph -> a messaging module` is a legal `lib -> lib` edge, so
nothing else would stop the engine from learning the host protocol; Bicep behaviour reaches it through
`nodeConfigAtom` instead. `lib/graph/theme.ts` closes the same kind of gap for styling: the engine
declares the theme tokens it needs, and `DefaultTheme` extends that interface, so dropping one is a
compile error rather than a blank canvas.

The shared-`hooks` and `utils` layers are matched through the `@/` alias only, because both are also
folder names inside features and `**/hooks/**` would flag every feature's own `../hooks/use-x`.

## Testing

Most behavioural coverage is end-to-end in `e2e/` (Playwright): palette visibility, pointer and
keyboard placement, drop rejection, catalog loading and search. Unit tests cover the pieces with logic
worth isolating — graph atoms, patch application, layout invalidation, the export file stem, and the
update coordinator's ordering rules.

Prefer assertions that cannot race. Poll for a settled value rather than sampling once: nodes animate
in and the graph springs to its layout over ~0.6s, so a single read taken when a node appears can land
mid-flight. A graph load runs two independent animations — the fit-view transform and each node's
spring — so waiting on the wrong one passes about half the time.

Mutation-check a test that encodes an ordering rule: revert the fix it covers and confirm it fails.
The coordinator's rules all look plausible when broken.

**The resource-creation failure path has no coverage.** The host reports failures as
`CreateResourceErrorResult` from four call sites, but the dev fake always succeeds, so neither the
error atom nor `ResourceCreationError` is exercised. Teaching the fake to fail on demand — the way
`catalogDelay` makes the loading state reachable — is the missing piece.

## Known gaps

- `GraphUpdatePatch` and `GraphLayoutPatch` are the same union, so a layout response is typed as
  though it could carry `addNode`. Splitting them would let the compiler reject a phase mismatch that
  is currently only a convention.
- `Palette` hand-rolls a second floating-panel style from raw `var(--vscode-*)` values at a different
  radius; folding it onto `FloatingPanel` is the duplication `ui/` exists to remove.
- Two module-scope `getDefaultStore()` handles in `features/canvas/hooks`, and the coordinator beside
  them, should come from context, so the sync pipeline can be driven by a scoped store in tests. They
  move together: scoping one without the others buys nothing.
- Share the protocol declarations with the extension host. `vscode-bicep` dispatches on raw string
  literals and casts params with `as`, so the two sides agree only by convention. It has no npm
  dependency on `vscode-bicep-ui` today, so this needs a `file:` dependency and a build-order
  constraint — and should be per-feature modules in a shared package, not one central protocol file.
- Extract `lib/graph` into `packages/` **when a second consumer appears, not before**. It is prepared:
  a clean barrel, a documented injection seam (`nodeConfigAtom`), a declared theme contract, and no
  Bicep knowledge, enforced by lint. The likely consumer is the playground, which sits outside this npm
  workspace and shares none of the engine's runtime dependencies.

## Further reading

- [visual-graph-protocol.md](./docs/visual-graph-protocol.md) — the server-driven graph and layout protocol.
- [resource-creation-design.md](./docs/resource-creation-design.md) — the resource creation feature design.
- [.github/instructions/](./.github/instructions) — React, state management and styling conventions,
  applied automatically by Copilot when editing matching files.
