# Step 6.5: Visual Polish & Interaction Refinements

## Goal

Add interactive feedback, visual depth, and polish to the visual designer before removing the old visualizer. These refinements make the graph feel responsive and professional.

## Tasks

### 6.5.1 — Hover Border Accent ✅

**Current state:** Nodes have no visual feedback on hover. `BaseNode` sets `cursor: default` and has no `:hover` styles.

**Changes:**
- Add `:hover` style to `BaseNode` — change border color to an accent blue.
- Keep `cursor: default` — do not change cursor on hover or click.
- No `:active`, `:focus-visible`, or shadow effects.

**Theme integration:** Add a `theme.node.hoverBorder` token in `styled.d.ts` and populate it across all four theme variants with an appropriate accent blue.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ResourceDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ModuleDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/theming/themes.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/styled.d.ts`

---

### 6.5.2 — Node Focus & Z-Index Elevation ✅

**Current state:** Static 3-tier z-index: compound nodes = 0, inner edges = 1, atomic nodes = 2. When nodes overlap at boundaries, there is no way to bring one forward. No visual distinction between a selected/focused node and others.

**Behavior:**

1. **Focus on mousedown (not hover).** Clicking a node makes it the focused node. Do not change z-index on hover.
2. **Focused node appearance.** The focused node gets a thicker border and the highest z-index. In dark theme, also lighten the border color to improve visibility. Add a new `focusedNodeAtom` (Jotai atom) to track which node ID is currently focused — clear it on canvas click.
3. **Sibling elevation.** Siblings of the focused node (other children of the same parent module) get the second-highest z-index tier.
4. **Parent module elevation.** The immediate parent module of the focused node gets a z-index higher than all other nodes except the focused node and its siblings.
5. **Recursive ancestor elevation.** The rule from (4) applies recursively up the ancestor chain — each grandparent module gets a z-index one tier below its child module, but still above unrelated nodes.
6. **Atom design.** This requires careful atom design:
   - `focusedNodeIdAtom` — stores the currently focused node ID (or `null`).
   - Derived atoms or selectors that, given a node ID, compute its effective z-index tier based on its relationship to the focused node (self → sibling → parent → grandparent → … → default).
   - The graph-engine layer should expose the parent-child hierarchy so the visualization layer can compute ancestor chains.

**Theme integration:** Add `theme.node.focusBorderWidth` and `theme.node.focusBorder` tokens (or reuse/extend existing hover tokens) for the focused state border styling.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/BaseNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/AtomicNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/CompoundNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ResourceDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ModuleDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/theming/themes.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/styled.d.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/atoms/nodes.ts`

---

### 6.5.3 — Clickable Status Bar Error Text

**Current state:** The `StatusBar` shows error count + "Show errors" text, but clicking does nothing.

**Changes:**
- Make the error count portion of the status bar clickable.
- On click, send a `SHOW_PROBLEMS_PANEL_NOTIFICATION` (new notification type) to the extension host.
- In the extension host's webview panel, handle the notification by calling `vscode.commands.executeCommand("workbench.actions.view.problems")`.
- Style the clickable area with `cursor: pointer` and a subtle underline or hover effect.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/components/StatusBar.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/messages.ts`
- `src/vscode-bicep/src/visualizer/` (extension host handler)
- `src/vscode-bicep-ui/apps/visual-designer/src/features/devtools/fake-message-channel.ts`

---

## Priority Order

1. **6.5.1** Hover Border Accent — highest impact, foundational for all other interactive polish
2. **6.5.2** Node Focus & Z-Index Elevation — builds on hover system, adds selection model
3. **6.5.3** Clickable Status Bar — functional improvement
