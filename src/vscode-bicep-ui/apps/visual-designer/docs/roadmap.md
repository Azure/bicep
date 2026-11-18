# Visual Designer Roadmap (Proposed)

This document records completed foundations and proposed directions, dependencies, and acceptance
criteria. Uncompleted stages are not a commitment to a particular release. See
[Architecture](./architecture.md) for the current graph, layout, and resource-creation contracts.

## Product principles

- Preserve the graph-viewing experience when editing is disabled: pan, zoom, selection, local node
  repositioning, double-click-to-source, reset layout, and export must continue to work.
- Keep Bicep source as the source of truth for resources and modules. Visual graph positions and
  transient creation previews must not silently modify it.
- Keep creation **drag-first**. Do not add click-to-create as an alternate mouse gesture; a dragged
  placeholder may open a chooser after drop, but does not become a resource until the user selects a
  valid type and version.
- Distinguish local visual-layout changes from Bicep edits in both UI and undo behavior. Do not
  imply that moving a node in the graph changes its deployment scope or module membership.
- Prefer focused, reversible edits with explicit errors and a source preview where the change could
  stop deploying or remove existing configuration.

## Current baseline

- `bicep.visualizer.experimental.enableResourceEditing` defaults to false and controls only the
  dock's Resources tool; resource creation is the only implemented resource-editing feature so far.
- The dock has only a working Resources tool; unimplemented tools are not shown. The resource palette is
  drag-only, with a type catalog, API-version choice, and Featured, Recent (session-local), and All
  browsing views; search covers the whole catalog.
- The dock also offers Select (`V`) and Hand (`H`), whatever the setting: click, modifier-click,
  and marquee multi-selection, moving a selection together, right-drag and hold-Space panning, and
  a node context menu with Go to Source. Hand pans from nodes without moving or selecting them.
- The visualizer follows the VS Code theme **kind** (light, dark, high contrast) using curated graph
  palettes by default. The optional `bicep.visualizer.matchColorTheme` setting instead derives the
  colors from the active color theme, falling back to the curated palette where a theme color is
  missing or lacks contrast. Export offers Current and explicit theme choices.
- A correlated resource drop and undo of its independent creation preserve the other
  nodes' positions and the camera. Edge changes and other topology changes can still reflow the
  graph; automatic layout frames a newly opened graph but keeps the camera for one already shown.
- Node clicks use total pointer travel to distinguish dragging, and cancellation does not change
  focus or selection.
- Bicep source edits use a version-checked `WorkspaceEdit` and VS Code's dirty-file/undo behavior;
  the host also rejects resource creation when editing is disabled. Resource creation, node drags,
  and Reset Layout share a session-local **Undo/Redo** timeline through the history-bar buttons
  and shortcuts while the designer has focus.
  Source replay validates document version and contents; visual layout is not persisted.
- Graph nodes do not currently distinguish deployed from `existing` resources. Node IDs contain
  symbolic names; a rename can look like removal and addition to the graph.
- Local modules, and registry modules whose published Bicep source is in the local module cache,
  expand as containers with child resources and can be collapsed manually to look like resource
  nodes. Collapse state is session-local, survives graph refreshes while IDs remain stable,
  and does not edit Bicep source or move the camera and unrelated nodes. A remote module whose source
  cannot be shown has a disabled toggle that says why, and one whose restore failed can be retried.

These details are documented in [Architecture](./architecture.md), the
[dock](../src/features/dock/components/Dock.tsx),
[graph-layout.ts](../src/core/graph-layout.ts),
[theme palettes](../src/ui/theme/themes.ts),
[color theme matching](../src/ui/theme/color-theme.ts), and the
[graph builder](../../../../Bicep.LangServer/Features/Custom/Visualization/VisualGraphBuilder.cs).

## Recommended delivery order

Stages 0 through 5 are complete. Dependencies, not calendar dates, gate the remaining stages. Stage 4
is viewer-only: it makes no source edits, does not depend on the editing setting, and benefits
everyone who views a graph, so it comes first.

Module presentation (stage 2) precedes the editing stages. It changes what is shown without moving
the camera or unaffected nodes, and keeps the positions of hidden nodes, which structural drops in
stage 7 also need; they still need layout that keeps placed nodes fixed, which stage 2 does not
provide. A collapsed module also gives module drops (stages 7 and 8) a compact initial form that
avoids a large reflow (a module can be marked collapsed before its node arrives), and lets rename
(stage 6) preserve collapsed state from the start. Stage 4 is merged.

