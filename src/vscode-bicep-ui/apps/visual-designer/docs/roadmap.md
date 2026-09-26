# Visual Designer Roadmap (Proposed)

This document captures proposed directions, dependencies, and acceptance criteria. It is not a
description of shipped behavior or a commitment to a particular release. See
[Architecture](./architecture.md) for the current graph, layout, and resource-creation contracts.

## Product principles

- Preserve the graph-viewing experience when editing is disabled: pan, zoom, local node repositioning,
  focus, double-click-to-source, reset layout, and export must continue to work.
- Keep Bicep source as the source of truth for resources and modules. Decide explicitly whether notes
  live in Bicep metadata or a workspace file. Visual graph positions and transient creation previews
  must not silently modify either.
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
- The dock has working Resources and disabled Modules/Notes affordances. The resource palette is
  drag-only, with a type catalog, API-version choice, and common provider namespaces sorted first.
- The visualizer follows the VS Code theme **kind** (light, dark, high contrast) using curated graph
  palettes, not the current editor theme's exact colors. Export offers Current and explicit theme
  choices.
- A correlated resource drop can preserve its position without a new layout. Edge changes and
  other topology changes can still reflow the graph; automatic layout currently fits the viewport.
- Bicep source edits use a version-checked `WorkspaceEdit` and VS Code's dirty-file/undo behavior.
  Local node moves and reset layout have no undo history or persistent layout storage.
- Graph nodes do not currently distinguish deployed from `existing` resources. Node IDs contain
  symbolic names; a rename can look like removal and addition to the graph.

These details are documented in [Architecture](./architecture.md), the
[dock](../src/features/dock/components/Dock.tsx),
[graph-layout.ts](../src/features/canvas/graph-layout.ts),
[theme palettes](../src/ui/theme/themes.ts), and the
[graph builder](../../../../Bicep.LangServer/Features/Custom/Visualization/VisualGraphBuilder.cs).

## Recommended delivery order

Stages 1 and 2 can proceed independently once stage 0 establishes the viewer contract. The order
within stages 3 and 4 can respond to user feedback; dependencies, not calendar dates, are the gate.

| Stage | Deliverable | Exit criterion |
| --- | --- | --- |
| 0. Foundations | Viewer regression coverage, gesture fix, mutation gates, undo/layout rules | Disabled editing still behaves as before; every source mutation is guarded and reversible |
| 1. Low-risk discovery | Optional VS Code theme matching; Featured/All and then Recent palette views | Theme and palette changes do not create source edits or surprise existing viewers |
| 2. Interaction model | Experimental Select/Hand, selection state, contextual toolbar | Gestures and shortcuts are predictable without changing the default viewer mode |
| 3. Focused source edits | LSP-backed rename, then Convert to existing | Edits preview/validate correctly, preserve graph context, and undo/redo in the editor |
| 4. Drag-first creation | Placeholder-first resources; references to existing local modules | Cancel does not edit or lay out the graph; commit honors the placement/layout contract |
| 5. Broader authoring | New local module files and metadata-backed notes | Multi-file edits and note persistence/visibility are specified and tested |
| 6. Module refactoring | Multi-select and Extract to Module | A language-server refactoring safely rewrites files and references in one operation |

### Stage 0: viewer safety and undo foundation

1. Fix click-versus-drag detection to use **total pointer travel since pointer-down**, rather than
   comparing each move to the four-pixel threshold. A cancelled pointer gesture must not select a
   node. Cover slow, multi-event drags as well as true clicks.
2. Preserve the flag-off experience: no editing dock or editing-only shortcut interception; existing
   focus, local drag, background pan, zoom, double-click-to-source, status, and export stay usable.
3. Check editing permission in the extension **for each mutating request**, including just before
   applying an asynchronously prepared edit. Hiding the webview control is insufficient. Keep
   document-version validation and explicit failure reporting; handle an opt-out during a pending
   operation.
4. Use the resource-editing opt-in for new resource actions. Decide separately whether module and
   note editing should share it or require a broader experimental opt-in. Default all new
   source-changing actions to off until explicitly enabled.
