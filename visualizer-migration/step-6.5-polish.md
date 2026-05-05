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

### 6.5.3 — Clickable Status Bar Error Text ✅

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

### 6.5.4 — Smooth Graph Initialization & Flicker-Free Updates ✅

#### Problem A: First-render nodes flash at the top-left corner

**Current state:** When the visual-designer app launches and the first `DeploymentGraph` message arrives, every node's `boxAtom` is initialized at a default origin. After ELK layout completes (async — lazy-loads ~1.5 MB ELK engine + computes layout), `computeFitViewTransform` adjusts the viewport and `applyLayout` spring-animates nodes to their final positions. Because the pan-zoom viewport starts with an identity transform (`translate(0,0) scale(1)`), any node position maps directly to screen coordinates — so nodes at `(0, 0)` appear at the **top-left corner** of the screen for several frames before the async layout completes.

**Root cause:** The real issue is not the origin coordinates but the **timing gap** between DOM rendering (immediate) and ELK layout (async). Nodes are rendered and visible in the DOM before the viewport transform is computed, so there is always a flash frame regardless of what initial position is used.

**Fix — viewport-center origin + visibility gate:**

The fix is generic for all empty→non-empty transitions (first render, switching from the Empty graph, etc.):

1. **`useApplyDeploymentGraph` accepts a `getViewportCenter` callback.** When the graph was previously empty (no existing node positions), the default origin for all new nodes is `getViewportCenter()` — i.e. `{ x: viewportWidth/2, y: viewportHeight/2 }`. This means nodes are created at the center of the canvas in graph coordinates. When the graph already has nodes, the centroid of existing positions is used (unchanged).

2. **`layoutReadyAtom` visibility gate.** The `Graph` component renders with `opacity: 0` while `layoutReadyAtom` is `false`. When the graph becomes empty, `layoutReadyAtom` is reset to `false` — so the next non-empty graph will go through the reveal sequence again.

3. **`useAutoLayout` reveal sequence.** When `layoutReadyAtom` is `false` (empty→non-empty transition):
   - Compute ELK layout (async).
   - Apply `computeFitViewTransform` viewport transform (so the graph center maps to screen center).
   - Wait one `requestAnimationFrame` for the DOM to paint the centered positions + viewport transform.
   - Set `layoutReadyAtom = true` — graph becomes instantly visible with all nodes stacked at the center.
   - Spring-animate nodes from center outward to their final ELK positions (600ms).

4. **`App.tsx` / `GraphContainer`** creates the `getViewportCenter` callback from `useGetPanZoomDimensions` and passes it to `useApplyDeploymentGraph`.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/atoms/graph.ts` — added `layoutReadyAtom`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/layout/hooks/use-auto-layout.ts` — generic reveal for any empty→non-empty transition
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/Graph.tsx` — read `layoutReadyAtom`, control `opacity`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/messaging/hooks/use-deployment-graph.ts` — accept `getViewportCenter`, use as default origin, reset `layoutReadyAtom` on empty
- `src/vscode-bicep-ui/apps/visual-designer/src/App.tsx` — pass `getViewportCenter` to `useApplyDeploymentGraph`

---

#### Problem B: Flickering on full graph rebuild (structural changes)

**Current state:** When the Bicep file undergoes a structural change (resources added/removed), `useApplyDeploymentGraph` previously tore down and rebuilt the entire atom graph:

```ts
setNodesByIdAtom({});   // ← cleared all nodes
setEdgesAtom([]);       // ← cleared all edges
```

Then it re-added nodes one by one. The `NodeLayer` component (which subscribes to `nodesByIdAtom`) could briefly render with an empty record before the new nodes were added back, causing a visible flicker.

**Note:** Trivial edits (blank lines, range-only changes) already bypass this path via the `isDeploymentGraphEqual` short-circuit that updates `dataAtom` values in-place. The flicker only affected structural changes.

**Fix — diff-and-patch instead of clear-and-rebuild:**

Replaced the clear-then-rebuild with a 6-phase diff-and-patch that preserves surviving node/edge atoms:

1. **Remove deleted nodes** — compute the set difference between current and incoming node IDs, then remove stale nodes via `removeNodesAtom` (new write atom added to `nodes.ts`).
2. **Compute default origin** for brand-new nodes — centroid of previous positions (or `(0, 0)` on first render).
3. **Update surviving atomic nodes in-place** — update `dataAtom` with new metadata. If a node's kind changed (compound → atomic), remove and re-add it.
4. **Update surviving compound nodes in-place** — update `childIdsAtom` and `dataAtom`. If kind changed (atomic → compound), remove and re-add.
5. **Diff edges** — compare edge IDs; only rebuild the edge list if the set actually changed.
6. **Bump `graphVersionAtom`** — unchanged, triggers ELK layout.

Also removed the duplicate-ID throw in `addAtomicNodeAtom` since the diff logic now prevents duplicate additions.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/features/messaging/hooks/use-deployment-graph.ts` — replaced clear-and-rebuild with diff-and-patch
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/atoms/nodes.ts` — added `removeNodesAtom`, removed duplicate-ID throw

---

### 6.5.5 — Visual Styling Refinement ✅

**Current state:** Node cards use a flat 2px solid border with 4px border-radius, no shadows, and hard-coded styling. Edges are 2px thick with tight-angle arrowheads. The overall look feels more like a wireframe than a production-quality developer tool.

**Changes:**

**Theme system expansion:** Added new tokens to `styled.d.ts` and all four theme variants in `themes.ts`:
- `borderWidth` / `errorBorderWidth` — 1px default, 1.5px for error nodes in light/dark; 2px for both in HC themes.
- `shadow` / `hoverShadow` / `hoverErrorShadow` — subtle elevation shadows (light/dark), `none` for HC.
- `selectedShadow` / `selectedErrorShadow` — accent-colored focus ring (blue for normal, error-colored for error nodes).
- `accentBorder` / `moduleAccent` — accent colors for resource and module nodes.
- `collectionOffset` — pixel offset for collection stack pseudo-element (6px light/dark, 10px HC).

**Node card restyling:**
- Border-radius from 4px → 8px; collection pseudo-element border-radius 10px for smooth wrapping.
- Resource nodes: height 70→76px, padding 12/16→14/20px, icon 32→36px, name font 18px/500→15px/600, type label 12px/500 uppercase with letter-spacing.
- Module nodes: padding 12px→12/16px, icon 20→24px, name font 14px→15px/600.
- Error nodes get thicker borders (`errorBorderWidth`) and error-colored focus rings (`selectedErrorShadow`).
- Shadows for depth in light/dark themes; clean border-only in HC.

**Edge refinement:**
- Stroke width from 2→1.5px with `transition: stroke 180ms ease`.
- Arrowhead widened: chevron points changed to `"1.5,1 5,5 1.5,9"` in a 6×10 viewBox.
- Shared `<EdgeMarkerDefs>` rendered once in `Graph.tsx` instead of per edge layer (prevents duplicate marker IDs).

**Global & control styling:**
- `GlobalStyle`: added `-webkit-font-smoothing: antialiased`, `line-height: 1.5`, `box-sizing: border-box` reset.
- `GraphControlBar`: border-radius 6→8px, softer shadow, blur 8→12px, button radius 4→6px, transition refined.
- `StatusBar`: font-size 13→12px, weight 500, secondary color, status circle 8→7px, error link underline-offset.

**Theme palette refresh:** All four themes (light, dark, high-contrast, high-contrast-light) received curated colors targeting a calm, low-contrast SaaS aesthetic for light/dark and full accessibility compliance for HC variants.

**Files:**
- `src/vscode-bicep-ui/apps/visual-designer/src/styled.d.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/theming/themes.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/GlobalStyle.ts`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ResourceDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/visualization/components/ModuleDeclaration.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/EdgeMarkerDefs.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/StraightEdge.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/Graph.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/components/EdgeLayer.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/components/GraphControlBar.tsx`
- `src/vscode-bicep-ui/apps/visual-designer/src/components/StatusBar.tsx`

---

## Priority Order

1. **6.5.1** Hover Border Accent — highest impact, foundational for all other interactive polish
2. **6.5.2** Node Focus & Z-Index Elevation — builds on hover system, adds selection model
3. **6.5.3** Clickable Status Bar — functional improvement
4. **6.5.4** Smooth Graph Initialization & Flicker-Free Updates — Problem A is trivial (one-line fix); Problem B is medium effort but only impacts structural edits
5. **6.5.5** Visual Styling Refinement — comprehensive theme/styling polish pass
