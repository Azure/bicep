# Step 6: Achieve Full Feature Parity

## Goal

Fill the remaining gaps between the old Cytoscape.js visualizer and the new visual designer so the new implementation can fully replace the old one. The two outstanding features are the **diagnostic reporting bar** and **click-to-reveal-range** navigation.

## Background

### Feature parity matrix

| Feature | Old Visualizer | New Visual Designer | Status |
|---------|---------------|-------------------|--------|
| Error indicator (red border) | Cytoscape `border-color` keyed on `hasError` | `$hasError` prop → `theme.error` border in `ResourceDeclaration` / `ModuleDeclaration` | **Done** |
| Collection indicator | Text suffix (`[]` / `<collection>`) | Stacked card visual via `::before` pseudo-element | **Done** (improved) |
| Auto-fit on load | Cytoscape `fit: true` | `useAutoLayout()` + `computeFitViewTransform()` | **Done** |
| High-contrast border | Yellow border via theme | 4 theme variants with distinct colors | **Done** |
| Text truncation | `truncate()` to 17/23/37 chars (fixed-width SVG) | Not needed — DOM nodes size dynamically to text content | **N/A** |
| Diagnostic reporting bar | `StatusBar` component showing error count / empty state | Not implemented | **TODO** |
| Click to reveal source range | Double-tap node → `REVEAL_FILE_RANGE` message | Message constant + extension host handler exist, but no click handler on nodes | **TODO** |

### Already-implemented features (details)

- **Error indicator**: Both `ResourceDeclaration` and `ModuleDeclaration` accept `hasError` and conditionally apply `border: 2px solid ${theme.error}`. The error color is theme-aware (red in dark/light, fuchsia in high-contrast).
- **Collection indicator**: Both components accept `isCollection` and render a stacked card effect using a `::before` pseudo-element with offset, background, and matching border. This is a visual improvement over the old text-only suffix — no text changes are needed.
- **Auto-fit on load**: `useAutoLayout()` calls `computeFitViewTransform()` from the ELK layout result and applies the transform before animating nodes to position.
- **Text truncation**: The old visualizer truncated text because nodes were rendered as fixed-width SVG data URIs (220×80 px). The new implementation uses real DOM elements with dynamic widths (via `min-width: 200px` and flexbox), so names of any length are displayed in full without overflow. No truncation logic is needed.

## Tasks

### 6.1 Diagnostic reporting bar

Port the `StatusBar` component from the old visualizer (`src/vscode-bicep/src/visualizer/app/components/StatusBar.tsx`). This shows at the bottom-left of the canvas and serves three purposes:

1. **Error reporting**: When `errorCount > 0`, shows a red indicator circle and a message like _"There are N errors in the file. The rendered graph may not be accurate."_
2. **Empty state**: When `errorCount === 0` and there are no nodes, shows _"There are no resources or modules in the file. Nothing to display."_
3. **No errors + has nodes**: Shows a green indicator circle with no text.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/components/StatusBar.tsx`

```tsx
import { styled } from "styled-components";

interface StatusBarProps {
  errorCount: number;
  hasNodes: boolean;
}

const $StatusBarContainer = styled.div`
  position: absolute;
  height: 32px;
  left: 20px;
  bottom: 20px;
  display: flex;
  flex-direction: row;
  align-items: center;
  z-index: 100;
  font-size: 13px;
  color: ${({ theme }) => theme.text.primary};
`;

const $StatusCircle = styled.div<{ $hasErrors: boolean }>`
  width: 8px;
  height: 8px;
  background-color: ${({ $hasErrors, theme }) =>
    $hasErrors ? theme.error : theme.success};
  border-radius: 50%;
  margin-right: 8px;
`;

export function StatusBar({ errorCount, hasNodes }: StatusBarProps) {
  return (
    <$StatusBarContainer>
      <$StatusCircle $hasErrors={errorCount > 0} />
      {errorCount > 0 && (
        <span>
          There {errorCount === 1 ? "is" : "are"} {errorCount}
          {errorCount === 1 ? " error" : " errors"} in the file.
          The rendered graph may not be accurate.
        </span>
      )}
      {errorCount === 0 && !hasNodes && (
        <span>There are no resources or modules in the file. Nothing to display.</span>
      )}
    </$StatusBarContainer>
  );
}
```

**State management**: Store `errorCount` and `hasNodes` in Jotai atoms, updated by the deployment graph notification handler in `useApplyDeploymentGraph()`:

```typescript
// atoms:
export const errorCountAtom = atom(0);
export const hasNodesAtom = atom(false);

