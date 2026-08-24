# Implementation Plan: Visual Designer Resource Creation

## Overview

Implement the initial WYSIWYG resource-creation workflow described in
[the visual designer resource creation design](../../src/vscode-bicep-ui/apps/visual-designer/resource-creation-design.md).
Users will drag an Azure resource type from the floating Resource Palette onto the canvas, producing a
versioned Bicep source edit and a canonical graph node at the drop point without moving existing nodes.

## Prerequisites

- Work on branch `visual-designer-resource-creation` in `D:\bicep-resource-creation`.
- Keep Bicep source as the only durable source of truth.
- Reuse the existing visual graph request loop, webview messaging package, compilation manager, resource type
  providers, syntax factory, rewriters, formatter, and VS Code `WorkspaceEdit` support.
- Follow existing request-handler registration in `Bicep.LangServer/Server.cs` and protocol mirroring in
  `vscode-bicep/src/features/visualization/protocol.ts`.
- Preserve existing visualizer behavior for ordinary source edits and explicit Reset Layout.

## Implementation Steps

### Step 1: Add language-server resource creation foundations [Complete]

- **Files to create**:
  - `src/Bicep.LangServer/Features/Custom/Visualization/VisualResourceCreationProtocol.cs`
  - `src/Bicep.LangServer/Features/Custom/Visualization/VisualResourceCreationHandler.cs`
  - `src/Bicep.LangServer/Features/Custom/Visualization/VisualResourceCreationService.cs`
  - `src/Bicep.LangServer.UnitTests/Features/Visualization/VisualResourceCreationServiceTests.cs`
  - `src/Bicep.LangServer.IntegrationTests/Features/Visualization/VisualResourceCreationTests.cs`
- **Files to modify**:
  - `src/Bicep.LangServer/IServiceCollectionExtensions.cs`
  - `src/Bicep.LangServer/Server.cs`
- **Changes**:
  - Define typed resource catalog and prepare-resource LSP contracts.
  - Resolve resource types against the active document namespace provider.
  - Implement deterministic, valid, case-insensitive symbolic-name generation with numeric suffixes.
  - Generate a top-level resource declaration through Bicep syntax and formatting APIs.
  - Return a versioned `WorkspaceEdit`, expected node ID, symbolic name, and unresolved required properties.
  - Register the handler and reusable service.
- **Validation**:
  - Run targeted unit tests for naming and source generation.
  - Run integration tests for catalog lookup, valid edit generation, stale/invalid type failures, and expected node ID.

### Step 2: Add the extension protocol and edit bridge [Complete]

- **Files to modify**:
  - `src/vscode-bicep/src/features/visualization/protocol.ts`
  - `src/vscode-bicep/src/features/visualization/visualizer-view.ts`
  - `src/vscode-bicep/src/features/visualization/visualizer-view-manager.ts`
  - `src/vscode-bicep/src/features/visualization/commands.ts`
  - `src/vscode-bicep/src/features/visualization/activation.ts`
  - `src/vscode-bicep/vite.config.mts`
  - `src/vscode-bicep/package.json`
- **Files to create**:
  - `src/vscode-bicep/src/features/visualization/resource-palette.ts`
- **Files to create or modify for tests**:
  - Visualizer unit tests under `src/vscode-bicep/src/features/visualization/__tests__`
- **Changes**:
  - Mirror the new LSP records in TypeScript.
  - Load the catalog for the visualizer's bound Bicep document.
  - Transform the typed paged LSP catalog into provider/type groups for the visual designer.
  - Add typed webview request handling for resource creation.
  - Bind requests to the visualizer document and current version.
  - Apply the returned versioned workspace edit and reply only after VS Code reports success.
  - Map expected failures to stable webview error codes and log technical details.
  - Notify visualizers directly from text document changes instead of depending only on diagnostics.
  - Add `bicep.visualizer.openPositioning` with `full`, `left`, and `right`.
- **Validation**:
  - Run targeted VS Code extension unit tests and TypeScript build.

### Step 3: Add floating island and local pointer dragging [Complete]

