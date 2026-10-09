# Visual Designer Roadmap (Proposed)

This document records completed foundations and proposed directions, dependencies, and acceptance
criteria. Uncompleted stages are not a commitment to a particular release. See
[Architecture](./architecture.md) for the current graph, layout, and resource-creation contracts.

## Product principles

- Preserve the graph-viewing experience when editing is disabled: pan, zoom, local node repositioning,
  focus, double-click-to-source, reset layout, and export must continue to work.
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

- `bicep.visualizer.experimental.enableResourceEditing` defaults to false and currently hides the
  creation dock. Viewer interactions remain available; resource creation is the only implemented
  resource-editing feature so far.
- The dock has only a working Resources tool; unimplemented tools are not shown. The resource palette is
  drag-only, with a type catalog, API-version choice, and Featured, Recent (session-local), and All
  browsing views; search covers the whole catalog.
- The visualizer follows the VS Code theme **kind** (light, dark, high contrast) using curated graph
  palettes by default. The optional `bicep.visualizer.matchColorTheme` setting instead derives the
  colors from the active color theme, falling back to the curated palette where a theme color is
  missing or lacks contrast. Export offers Current and explicit theme choices.
- A correlated resource drop and undo of its independent creation preserve the other
  nodes' positions and the camera. Edge changes and other topology changes can still reflow the
  graph; automatic layout currently fits the viewport.
- Node clicks use total pointer travel to distinguish dragging, and cancellation does not change
  focus. With resource editing off, the creation dock is absent without disabling viewer gestures.
- Bicep source edits use a version-checked `WorkspaceEdit` and VS Code's dirty-file/undo behavior;
  the host also rejects resource creation when editing is disabled. Resource creation, node drags,
  and Reset Layout share a session-local **Undo/Redo** timeline through the history-bar buttons
  and shortcuts while the designer has focus.
  Source replay validates document version and contents; visual layout is not persisted.
- Graph nodes do not currently distinguish deployed from `existing` resources. Node IDs contain
  symbolic names; a rename can look like removal and addition to the graph.
- The graph can expand local modules into containers with child resources. Collapsed modules
  currently look like regular resource nodes.

These details are documented in [Architecture](./architecture.md), the
[dock](../src/features/dock/components/Dock.tsx),
[graph-layout.ts](../src/core/graph-layout.ts),
[theme palettes](../src/ui/theme/themes.ts),
[color theme matching](../src/ui/theme/color-theme.ts), and the
[graph builder](../../../../Bicep.LangServer/Features/Custom/Visualization/VisualGraphBuilder.cs).

## Recommended delivery order

Stages 0 and 1 are complete. Dependencies, not calendar dates, gate the remaining stages. Stages 2
through 4 are viewer-only: they make no source edits, do not depend on the editing setting, and
benefit everyone who views a graph, so they come first.

Module presentation (stage 2) precedes the editing stages. Its partial relayout, which updates
affected nodes while preserving the camera and unaffected positions, is the same capability that
structural drops in stage 7 need. A collapsed module also gives module drops (stages 7 and 8) a
compact initial form that avoids a large reflow, and lets rename (stage 6) preserve collapsed state
from the start. Stage 4 is gated on containment design rather than code; the editing stages do not
wait for it.

Resource placeholders in stage 7 depend only on the stage 0 mutation and layout rules, so they can
proceed alongside stages 5 and 6; module references also build on stage 2. The order within stages
6 and 7 can respond to user feedback.

| Stage                            | Deliverable                                                                    | Depends on | Exit criterion                                                                                    |
| -------------------------------- | ------------------------------------------------------------------------------ | ---------- | ------------------------------------------------------------------------------------------------- |
| 0. Foundations (complete)        | Viewer regression coverage, gesture fix, mutation rules, undo/layout rules     | —          | Disabled editing still behaves as before; every source mutation is version-checked and reversible |
| 1. Low-risk discovery (complete) | Optional VS Code theme matching; Featured/All and then Recent palette views    | —          | Theme and palette changes do not create source edits or surprise existing viewers                 |
| 2. Module presentation           | Distinct collapsed-module styling and manual collapse/expand                   | 0          | Local modules toggle without source edits or loss of graph context                                |
| 3. Remote-module expansion       | Linked-source remote modules with child resources                              | 2          | Remote children and source links work; unavailable source is explicit                             |
| 4. Resource containers           | Source-defined RG/MG/VNet/subnet containers                                    | Design     | Agreed containment rules yield readable nested containers in themes and exports                   |
| 5. Interaction model             | Experimental Select/Hand, selection state, contextual toolbar and context menu | 0          | Gestures and shortcuts are predictable without changing the default viewer mode                   |
| 6. Focused source edits          | LSP-backed rename, then Convert to existing                                    | 5          | Edits preview/validate correctly, preserve graph context, and undo/redo in the editor             |
| 7. Drag-first creation           | Placeholder-first resources; references to existing local modules              | 0, 2       | Cancel does not edit or lay out the graph; commit honors the placement/layout contract            |
| 8. Module authoring              | New local module files                                                         | 7          | The new file and parent declaration are one previewed, undoable operation                         |
| 9. Module refactoring            | Multi-select and Extract to Module                                             | 5, 8       | A language-server refactoring safely rewrites files and references in one operation               |