5. Define undo ownership now: source changes use VS Code's edit history; local graph movements need
   their own session history. Test the interaction between the two before exposing a generic Undo
   button. See [Undo and redo](#undo-and-redo).

### Stage 1: theme and palette

**Match VS Code theme.** Offer an appearance-only option that follows the active theme's actual
colors, including changes between two themes of the same light/dark kind. Keep curated graph
contrast or fallbacks for nodes and edges rather than blindly using editor colors for every surface.
This is useful to viewers and should not depend on the editing setting. Decide whether it becomes
the default only after testing custom themes and both high-contrast modes. The export preview and
"Current" export option must capture the intended colors; explicit Light/Dark exports stay stable.

**Palette organization.** Make the existing common-provider ordering discoverable through Featured
and All sections; try Recent before adding Favorites. Preserve provider browsing, scope filtering,
lazy catalog loading, search, and API-version selection. If Favorites proves valuable, decide
whether it follows the user across workspaces or belongs to a workspace before persisting it.

### Stage 2: navigation and selection

- In experimental editing mode, Select (`V`) selects nodes and permits node repositioning; Hand
  (`H`) pans even when the pointer starts on a node. Retain empty-canvas panning in Select until a
  marquee interaction is deliberately designed. Consider Space as a temporary Hand override.
- Scope shortcuts to the focused canvas, not the whole VS Code window. Do not intercept text input
  in palette search, inline names, note editors, or the source editor. Show the active mode and an
  appropriate cursor.
- Separate *selected nodes* from the current single-node focus/z-order state as multi-selection
  becomes necessary. A compact toolbar can start with Go to Source before adding edit actions.
- Keep double-click-to-source. Prefer F2 and a toolbar/context-menu Rename action over making every
  second click on a selected card enter editing; revisit label-only second-click editing after it can
  be distinguished reliably from a double-click or drag.
- Roll out mode switching without changing the existing gestures for people who have not opted into
  editing. Fix the accidental selection problem in stage 0 rather than relying on modes to hide it.

### Stage 3: focused Bicep edits

**Rename symbolic name.** Resolve the selected node's current source location on demand, invoke
the existing Bicep LSP rename instead of replacing text, validate the document version, and apply
the result as a normal VS Code edit. Enter commits; Escape cancels; a failed blur commit keeps the
draft and explains the error. Preserve selection and placement across the graph ID change,
including descendants when a module's symbolic name changes. Source navigation must still work.

**Convert a resource to `existing`.** Offer an explicit selected-resource action, not a change of
card appearance alone: `existing` stops this declaration from deploying the resource. Show a
preview of the source diff and any deployment properties that would be removed. Preserve the
symbolic name and type; require a valid `name` and, where necessary, `scope`. Validate dependent
declarations and limit the initial version to straightforward resources rather than silently
rewriting complex nested cases. Add an `isExisting`-style field through server graph, diff,
client protocol, and node rendering so deployed and referenced resources are distinguishable.
Keep an unchanged card at its position when only this metadata changes; re-evaluate layout only
if topology or measured size actually changes. Undo must restore the original declaration.

### Stage 4: drag-first creation and its layout contract

**Resource placeholder.** Drag a generic resource placeholder onto the canvas, then open the
existing searchable type/API-version picker anchored to the drop point. The placeholder is a
temporary overlay outside the authoritative graph: dragging, choosing, Escape, and cancellation
neither modify Bicep nor request graph layout. After a valid choice, reuse the existing
version-checked creation flow and correlate the returned node with the original graph coordinate.
Do not add click-to-create. The post-choice pending card remains distinct from the pre-choice
placeholder.

**Module reference.** Start with dragging a module affordance and choosing an existing local
`.bicep` file. Validate the relative path, required parameters, scope, and generated symbolic
name before inserting a declaration. Make opening its source and handling its child nodes clear:
the graph can expand local modules, so module creation is not always a single-leaf add. Follow
with creating a new local module file and its parent declaration as one reviewed, undoable
workspace operation. Registry modules and Extract to Module are separate workflows.

**Placement/layout policy.**

| Event | Intended outcome |
| --- | --- |
| Drag, choose a type/path, or cancel a placeholder | No graph layout, code edit, or camera movement |
| Commit an independent explicitly placed resource | Place it where dropped; do not reflow or auto-fit |
| Commit edges, a module subtree, or other structural changes | Lay out affected nodes if possible; preserve the drop point and camera, with explicit Tidy Layout as a fallback |
| Reset/Tidy Layout requested by the user | Recompute layout as requested, with the existing viewport-preservation contract |
| Add or edit a note | Never participate in deployment-dependency layout |

The current layout invalidation already exempts an explicitly placed `addNode`, but edges can
trigger reflow and automatic layout fits the viewport. Review those cases before introducing
placeholders or module drops; do not present this policy as shipped behavior.

### Stage 5: notes and broader module authoring

**Notes.** Drag a note onto an annotation layer, not the deployment-dependency graph. For a
shared, source-controlled note, use a versioned schema in Bicep metadata with stable note IDs and
plain-text content. Decide whether notes attach to a resource/module (and follow it on re-layout)
or float at graph coordinates (and how to restore them on reopen). Source edits or renames must not
silently orphan an attached note. Bicep metadata is included in the compiled ARM template, not
private designer state; make that visible to users and do not encourage storing secrets there.
Decide whether existing notes are visible by default to viewers and included in export; keep
note creation/editing experimental. Consider a workspace-only sidecar if the intended notes must
*not* be included in deployment templates.

**New modules.** Add a distinct "Create module file" path once existing-file references work. A
valid file, parent declaration, required parameters, and scope are a single user action with a
clear preview and undo behavior. An empty module may render as a leaf until it has children; do not
promise it behaves like a visual group before its source exists.

### Stage 6: Extract to Module

Implement and test the language-server refactoring first. It must handle references across the
selection boundary, module parameters/outputs, dependencies, scopes, file creation, and stale
documents. Then expose it from multi-selection as a previewable, undoable workspace edit. Dragging
resources into a module box remains a **visual move only** unless the user explicitly invokes this
refactoring.

## Undo and redo

- A committed source change (create, rename, convert to `existing`, module edit, note metadata
  edit) should be one VS Code undo step and one redo step. Reconciliation after undo/redo reads the
  document; it does not invent a second inverse webview edit. Multi-file operations must be tested
  together, including removal/restoration of a newly created module file.
- A node drag and Reset/Tidy Layout should each be one local history step, recorded at gesture
  boundaries rather than on every pointer movement or animation frame. Preserve the semantics
  when graph nodes appear, disappear, or change IDs. Local history lasts for the visualizer
  session unless persistent layout is separately designed.
- Pan, zoom, focus/selection, opening a chooser, and cancelling an uncommitted placeholder are not
  undo steps. Inline text fields retain normal text-entry undo while focused.
- Do not blindly forward Ctrl/Cmd+Z from a focused webview to VS Code's global Undo: it might
  affect a different editor. Start with clearly named Undo Layout/Redo Layout alongside native
  source undo. A unified designer Undo/Redo requires a safe policy for interleaving source edits,
  local moves, and edits made directly in the Bicep editor.

## Validation and release gates

- **Viewer:** with editing off, dock/edit actions are absent, and existing navigation, graph
  interactions, export, and source reveal still work. Toggling the setting while an edit is in
  flight cannot apply a source change after it is disabled.
- **Gestures:** one-pixel pointer moves accumulating past the threshold do not select; true
  clicks select; cancelled drags do not; Hand never moves nodes; V/H/Space do not fire while typing.
- **Creation/layout:** a placeholder cancelled under pan/zoom creates no file edit, graph node, or
  layout request. An independent placed resource does not move the camera; module children and
  edge changes follow the documented structural-layout policy.
- **Mutations:** rename, `existing` conversion, notes, and module insertion handle stale documents
  and invalid input explicitly, preserve correct graph metadata, and undo/redo through VS Code.
  Check interleaving with visual drags and direct edits in the source editor.
- **Presentation:** light/dark custom themes, both high-contrast modes, export overrides, optional
  notes, and `existing` resource badges remain legible and reflect actual source state.

Use focused language-server and extension tests for source edits and gates; Vitest for graph
metadata, layout, and history; and Playwright for viewer compatibility, pointer/keyboard
interactions, theme/export, and source/visual undo scenarios.

## Decisions to confirm before implementation

1. Should module and note editing share the resource-editing setting or require a broader editing
   opt-in? Recommendation: keep resource actions under this setting and decide on a broader gate
   before introducing non-resource editing.
2. Should "Match VS Code theme" become the default or remain optional after contrast/export testing?
3. Which undo affordances should appear before source and visual histories can be safely unified?
4. If a module drop introduces edges or child nodes, when is limited reflow acceptable versus an
   explicit Tidy Layout prompt?
5. Should notes be shipped in ARM metadata, kept in a workspace-only file, or offer both? How are
   free-floating positions, anchors, visibility, and export represented?
6. If Favorites are added, are they user-wide or workspace-specific? What accessible creation
   alternative preserves the drag-first mouse model without adding click-to-create?
