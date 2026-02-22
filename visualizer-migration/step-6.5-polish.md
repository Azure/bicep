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

### 6.5.2 — Z-Index Elevation on Hover

**Current state:** Static 3-tier z-index: compound nodes = 0, inner edges = 1, atomic nodes = 2. When nodes overlap at boundaries, there is no way to bring one forward.

**Changes:**
- On hover, temporarily raise the node's `zIndex` above all others (e.g., `zIndex: 100`).
- Use CSS `:hover` pseudo-class or React state + inline style.
- Ensure the z-index resets when the pointer leaves.
- Consider adding a subtle `transition: z-index` or `transition: box-shadow` for smoothness.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/BaseNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/AtomicNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/CompoundNode.tsx`

---

### 6.5.3 — Module (Compound Node) Transparency

**Current state:** Compound nodes use `theme.background` (fully opaque). The nested structure can feel visually heavy because inner nodes sit on a solid background.

**Changes:**
- Make the compound node's body background semi-transparent (e.g., `rgba(...)` or `opacity` on the background only).
- The header bar should remain fully opaque for readability.
- Ensure the effect works across all four themes — use a new theme token like `theme.compoundBackground` with alpha, or apply `opacity` to the CSS background-color.
- Test with high-contrast themes to ensure WCAG contrast ratios are maintained.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/CompoundNode.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/themes.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/styled.d.ts`

---

### 6.5.4 — Cursor Improvements

**Current state:** The custom SVG circle cursor only appears on `:active` (mousedown) on the pan-zoom canvas. Nodes use `cursor: default`.

**Changes:**
- **Canvas:** Change to `cursor: grab` on idle and `cursor: grabbing` on drag for the pan-zoom container.
- **Nodes:** Use `cursor: pointer` to indicate interactivity (double-click to reveal).
- **Edges:** Keep `pointer-events: none` — no cursor change needed.
- Remove the custom SVG circle cursor from the pan-zoom `:active` state (or keep it as an option via a CSS variable if the glassmorphism effect is desired later).

**Files:**
- `src/vscode-bicep-ui/packages/components/src/pan-zoom/PanZoom.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/BaseNode.tsx`

---

### 6.5.5 — Clickable Status Bar Error Text

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

### 6.5.6 — Drag Elevation Effect

**Current state:** Nodes cannot be dragged (no drag handler). If drag support is added later, nodes should visually lift during drag.

**Changes (prep only — no drag implementation):**
- Document the pattern: when a node is being dragged, apply `box-shadow` elevation and raise `zIndex`.
- If drag is not in scope, this task is **deferred**. Mark as N/A if drag won't be added.

**Status:** Deferred — depends on drag support being added to the graph engine.

---

### 6.5.7 — Edge Hover Animation

**Current state:** Edges are static SVG paths with `pointer-events: none`. They use `stroke-dasharray` animation only during layout transitions.

**Changes:**
- Add a subtle color or width change on edge hover.
- Enable `pointer-events: visibleStroke` on the edge `<path>` elements.
- On hover, increase `stroke-width` slightly or change `stroke` color to a highlight color.
- Consider showing a tooltip with the source → target relationship.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/Edge.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/themes.ts`

---

## Priority Order

1. **6.5.1** Focus System — highest impact, foundational for all other interactive polish
2. **6.5.4** Cursor Improvements — quick win, improves perceived quality immediately
3. **6.5.2** Z-Index Elevation — builds on focus system
4. **6.5.3** Module Transparency — visual depth improvement
5. **6.5.5** Clickable Status Bar — functional improvement
6. **6.5.7** Edge Hover Animation — nice-to-have
7. **6.5.6** Drag Elevation — deferred