### Stage 0: viewer safety and undo foundation (complete)

1. Click-versus-drag detection uses **total pointer travel since pointer-down** and a four-pixel
   threshold. A `pointercancel` does not focus or select a node. Regression tests cover slow,
   multi-event drags and true clicks.
2. Preserve the flag-off experience: no editing dock or editing-only shortcut interception; existing
   focus, local drag, background pan, zoom, double-click-to-source, status, and export stay usable.
3. Editing controls and gestures are not rendered or bound when the setting is off. The extension
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
colors. Cards and floating bars are a lighter shade of the editor background with the same hue and
most of its tint, in light and dark themes alike (lighter cream on Solarized Light, lighter navy on
Ayu Dark), with a firmer border where a near-white
background leaves little room to lighten; edges and the dot grid keep the curated separation.
Secondary text, such as resource types, is the theme's text color faded toward the card rather
than VS Code's default gray description color. Each text and accent
color is checked for contrast (WCAG AA, and higher in high-contrast themes) against what it is
painted on; a theme color that falls slightly short is strengthened along its hue, and the curated
palette for the theme kind supplies any color that is missing, unparseable, or far too faint. Card depth (shadows, border widths) stays curated. Unit tests check every
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

### Stage 2: module presentation and controls

**Collapsed appearance.** Explore a small container node with a package icon at its center,
rather than a regular resource card. This retains the module's container identity while its
contents are hidden. Keep the module name visible and provide a clear, accessible expansion
affordance. Validate the size, label placement, focus treatment, contrast, and relationship to the
expanded module boundary before committing to the design.

**Manual collapse/expand.** Give local and remote modules the same controls to collapse and expand
available contents; stage 3 supplies remote contents from linked source. Hide or reveal descendants
without changing Bicep source, preserve dependencies across the module boundary, and keep source
navigation usable in either state. Preserve the camera and unaffected nodes when updating layout;
stage 7 reuses this partial relayout for structural drops. Unavailable contents must be explained
explicitly rather than presented as an empty module.

### Stage 3: remote-module expansion

When a remote module has linked Bicep source files, render it as an expanded module container with
its resources inside, using the same hierarchy as local modules and the controls from stage 2.
Preserve source navigation for the module declaration and its child resources, and respect the
user's collapsed state when the graph refreshes. If linked source is unavailable, keep the
module-specific collapsed appearance and explain why its contents cannot be expanded.

### Stage 4: resource containers

Render container resources such as resource groups (RGs), management groups (MGs), virtual
networks (VNets), and subnets as container nodes with their contained resources inside. Base
containment on source-defined scope, parent, or network relationships, not arbitrary dependency
edges or visual proximity. The supported resource types, containment rules, and visual treatment
need design; a dashed border is a candidate, not a committed style. Keep resource containers
distinguishable from module containers while preserving resource identity, source navigation,
theme contrast, and export readability. Visual containment must not silently change deployment
scope, module membership, or network configuration.

### Stage 5: navigation and selection

- In experimental editing mode, Select (`V`) selects nodes and permits node repositioning; Hand
  (`H`) pans even when the pointer starts on a node. Retain empty-canvas panning in Select until a
  marquee interaction is deliberately designed. Consider Space as a temporary Hand override.
- Scope shortcuts to the focused designer webview, not the whole VS Code window. Do not intercept text input
  in palette search, inline names, or the source editor. Show the active mode and an appropriate
  cursor.
- Separate _selected nodes_ from the current single-node focus/z-order state as multi-selection
  becomes necessary. Add a compact contextual toolbar and a node-specific context menu
  with Go to Source before adding edit actions.