Resource placeholders in stage 7 depend only on the stage 0 mutation and layout rules, so they can
proceed alongside stages 5 and 6; module references also build on stage 2. The order within stages
6 and 7 can respond to user feedback.

| Stage                                 | Deliverable                                                                 | Depends on | Exit criterion                                                                                    |
| ------------------------------------- | --------------------------------------------------------------------------- | ---------- | ------------------------------------------------------------------------------------------------- |
| 0. Foundations (complete)             | Viewer regression coverage, gesture fix, mutation rules, undo/layout rules  | —          | Disabled editing still behaves as before; every source mutation is version-checked and reversible |
| 1. Low-risk discovery (complete)      | Optional VS Code theme matching; Featured/All and then Recent palette views | —          | Theme and palette changes do not create source edits or surprise existing viewers                 |
| 2. Module presentation (complete)     | Distinct collapsed-module styling and manual collapse/expand                | 0          | Local modules toggle without source edits or loss of graph context                                |
| 3. Remote-module expansion (complete) | Linked-source remote modules with child resources                           | 2          | Remote children and source links work; unavailable source is explicit                             |
| 4. Resource containers (complete)     | Source-defined RG/MG/VNet/subnet containers                                 | 2          | Agreed containment rules yield readable nested containers in themes and exports                   |
| 5. Interaction model (complete)       | Select/Hand, multi-selection, and node context menu                         | 0          | Gestures and shortcuts are predictable, and every viewing action keeps a gesture                  |
| 6. Focused source edits               | LSP-backed rename, then Convert to existing                                 | 5          | Edits preview/validate correctly, preserve graph context, and undo/redo in the editor             |
| 7. Drag-first creation                | Placeholder-first resources; references to existing local modules           | 0, 2       | Cancel does not edit or lay out the graph; commit honors the placement/layout contract            |
| 8. Module authoring                   | New local module files                                                      | 7          | The new file and parent declaration are one previewed, undoable operation                         |
| 9. Module refactoring                 | Extract to Module from a multiple selection                                 | 5, 8       | A language-server refactoring safely rewrites files and references in one operation               |

### Stage 0: viewer safety and undo foundation (complete)

1. Click-versus-drag detection uses **total pointer travel since pointer-down** and a four-pixel
   threshold. A `pointercancel` does not focus or select a node. Regression tests cover slow,
   multi-event drags and true clicks.
2. Preserve the flag-off experience: no resource creation UI; local drag, pan, zoom,
   double-click-to-source, status, and export stay usable.
3. Resource creation is not rendered when the setting is off. The extension
   rechecks the setting before applying a resource edit, preserves document-version validation,
   and logs failures without showing canvas notifications.
4. The resource-editing opt-in gates resource actions. Module source editing is not implicitly
   covered by this resource-only setting; a broader experimental opt-in must be chosen before
   adding it. All new source-changing actions default to off.