// In the deployment graph callback:
store.set(errorCountAtom, graph?.errorCount ?? 0);
store.set(hasNodesAtom, (graph?.nodes.length ?? 0) > 0);
```

Then consume them in `App.tsx`:

```tsx
const errorCount = useAtomValue(errorCountAtom);
const hasNodes = useAtomValue(hasNodesAtom);
// ...
<StatusBar errorCount={errorCount} hasNodes={hasNodes} />
```

The `StatusBar` is rendered inside `$AppContainer` but **outside** `PanZoomProvider` so it stays fixed relative to the viewport and is not affected by pan/zoom.

### 6.2 Click to reveal ranges

When a user double-clicks a node, the extension should navigate to the corresponding source range in the `.bicep` file.

**What already exists**:
- `REVEAL_FILE_RANGE_NOTIFICATION` and `RevealFileRangePayload` are defined in `src/vscode-bicep-ui/apps/visual-designer/src/messages.ts`.
- The extension host handler in `src/vscode-bicep/src/visualizer-v2/view.ts` already processes the `revealFileRange` notification and calls `revealFileRange()` → `revealEditorRange()`.
- Each node's `dataAtom` already contains `range` and `filePath` (set by `useApplyDeploymentGraph()`).

**What needs to be implemented** (webview side only):

1. **`BaseNode`**: Add an `onDoubleClick` prop and forward it to the underlying `$BaseNode` div.

```tsx
export type BaseNodeProps = PropsWithChildren<{
  zIndex: number;
  onDoubleClick?: () => void;
}>;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(
  ({ zIndex, onDoubleClick, children }, ref) => (
    <$BaseNode ref={ref} $zIndex={zIndex} onDoubleClick={onDoubleClick}>
      {children}
    </$BaseNode>
  ),
);
```

2. **`AtomicNode`**: Read `range` and `filePath` from `dataAtom`, and on double-click send a `REVEAL_FILE_RANGE_NOTIFICATION` via the message channel.

```tsx
const handleDoubleClick = useCallback(() => {
  const data = store.get(dataAtom) as { range?: Range; filePath?: string };
  if (data?.range && data?.filePath) {
    messageChannel.sendNotification({
      method: REVEAL_FILE_RANGE_NOTIFICATION,
      params: { filePath: data.filePath, range: data.range },
    });
  }
}, [store, dataAtom, messageChannel]);

return (
  <BaseNode ref={ref} zIndex={2} onDoubleClick={handleDoubleClick}>
    <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
  </BaseNode>
);
```

3. **`CompoundNode`**: Apply the same pattern.

**Extension host side** (already handled in Step 5): `view.ts` processes the `revealFileRange` notification using the same logic as the old visualizer — check visible editors first, then open the file if not already visible.

## Verification

### Manual testing checklist

1. **Diagnostic reporting bar**:
   - File with errors → red dot + error count message at bottom-left
   - File without errors, with resources → green dot, no text
   - Empty file (no resources/modules) → message: "There are no resources or modules in the file. Nothing to display."
   - Error count updates dynamically as errors are introduced/fixed
   - Bar is visible in all 4 themes (dark, light, high-contrast, high-contrast-light)
   - Bar stays fixed in the viewport (not affected by pan/zoom)

2. **Click to reveal source range**:
   - Double-click a resource node → editor reveals the source range
   - Double-click a module node → editor reveals the module declaration
   - Works when the source file is already open in another editor column
   - Works when the source file is not open (should open it)
   - Cursor is positioned at the correct line/character

3. **Compare with old visualizer**:
   - Open the same `.bicep` file in both old and new visualizers
   - Verify visual parity for error indicators, collection indicators, auto-fit, status bar, and click-to-reveal

## Notes

- The `revealFileRange` on the extension host side is already fully implemented in `src/vscode-bicep/src/visualizer-v2/view.ts`. It follows the same logic as the old visualizer: check visible editors first, then open the file if not already visible.
- The `range` data from the language server uses 0-based line/character numbers (LSP convention). The `vscode.Range` constructor also uses 0-based. No conversion needed.
- **Keyboard accessibility** (e.g., `tabIndex={0}` on nodes, Enter/Space to activate, ARIA roles) was not present in the old visualizer and is not required for feature parity. It can be addressed as a follow-up enhancement.