- With editing enabled, double-clicking a node, or clicking a node that is already selected, enters
  inline rename (stage 6), and Go to Source moves to the contextual toolbar and context menu. With
  editing disabled, double-click keeps revealing source. A second click that starts a drag moves the
  node instead of entering rename, using the stage 0 total-travel threshold.
- Roll out mode switching without changing the existing gestures for people who have not opted into
  editing. Fix the accidental selection problem in stage 0 rather than relying on modes to hide it.

### Stage 6: focused Bicep edits

**Rename symbolic name.** Double-click a node or click an already selected node to edit its name
inline. Resolve the node's current source location on demand, invoke the existing Bicep LSP rename
instead of replacing text, validate the document version, and apply the result as a normal VS Code
edit. Enter commits; Escape cancels; a failed blur commit keeps the draft and explains the error.
Preserve selection and placement across the graph ID change, including descendants and collapsed
state when a module's symbolic name changes and edges whose IDs embed the renamed node, so a rename is not treated as a
topology change. Go to Source from the toolbar or context menu must still work.

**Convert a resource to `existing`.** Offer an explicit selected-resource action, not a change of
card appearance alone: `existing` stops this declaration from deploying the resource. Show a
preview of the source diff and any deployment properties that would be removed. Preserve the
symbolic name and type; require a valid `name` and, where necessary, `scope`. Validate dependent
declarations and limit the initial version to straightforward resources rather than silently
rewriting complex nested cases. Add an `isExisting`-style field through server graph, diff,
client protocol, and node rendering so deployed and referenced resources are distinguishable.
Keep an unchanged card at its position when only this metadata changes; re-evaluate layout only
if topology or measured size actually changes. Undo must restore the original declaration.

### Stage 7: drag-first creation and its layout contract

**Resource placeholder.** Drag a generic resource placeholder onto the canvas, then open the
existing searchable type/API-version picker anchored to the drop point. The placeholder is a
temporary overlay outside the authoritative graph: dragging, choosing, Escape, and cancellation
neither modify Bicep nor request graph layout. After a valid choice, reuse the existing
version-checked creation flow and correlate the returned node with the original graph coordinate.
Do not add click-to-create. The post-choice pending card remains distinct from the pre-choice
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
removal on undo. Edges can still trigger reflow, and automatic structural layout fits
the viewport. Review those cases before introducing placeholders or module drops; do not
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

- **Viewer:** with editing off, dock/edit actions are absent, and existing navigation, graph
  interactions, export, and double-click-to-source still work.
- **Gestures:** one-pixel pointer moves accumulating past the threshold do not select; true
  clicks select; cancelled drags do not; Hand never moves nodes; V/H/Space do not fire while typing.
  With editing on, double-click and a second click on a selected node enter rename, while a second
  click that becomes a drag only moves the node.
- **Creation/layout:** a placeholder cancelled under pan/zoom creates no file edit, graph node, or
  layout request. An independent placed resource does not move the camera; module children and
  edge changes follow the documented structural-layout policy.
- **Mutations:** rename, `existing` conversion, and module insertion handle stale documents
  and invalid input explicitly, preserve correct graph metadata, and undo/redo through VS Code.
  Check interleaving with visual drags and direct edits in the source editor.
- **Presentation:** light/dark custom themes, both high-contrast modes, export overrides, and
  `existing` resource badges remain legible and reflect actual source state.
- **Module presentation (stage 2):** available module contents can be manually collapsed and expanded
  without source edits. Collapsed modules are recognizable as modules, with accessible names and
  controls; evaluate the compact-container/package-icon candidate in themes and exports. Toggles
  preserve boundary dependencies, camera context, and unaffected node positions.
- **Remote modules (stage 3):** linked remote source supplies child resources with working source links
  and the same collapse/expand controls as local modules. Unavailable source is explained explicitly,
  and graph refreshes respect the user's collapsed state.
- **Resource containers (stage 4):** containers reflect the agreed containment rules, including nested
  containers, remain distinct from modules, and stay legible in themes and exports with editing
  disabled.

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
5. Does a compact container with a centered package icon communicate a collapsed module clearly?
   Confirm the module label, expansion affordance, focus styling, and relationship to the expanded
   boundary before choosing the final design.
6. Should module collapse state be session-local or persisted, and should toggles participate in
   the designer's visual undo history?
7. Which resource types and source relationships define containment, and how should resource
   containers look? Evaluate dashed borders without committing to them before design review.