5. Designer-created source edits and local graph movements share a session history with Undo/Redo
   buttons in the control bar and shortcuts while the designer has focus. Source replay applies
   a validated minimal `WorkspaceEdit`, not a focus-dependent VS Code Undo command. Editor and
   text-field histories remain separate. See [Undo and redo](#undo-and-redo).

### Stage 1: theme and palette (complete)

**Match VS Code theme.** The appearance-only `bicep.visualizer.matchColorTheme` setting (off by
default, independent of the editing setting) derives the designer's colors from the active color
theme, including changes between two themes of the same kind. The canvas is the editor background,
so the designer matches the editor beside it, and text and accents take the closest workbench
colors. Nodes and floating bars are a lighter shade of the editor background with the same hue and
most of its tint, in light and dark themes alike (lighter cream on Solarized Light, lighter navy on
Ayu Dark). Nodes take the theme's `editorWidget.border` as is, the outline VS Code draws around the Find
widget and hovers, or the curated border when a theme leaves it out or sets it too faint; edges and
the dot grid keep the curated separation.
Secondary text, such as resource types, is the theme's text color faded toward the node surface rather
than VS Code's default gray description color. Each text and accent
color is checked for contrast (WCAG AA, and higher in high-contrast themes) against what it is
painted on; a theme color that falls slightly short is strengthened along its hue, and the curated
palette for the theme kind supplies any color that is missing, unparseable, or far too faint. Node depth (shadows, border widths) stays curated. Unit tests check every
built-in VS Code theme, including both high-contrast modes. The export preview and "Current" export
use the matched colors; explicit Light/Dark/high-contrast exports stay curated. Whether matching
becomes the default remains open (see [decisions](#decisions-to-confirm-before-implementation)),
pending testing with popular custom themes.

**Palette organization.** Browsing has three views, chosen with tabs below the search box and kept
across palette reopen: **Featured** (the common providers, in the curated order), **Recent** (types
dropped onto the canvas this session, newest first, up to eight), and **All** (every provider,
Microsoft first, each alphabetically). Search covers the whole catalog and hides the tabs while a
query is entered. Provider browsing, scope filtering, lazy catalog loading, progressive rendering,
and API-version selection are unchanged. Recent is session-local; persisting it, and adding
Favorites, waits on the same user-wide versus workspace decision.

### Stage 2: module presentation and controls (complete)

**Collapsed appearance.** A collapsed module is laid out like a resource node: the colored
folder icon, the symbolic name, and a `MODULE` type label. The expanded container's header shows
the smaller folder icon, the name, and the same `MODULE` label inline, the pattern every container
header follows (see [stage 4](#stage-4-resource-containers-complete)). An expanded frame is a region rather
than an object, so it casts no shadow: only nodes and collapsed containers stand above the canvas.
A trailing toggle uses VS Code's `unfold` (expand) and `fold` (collapse) codicons, the same vocabulary as
editor folding. The toggle appears while the module is hovered or selected, or when it has keyboard focus, so
graphs with many modules stay uncluttered. The same focus, error, collection, theme, and export
treatment applies in either state, and exports omit the toggle.

**Manual collapse/expand.** Collapse state is a session-local graph projection, independent of the
resource-editing setting. Descendants and internal edges are hidden; dependencies crossing the
boundary terminate at the collapsed module. The container shrinks to the collapsed module at its center while its descendants fade out; expanding grows the
frame back and fades the descendants in. Hidden descendants keep their positions and move with the module,
so they come back exactly where they were, and the camera and unrelated nodes do not move.
The language server lays out the rendered graph for Reset Layout and later graph updates, checking
only that the graph revision it came from is current. State survives refreshes, even while a module is missing or empty. Collapsing or expanding is an undo
step, so undo never changes nodes the user cannot see: undoing a drag inside a module that was collapsed
afterwards expands it first. Stage 3 supplies remote children through the same controls.

### Stage 3: remote-module expansion (complete)

A registry (`br:`) module published with its Bicep source expands like a local module once it is in
the local module cache: its resources and modules, including nested local and registry modules,
appear inside it with the same IDs, hierarchy, and stage 2 controls, and collapsed state survives
refreshes. Revealing a node inside it opens the published source in a read-only `bicep-extsrc:`
document, as Go to Definition does. Published source is not the user's to fix, so its nodes show no
errors.

A registry module that is not in the module cache yet shows the expand toggle. Expanding it restores
that one module, without editing or recompiling the Bicep file, and then opens it; the toggle shows a
progress ring until it has. Nothing in the
designer restores modules in the background. Children that arrive anyway, because the language server
restored a module the document references, leave the module collapsed, so nothing moves until the user
opens it. Children shown for the first time are laid out around the module before it opens, so it opens
as a local module does, leaving the camera and the other nodes in place.

A module has one toggle in every case. When the module's restore failed or its source archive
cannot be read, the toggle shows the error color and the message, and expanding tries again. When the
module has no Bicep source to show (it was published without it, or it is a template spec), the toggle
is disabled and its tooltip says the source is unavailable.

A remote module, from a registry or a template spec, has a cloud pressed into the front of its folder
icon, collapsed or expanded, so it can be told from a local module at a glance. The graph marks it
with `isRemote`. The cloud is drawn in a darker gold from the folder itself, like the folder's tab, so
it reads as one icon in every theme rather than as a badge added on top.

### Stage 4: resource containers (complete)

Draw a resource as a container around the declarations the source places inside it, such as a
resource group (RG) around its modules or a virtual network (VNet) around its subnets. Containment
comes from relationships that define the contained resource's identity or deployment scope, never
from type lists, dependency edges, or visual proximity, so every container is one ARM would also
recognize. The stage is viewer-only: it makes no source edits and does not depend on the editing
setting. Visual containment must not change deployment scope, module membership, or network
configuration.

**Containment rules.** Within one Bicep file, a node's container is, in order of precedence:

1. **Its parent resource:** the resource it is nested in or the one its `parent` property names
   (the compiler's resource ancestors). This covers VNet -> subnet, storage account -> blob
   service -> container, SQL server -> database, and every other child type without a type list.
2. **The resource its `scope` property names**, when that is a declared
   `Microsoft.Resources/resourceGroups` or `Microsoft.Management/managementGroups` resource
   (`scope: rg`). Modules and `existing` resources deployed into an RG or management group (MG)
   declared in the file appear inside it. The compiler already resolves these two types as scopes.

The first version deliberately excludes these; each keeps its dependency edge, if any:

- A scope that only computes a matching name, such as `resourceGroup(rg.name)`: the name can
  resolve to a resource group in another subscription, so it is a reference, not identity.
- An extension resource scoped to another resource (a role assignment, lock, or diagnostic
  setting with `scope: storageAccount`). These attach to many resources and would turn most of
  them into containers.
- Network membership expressed through property references, such as a NIC's
  `subnet: { id: subnet.id }`. Such references have type-specific meaning, and a NIC can join
  several subnets, so they cannot define a single container.
- A container that is a collection (`parent: vnets[i]`, `scope: rgs[i]`): one node stands for
  many instances, and the index need not map children one-to-one. Nested declarations cannot
  occur under a looped parent (`BCP160`), so only `parent` and `scope` references are affected.
- The file's own target scope, which is not a declared resource; the scope indicator shows it.
  Subscriptions are therefore never containers.

Containment never crosses a file: a module's contents stay inside the module, and the module
itself can sit inside an RG declared in its parent file. Published registry-module source
(stage 3) follows the same rules. A reference that does not resolve, because of an error,
produces no containment.

**Graph contract.** A contained node's `parentId` is its container, but its ID does not change:
it stays the enclosing module path plus its symbolic name. Adding or removing `parent` then
reparents a node without losing its position, collapse state, or undo history, and module paths
stay parseable for registry restore. No code may derive containment from IDs; the devtools
fakes name a resource container explicitly, and only use a sample's ID as shorthand for its module. A resource is a container only while it has children in the
graph: `kind: "resource"` with `hasChildren: true`. A resource with nothing contained, such as a
VM that declares no `extensions` children, stays an ordinary resource node, as a module without
children is a leaf. Adding its first child or removing its last one changes whether it is drawn
around children, which already invalidates layout. The revision already hashes `parentId`, and
a node changing container already invalidates layout.

A dependency from a node on its own container or any other ancestor is implied by the
containment and is dropped, since it would join a node to the frame around it. Every other
dependency stays, which introduces the first edges that **cross container boundaries**, such as
a top-level NIC that depends on a subnet inside a VNet. They are the main implementation cost:

- **Layout.** The MSAGL engine lays out each container's children on their own and assumes that
  every edge joins siblings. A crossing edge instead takes part in layering at the scope that
  contains both ends, between the two ancestors that are siblings there (NIC -> VNet), as the
  collapse projection already lifts edges. It is still drawn to its real endpoints.
- **Paint order.** `lib/graph` paints an edge among the children of its endpoints' shared
  parent. A crossing edge is painted just above the frame of the deeper endpoint's container,
  so it stays visible inside that container and below the node it joins. Outside the container
  it may pass over siblings drawn below the container, another compromise of the single
  stacking context.
- **Collapse.** `projectGraph` already terminates crossing edges at the outermost collapsed
  ancestor at any depth, so no change is needed.

**Collapse.** A resource container collapses and expands with the module toggle, animation, and
undo step. `isCollapsible` becomes "has children" rather than "is a module with children", and
the collapsed set holds node IDs rather than module IDs. Collapsed, a resource container is
simply its resource node. Resource containers start expanded; only remote modules that gain
children are added to the collapsed set automatically.

**Appearance.** Resource and module containers are one family of frames and differ only in their
headers, so the distinction holds in every theme, in high contrast, and in grayscale exports:

- **Header:** every container header shows an icon, the symbolic name, and an inline type label,
  at the same height and padding and with the toggle in the same place. A resource container
  shows its resource type icon and type label (`vnet VIRTUAL NETWORKS`), matching the node it
  collapses to; a module shows the folder icon and `MODULE`.
- **Frame:** both use the module fill (`compoundBackground`), so frames sit a step below the nodes
  inside them, and the ordinary node border. In the curated palettes that border is opaque, so it
  looks the same on the canvas and inside a frame. Expanded frames cast no shadow; nodes and
  collapsed containers keep theirs, so nodes always read as the raised layer. Containers are
  not told apart by border style, tint, or accent: those fight the theme and vanish in high
  contrast.
- **Collapsed:** a collapsed resource container is its resource node. Like a collapsed module, it
  reveals the expand toggle on hover, selection, or keyboard focus rather than always.
- **Shared treatment:** focus, error, collection, theme matching, and export behave as for
  modules, and exports omit the toggle.

**Rollout.** Resource containers ship as a generally available viewer feature, with no setting.
They reshape some existing graphs (a subscription-scope file with an RG and its modules becomes
one large frame), so each step below must be complete enough to release on its own: containers
are never shown without their own appearance or with crossing edges hidden or misplaced.

**Delivered together:** parent and RG/MG scope containment, crossing-edge layout and paint order,
the container appearance, and collapse of resource containers, so no step ships without the
others. The dev playground has resource container, resource group, and nested container samples.

**Tests.** Language-server builder tests cover each rule and exclusion, unchanged IDs when a node
is reparented, and dropped ancestor edges; layout engine tests cover crossing edges. Vitest
covers crossing-edge paint order, projection and collapse of resource containers, and layout
invalidation on reparenting. Playwright or manual checks cover nesting depth, both
high-contrast themes, theme matching, and exports.

### Stage 5: navigation and selection (complete)

- The dock offers Select (`V`) and Hand (`H`), whether or not resource editing is enabled; the
  setting only adds the Resources tool after them. In
  Select, a click selects one node, Shift/Ctrl/Cmd+click toggles a node in the selection, and a
  drag on the background draws a marquee selecting the nodes it touches (a container only when
  enclosed, and then together with everything inside it); a modified marquee adds to the
  selection. Dragging a node moves it, or every selected node when it belongs to a multiple
  selection, as one undo step; a node moves once even when its container is selected too. In
  Hand, a left drag pans even when it starts on a node, and never changes selection or moves
  nodes. A right drag pans in either tool. The designer draws its own cursors, styled like a node
  (a filled shape with a thin outline and a soft shadow) and colored like the dock's icons: the
  outline in the icon color and the fill in the dock's surface, in every theme including matched
  ones. They are the Codicon `cursor` arrow for Select
  and as the default everywhere in the designer, a pointing hand over buttons and other clickable
  controls, and an outlined open hand for Hand and the palette's drag handles that closes while
  panning or dragging. Text fields keep the text cursor. A pan shows the closed hand, and
  pressing or dragging a node with Select shows no indicator at all. The active tool is marked without an accent color. Holding
  Space switches to Hand until it is released (the dock shows Hand meanwhile), so trackpads can
  pan without changing tools; Space keeps its usual meaning while a button or other control has
  focus, and in text fields.
- A module's frame is a drag handle, so a plain marquee cannot start on it. Alt/Option+drag, or
  Ctrl/Cmd+drag, starts a marquee anywhere, including on a node or inside a module's frame, so
  selecting some of a module's nodes is a modified drag inside it. Together: click a module to select
  it alone, modified-drag inside it to select some of its nodes, and drag a marquee around it to
  select it and everything inside. Shift/Ctrl/Cmd+click also adds individual children. Several Linux
  window managers (KDE, Xfce, and some GNOME setups) take Alt+drag to move windows, so it may never
  reach the designer there; Ctrl+drag is the alternative. A Ctrl/Cmd marquee adds to the selection,
  as other modified marquees do.
- Shortcuts are scoped to the designer webview. Text fields, composition, and modified shortcuts
  are not intercepted. A left drag on the background now draws a marquee rather than panning, for
  everyone: pan with Hand, hold Space, right-drag, or the wheel.
- Selection is separate from focus and paint order. Dragging an unselected node raises it without
  selecting it. Blank-canvas clicks and Escape clear selection. Refreshes preserve surviving
  selections; collapsing a container replaces its selected descendants with the container, and
  removed nodes are forgotten.
- A right click that leaves exactly one node selected opens that node's context menu with Go to
  Source; right-clicking an unselected node selects it first, and no menu opens while several
  nodes are selected. Shift+F10 or the Context Menu key opens the menu for a single selected node,
  Escape dismisses it without clearing selection, and keyboard focus returns to the canvas. There
  is no floating node toolbar until there are actions beyond Go to Source.
- Double-click still reveals source in Select; Hand does not reveal source.
  Stage 6 will replace double-click and a second click on an already selected node with inline rename
  in editing mode, retaining Go to Source in the context menu. A second click that becomes a drag
  must continue to move the node rather than rename it, using the stage 0 total-travel threshold.

### Stage 6: focused Bicep edits

**Rename symbolic name.** Double-click a node or click an already selected node to edit its name
inline. Resolve the node's current source location on demand, invoke the existing Bicep LSP rename
instead of replacing text, validate the document version, and apply the result as a normal VS Code
edit. Enter commits; Escape cancels; a failed blur commit keeps the draft and explains the error.
Preserve selection and placement across the graph ID change, including descendants and collapsed
state when a module's symbolic name changes and edges whose IDs embed the renamed node, so a rename is not treated as a
topology change. Go to Source from the context menu must still work.

**Convert a resource to `existing`.** Offer an explicit selected-resource action, not a change of
node appearance alone: `existing` stops this declaration from deploying the resource. Show a
preview of the source diff and any deployment properties that would be removed. Preserve the
symbolic name and type; require a valid `name` and, where necessary, `scope`. Validate dependent
declarations and limit the initial version to straightforward resources rather than silently
rewriting complex nested cases. Add an `isExisting`-style field through server graph, diff,
client protocol, and node rendering so deployed and referenced resources are distinguishable.
Keep an unchanged node at its position when only this metadata changes; re-evaluate layout only
if topology or measured size actually changes. Undo must restore the original declaration.

### Stage 7: drag-first creation and its layout contract

**Resource placeholder.** Drag a generic resource placeholder onto the canvas, then open the
existing searchable type/API-version picker anchored to the drop point. The placeholder is a
temporary overlay outside the authoritative graph: dragging, choosing, Escape, and cancellation
neither modify Bicep nor request graph layout. After a valid choice, reuse the existing
version-checked creation flow and correlate the returned node with the original graph coordinate.
Do not add click-to-create. The post-choice pending node remains distinct from the pre-choice
placeholder. If resource containers (stage 4) are present, dropping inside one is visual placement
only; it must not set a parent, scope, or network relationship.

**Module reference.** Start with dragging a module affordance and choosing an existing local
`.bicep` file. Validate the relative path, required parameters, scope, and generated symbolic
name before inserting a declaration. Make opening its source and handling its child nodes clear:
the graph can expand local modules, so module creation is not always a single-leaf add. Inserting
the module collapsed with the stage 2 appearance keeps the initial layout change small. Follow
with creating a new local module file and its parent declaration as one reviewed, undoable
workspace operation. Registry-module authoring and Extract to Module are separate workflows.

**Placement/layout policy.**

| Event                                                       | Intended outcome                                                                                                |
| ----------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------- |
| Drag, choose a type/path, or cancel a placeholder           | No graph layout, code edit, or camera movement                                                                  |
| Commit an independent explicitly placed resource            | Place it where dropped; do not reflow or auto-fit                                                               |
| Undo an independent explicitly placed resource              | Remove that node; do not reflow surviving nodes or move the camera                                              |
| Commit edges, a module subtree, or other structural changes | Lay out affected nodes if possible; preserve the drop point and camera, with explicit Tidy Layout as a fallback |
| Reset/Tidy Layout requested by the user                     | Recompute layout as requested, with the existing viewport-preservation contract                                 |

Layout invalidation already exempts a correlated `addNode` and the matching independent resource
removal on undo. Edges can still trigger reflow, which moves nodes
under a fixed camera. Review those cases before introducing placeholders or module drops; do not
present the entire policy as shipped behavior.

### Stage 8: new module files

Add a distinct "Create module file" path once existing-file references work. A
valid file, parent declaration, required parameters, and scope are a single user action with a
clear preview and undo behavior. An empty module may render as a leaf until it has children; do not
promise it behaves like a visual group before its source exists.

### Stage 9: Extract to Module

Implement and test the language-server refactoring first. It must handle references across the
selection boundary, module parameters/outputs, dependencies, scopes, file creation, and stale
documents. Then expose it from multi-selection as a previewable, undoable workspace edit. Dragging
resources into a module box remains a **visual move only** unless the user explicitly invokes this
refactoring.

## Undo and redo

- Resource creation, node drags, and Reset Layout now share one chronological undo history.
  A source action applies a normal VS Code edit; undo/redo applies a _new_ minimal
  version- and content-checked edit to the bound Bicep document. This does not pop the native
  editor's original undo element. Direct source changes invalidate designer source replay;
  unaffected layout steps remain available. Reconciliation reads the document, never a
  synthetic webview graph edit. See [Undo and redo](./undo-redo.md).
- A node drag and Reset/Tidy Layout each contribute one layout step at gesture boundaries, not
  per pointer event or animation frame. Keep redo snapshots for nodes absent because their
  designer creation was undone; discard stale IDs after unrelated graph changes. Undo/redo of
  layout steps animates positions without moving the camera, or snaps under reduced motion.
  History lasts for the visualizer session unless persistent layout is separately designed.
- Pan, zoom, focus/selection, opening a chooser, and cancelling an uncommitted placeholder are not
  undo steps. Inline text fields retain normal text-entry undo while focused.
- Do not forward Ctrl/Cmd+Z from a focused webview to VS Code's global Undo: it might affect a
  different editor. While the designer has focus, Ctrl/Cmd+Z calls designer Undo directly, Ctrl/Cmd+Shift+Z
  (or Ctrl+Y on Windows/Linux) calls designer Redo. Do not intercept shortcuts in text fields or
  the source editor. Designer replay is disabled at a conflicting source step after external
  changes; it never guesses which editor undo entry belongs to the designer.
  Rename and module edits need typed reversible transactions and multi-file validation before
  they can join the same timeline.

## Validation and release gates

- **Viewer:** with editing off, the Resources tool and resource edits are absent, and navigation,
  selection, graph interactions, export, and double-click-to-source still work.
- **Gestures:** one-pixel pointer moves accumulating past the threshold do not select; true
  clicks select; cancelled drags do not; Hand never moves nodes; V/H do not fire while typing.
  Once stage 6 ships, double-click and a second click on a selected node enter rename, while a second
  click that becomes a drag only moves the node.
- **Creation/layout:** a placeholder cancelled under pan/zoom creates no file edit, graph node, or
  layout request. An independent placed resource does not move the camera; module children and
  edge changes follow the documented structural-layout policy.
- **Mutations:** rename, `existing` conversion, and module insertion handle stale documents
  and invalid input explicitly, preserve correct graph metadata, and undo/redo through VS Code.
  Check interleaving with visual drags and direct edits in the source editor.
- **Presentation:** light/dark custom themes, both high-contrast modes, export overrides, and
  `existing` resource badges remain legible and reflect actual source state.
- **Module presentation (stage 2):** available module children can be manually collapsed and expanded
  without source edits. Collapsed modules are recognizable as modules, with accessible names and
  controls, and stay legible in themes and exports. Toggles animate, respect reduced motion, and
  preserve boundary dependencies, camera context, and unaffected node positions.
- **Remote modules (stage 3):** linked remote source supplies child resources with working source links
  and the same collapse/expand controls as local modules. Unavailable source is explained explicitly,
  graph refreshes respect the user's collapsed state, and remote modules carry the cloud emblem.
- **Resource containers (stage 4):** containers follow the agreed containment rules, including nested
  containers, and remain distinct from modules. Node IDs survive reparenting, edges that cross a
  container boundary stay visible and lay out sensibly, and containers stay legible in themes and
  exports with editing disabled. A resource without children stays a resource node.

Use focused language-server and extension tests for source edits and gates; Vitest for graph
metadata, layout, and history; and Playwright for viewer compatibility, pointer/keyboard
interactions, theme/export, and source/visual undo scenarios.

## Decisions to confirm before implementation

1. Which broader experimental opt-in should gate module editing? It must not implicitly share the
   resource-only editing setting; decide on its exact form before introducing module edits.
2. Should "Match VS Code theme" become the default or remain optional after contrast/export testing?
3. If a module drop introduces edges or child nodes, when is limited reflow acceptable versus an
   explicit Tidy Layout prompt?
4. Should Recent persist beyond the designer session, and if Favorites are added, are both
   user-wide or workspace-specific?
5. Resource containers follow the shipped parent and RG/MG scope rules. Should extension resources
   scoped to a resource (role assignments, locks, diagnostic settings) ever be contained?