- **Files to modify**:
  - `src/vscode-bicep-ui/apps/visual-designer/src/App.tsx`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/messages.ts`
- **Files to create**:
  - `src/vscode-bicep-ui/apps/visual-designer/src/features/resource-palette/*`
- **Changes**:
  - Render a visual-designer-local Resource Palette as a collapsible floating island without resizing the canvas.
  - Use pointer capture, Escape cancellation, and a cursor-locked Motion overlay.
  - Keep preview and pending-node centers aligned and avoid a partial cross-component morph.
  - Add canvas target resolution and viewport-to-graph coordinate conversion.
  - Render a pending node immediately at the drop point.
  - Keep the independently runnable Resource Type Explorer app unchanged; it is not a product dependency of the visual designer.
- **Validation**:
  - Add Vitest coverage for payload validation, coordinate conversion, and pending UI.
  - Run the visual designer targeted test suite and build.

### Step 4: Coordinate source mutation and canonical graph reconciliation [Complete]

- **Files to modify**:
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/use-graph-update.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/use-visual-graph.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/messaging/layout-invalidation.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/src/lib/graph/atoms/nodes.ts`
  - Resource creation feature files from Step 3
- **Changes**:
  - Serialize resource mutations per visualizer document.
  - Correlate operation ID, expected canonical node ID, and drop origin.
  - Keep pending nodes out of the canonical graph submitted to the language server.
  - Add a one-shot origin override for the matching canonical node.
  - Suppress layout and fit-view only for the correlated `addNode` patch.
  - Preserve layout invalidation for unrelated concurrent topology changes.
  - Handle applied-edit reconciliation delays without falsely reporting source failure.
- **Validation**:
  - Extend graph update and layout invalidation tests.
  - Verify surviving node boxes, pan/zoom, and focus remain unchanged.
  - Verify no `getGraphLayout` request occurs for a correlated creation.

### Step 5: Add end-to-end coverage and harden error handling [Complete]

- **Files to modify**:
  - `src/vscode-bicep-ui/apps/visual-designer/e2e/fixtures.ts`
  - `src/vscode-bicep-ui/apps/visual-designer/e2e/node-interactions.spec.ts`
  - Resource creation feature tests
- **Changes**:
  - Cover drops at default and transformed viewports.
  - Cover duplicate symbolic names, edit rejection, invalid payloads, unresolved required properties, and explicit Reset Layout.
  - Confirm native source undo removes the canonical node.
  - Keep preview and pending cards visually identical to avoid flashing during fast reconciliation; reserve the error surface for failures.
- **Validation**:
  - Run visual designer Playwright tests.
  - Run targeted language-server integration and extension tests.

### Step 6: Final verification and documentation alignment [Complete]

- **Files to modify**:
  - `src/vscode-bicep-ui/apps/visual-designer/resource-creation-design.md` only if implementation decisions change.
- **Changes**:
  - Inspect all generated source and baseline diffs.
  - Confirm protocol names and ownership boundaries match the design.
  - Remove temporary adapters or dead standalone explorer code after parity.
- **Validation**:
  - Run targeted .NET builds/tests for changed projects.
  - Run `npm run lint`, `npm run build`, and targeted tests in `src/vscode-bicep-ui`.
  - Run targeted lint/build/tests in `src/vscode-bicep`.
  - Run `git diff --check` and inspect the complete branch diff.

## Testing Plan

### Unit tests

- Symbolic-name derivation, identifier sanitization, case-insensitive collisions, and suffix selection.
- Resource type validation and catalog paging/filtering.
- Syntax generation and formatting without sample values or snippet tab stops.
- Pointer activation, cancellation, animated overlay behavior, and coordinate conversion.
- Mutation queue state transitions and one-shot placement overrides.
- Correlated versus unrelated graph layout invalidation.

### Integration tests

- Language server returns a versioned edit that inserts parser-valid Bicep.
- Expected node ID matches the visual graph node produced after applying the edit.
- Extension applies the edit to the bound document and maps stale/read-only failures.
- Document changes converge through the existing graph patch loop.

### End-to-end tests

- Drag supported resource types at default and transformed viewport coordinates.
- Existing nodes retain exact positions and the viewport does not fit or recenter.
- Duplicate resource types receive numeric symbolic-name suffixes.
- Pending state resolves to the canonical node or actionable failure.
- Explicit Reset Layout remains functional.

### Manual verification

- Open a real Bicep file, open/close the floating island, create several resources, edit and undo source, and verify synchronization.
- Exercise a large template and catalog filter for interaction responsiveness.
- Verify keyboard/focus behavior and accessible status announcements.

## Rollback Plan

- Revert implementation commits in reverse order.
- The feature introduces no persisted state or schema migration.
- Existing visual graph update/layout methods remain backward compatible until the new workflow is complete.
- If rollout gating is added, disable the resource creation surface while retaining the read-only visualizer.

## Success Criteria

- A supported Azure resource type can be dragged onto the visual designer.
- A parser-valid declaration is inserted through a versioned native workspace edit.
- The symbolic name is deterministic and collision-free.
- The canonical node is centered at the exact graph-space drop point.
- Existing node positions, pan, zoom, and focus remain unchanged.
- No automatic layout or fit-view occurs for the correlated addition.
- Source edits, undo, and external changes converge through the canonical graph.
- Failures do not leave an uncommitted canonical node or report false source success.
- Typed contracts enforce the UI, extension, and language-server ownership boundaries.
- Targeted .NET, TypeScript, UI, extension, and end-to-end tests pass.
