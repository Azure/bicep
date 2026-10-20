# Undo and redo

## Scope and ownership

The visual designer has **one chronological, session-local history** of actions initiated in the
designer. Stage 0 records resource creation, node drags, and Reset Layout. **Undo** and **Redo**
traverse that history from the history-bar buttons or keyboard shortcuts while the designer has focus.
They do not present separate source and layout commands.
Source edits made directly in the Bicep editor are **not** designer actions.
Renaming, conversion to `existing`, and module edits are future additions to this contract, not
implemented actions.

The webview owns the order of designer actions, layout snapshots, and the placement associated with
a created node. The extension owns source transactions and the text each creation inserted; the
language server is the only participant allowed to construct source undo/redo edits. An opaque
`operationId` links them; no Bicep source text is sent to the webview for history. All of them
discard their history when the visualizer session ends.

While the designer has focus outside a text field, Ctrl/Cmd+Z invokes designer Undo and
Ctrl/Cmd+Shift+Z or Ctrl+Y (Windows/Linux) invokes designer Redo. These invoke the designer
history directly, never VS Code's global Undo command. Text inputs and the source editor retain
their native undo/redo behavior. The designer commands are separate from VS Code's editor undo stack,
although each source edit, including a designer reversal, is a normal dirty-file `WorkspaceEdit`
that the editor can itself undo.

## Why designer source undo is not `executeCommand("undo")`

VS Code's built-in Undo/Redo commands act on the focused control or active editor; the public API
does not accept a document URI or an undo-stack entry token. Focusing the Bicep editor before
invoking Undo could undo an intervening direct editor change rather than the designer action. The
designer instead has the language server prepare an **exact, minimal inverse** of its own source
edit against the current document, applies it as a new `WorkspaceEdit`, and verifies the result
before moving the undo history cursor. It never replaces the whole Bicep file with an old snapshot.

This deliberately creates a _new_ VS Code undo step; it does not pop the original native undo
element. A subsequent native editor Undo may therefore reapply a designer-undone edit. The next
graph update then reports that the designer step no longer applies, and the designer disables it.
Users can continue using the editor's native history, but must not expect both histories to share a
cursor. This is preferable to running a focus-dependent command that might change another editor.

## When a source step can be replayed

A designer source step is offered only when replaying it is known to reverse or restore exactly what
the designer did, whatever else changed in the file:

- **Undo a creation** requires the declaration named by the node ID to be byte-for-byte the text the
  designer inserted (so no user edit is discarded) and to have no references (so no other
  declaration breaks). The edit removes the declaration and the blank lines inserted with it, where
  they are still next to it.
- **Redo a creation** requires its symbolic name to be free. The edit reinserts the same text where a
  new resource goes: after the last resource.

Edits elsewhere in the file (blank lines, other declarations, formatting that leaves the declaration
alone) do not affect either check, and nothing tracks offsets between edits. The language server
finds the declaration in the current compilation every time.

**Undo and Redo are never enabled for a step that cannot run.** Every graph update request carries
the history's source steps with their next direction, and the response lists the ones the language
server can replay against the same document. The designer enables a source step only while it is in
the latest confirmed list. Unconfirmed steps stay in the history, disabled, because an edit in
progress (a half-typed brace earlier in the file, say) can make one replayable again; a disabled
step at the top of a stack blocks the steps beneath it until that happens.

Confirmation is withdrawn as soon as the file changes. The extension sends `document/didChange` on
every change without debouncing; the webview then disables every source step and discards any graph
update already in flight, and fetches a new one once the edits pause.
A step that was just recorded or replayed likewise waits for the graph update that follows it (the
buttons are disabled for that mutation anyway). If the host cannot answer, it reports `null` and no
source step is offered until the next update. Clicking still prepares the edit again against the
current document; if that fails, the step leaves the history and the failure is logged.

## Stage 0 transaction

1. The webview starts a serialized resource-creation mutation with a unique operation ID.
2. The language server returns a proposed single-document, versioned insertion. Before applying
   it, the extension checks the resource-editing opt-in, document version, target URI, and
   single-insertion shape.
3. The extension calls `workspace.applyEdit` once. It checks that the document changed only by the
   insertion before recording the creation's node ID and the **actual inserted text** (after any
   line-ending normalization by the editor). If the source changed but tracking could not be
   verified, the response carries a `historyTrackingError`. The extension logs the tracking
   failure; it does not call a successful source edit a failed creation.
4. The webview records the source step only after successful application and tracking, alongside
   the expected graph node ID and original drop point. Reconciliation mounts the new node, and the
   same graph update confirms the step can be undone.
5. Undo or redo first peeks at the latest designer step. A layout step restores recorded atomic
   positions locally, without a code edit, layout request, or camera change. A confirmed source step
   sends its operation ID and direction to the extension. The extension checks the opt-in, asks the
   language server for the replay edit (`textDocument/prepareVisualResourceReplay`), checks the
   document version did not change meanwhile, applies it, and verifies that only that range changed.
   The webview then advances its history cursor and reconciles the graph.
   Undo discards any still-pending preview and placement even if the graph update failed.
   Redo correlates the returned node with the original graph position and restores a pending
   preview only if the canonical node has not mounted. Undo of an independent top-level resource
   does not lay out or auto-fit its survivors. This exemption survives a delayed graph update
   until the matching removal is observed or redo cancels it; edges and structural changes still
   request layout.

The extension stores each creation's node ID and inserted text, not offsets, hashes, or document
copies, so unrelated edits and designer replays never need to be reconciled with it.

Node movement starts a snapshot at the first actual drag movement and commits once at gesture end.
A module drag records all atomic descendants as one step. Reset Layout records the settled and
server target positions as one step **only after a successful layout**. Settled positions are the
targets of any animation the reset interrupts, so undoing it never stops partway through one. History never records
pan, zoom, focus, chooser opening, cancellation, automatic layout, or animation frames.
Layout undo/redo springs between recorded positions using the graph's existing animation. A
new layout action retargets an in-flight spring from the current visual position; a drag stops
it. Effective reduced motion snaps directly to the target. These visual transitions do not
change the camera, issue a layout request, or add extra history steps.

## Ordering and conflicts

- The graph coordinator serializes source mutations and reconciles the document after both
  successful and failed attempts. While a source action or Reset Layout request is pending,
  Undo, Redo, and Reset Layout are disabled and a canvas shield prevents another node gesture
  from starting.
- A failed edit, disabled setting, or rejected workspace edit leaves the history cursor in place
  and logs an error without an in-canvas notification. A step that fails the language server's
  check when clicked leaves the history and the failure is logged. No unverified inverse is applied.
- Reconciliation removes layout snapshots for nodes that disappear or change kind. A node
  temporarily absent because its **designer creation was undone** retains its redo snapshots;
  replaying creation restores its drop point before later layout steps can be redone.
- A source edit made directly in the Bicep editor, an editor Undo/Redo, another extension, or
  another visualizer panel triggers a graph update like any other change. If it edited, referenced,
  or removed a designer-created declaration (or took the name of an undone one), that update disables
  the step until the edit is reverted; otherwise the step stays available.
- The setting `bicep.visualizer.experimental.enableResourceEditing` gates both creating and
  replaying a resource source edit. With it disabled, viewer layout history remains usable;
  a source step at the top cannot be replayed until editing is enabled again. Module editing
  requires a separately chosen experimental opt-in.

The public `WorkspaceEdit` API has no caller-supplied document-version condition or per-edit undo
token. The extension checks version and text immediately before applying, and verifies the
result afterward; it cannot make a preflight check atomic with an editor change made concurrently
in another window. An unexpected post-edit state is an explicit conflict requiring inspection,
not a reason to guess at a second compensating edit. Do not add native-command forwarding as a
fallback.

## Extending the transaction contract

Future source actions must provide a typed forward change, a corresponding minimal inverse,
the document/file preconditions for both directions, and a graph-identity mapping. Record them
only after a successful forward operation. They must preserve the same conflict policy and
single designer timeline:

| Action                  | Additional requirements before enabling undo                                                                                                                                                                                                                                                                                                           |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Rename symbol           | Obtain LSP rename edits; capture replaced text in every affected document; validate versions/URIs and non-overlapping ranges; compute post-edit inverse ranges after replacements; preview and test cross-file references, edges, node IDs, descendants, selection, and placement.                                                                     |
| Convert to `existing`   | Preview removed deployment properties; retain their exact source text for an inverse; validate scope/name and dependent declarations; preserve node identity across metadata-only changes.                                                                                                                                                             |
| Add a local module file | Treat file creation and parent declaration as one reviewed user action, but do not assume file operations have the same all-or-nothing guarantee as text-only workspace edits. Check file existence and dirty state before either direction, handle partial failures explicitly, and test restoration of the new file and parent declaration together. |
| Extract to Module       | Complete the language-server refactoring first; validate cross-boundary references, parameters/outputs, scope, dependencies, and multi-file edits before exposing a undo history step.                                                                                                                                                                 |

Text-only multi-document edits require a separate implementation and integration tests; the Stage 0
source replay handler intentionally accepts only a single insertion. A future action must not
silently pass its `WorkspaceEdit` through that handler.

## Validation

- Unit tests cover layout snapshot boundaries, mixed chronological history, redo invalidation,
  creation removed/readded across graph reconciliation, and listing and dropping source steps.
- Language server tests cover undo after unrelated edits, blank lines added next to the
  declaration, incomplete declarations, and refusal once the declaration is edited, referenced, or
  removed, or its name is taken for redo.
- Host tests cover the enablement gate, availability reported with graph updates, version drift
  while preparing a replay, failed application, unexpected results, and source undo/redo.
- Playwright tests cover flag-off viewer behavior, slow/cancelled drags, the history-bar
  buttons and canvas shortcuts, and creation/layout undo and redo in one timeline. They use a fake host and do not prove VS
  Code editor behavior.
- Before enabling rename or multi-file replay, add extension-host tests with a real VS Code
  document and file-operation tests. Test manual editor Undo after a designer reversal, another
  editor changing the Bicep document, a closed/reopened file, invalid/readonly documents,
  multiple visualizer panels, multi-file partial failures, and keyboard focus in the webview.

See [Architecture](./architecture.md) for graph reconciliation and
[Roadmap](./roadmap.md) for later editing stages. The VS Code [commands guide](https://code.visualstudio.com/api/extension-guides/command)
and [extension API reference](https://code.visualstudio.com/api/references/vscode-api#WorkspaceEdit)
describe the public primitives; neither provides a document-targeted undo-stack operation.
