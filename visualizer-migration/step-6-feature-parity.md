# Step 6: Achieve Full Feature Parity

## Goal

Fill all remaining gaps between the old Cytoscape.js visualizer and the new visual designer so that the new implementation can fully replace the old one. This includes double-click-to-source navigation, status bar, error indicators, collection indicators, empty state, and auto-fit on load.

## Background

### Features in the old visualizer not yet in the new one

| Feature | Old Visualizer | New Visual Designer |
|---------|---------------|-------------------|
| Double-click to source | Double-tap node → opens editor at source range | Not implemented |
| Status bar | Shows error count or "nothing to display" at bottom-left | Not implemented |
| Collection indicator | `isCollection` appends `[]` to type name or `<collection>` to module name | Not implemented |
| Error indicator | Red border on nodes with `hasError: true` | Not implemented |
| Auto-fit on load | Fits graph to viewport after layout completes | Not implemented |
| Empty state | Shows "no resources or modules" when graph is empty | Not implemented |
| High-contrast border | Yellow borders in HC theme | Handled by CSS variables (Step 2), but `hasError` override needs work |

## Tasks

### 6.1 Double-click to reveal source

When a user double-clicks (or double-taps on touch) a node, the extension should navigate to the source range in the `.bicep` file.

**Webview side**: Add a double-click handler to `AtomicNode` and `CompoundNode` (or to the `BaseNode` wrapper).

**Option A — Event handler on BaseNode**:

Add an `onDoubleClick` prop to `BaseNode`:

```tsx
// BaseNode.tsx
export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(
  ({ zIndex, onDoubleClick, children }, ref) => (
    <$BaseNode ref={ref} $zIndex={zIndex} onDoubleClick={onDoubleClick}>
      {children}
    </$BaseNode>
  ),
);
```

**Option B — Event handler via NodeContent**:

Add the handler in `NodeContent` or a wrapper component that reads `dataAtom` and sends the notification.

**Implementation in `AtomicNode.tsx`**:

```tsx
import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { REVEAL_FILE_RANGE_NOTIFICATION } from "../../../messages";

export function AtomicNode({ id, originAtom, boxAtom, dataAtom }: AtomicNodeState) {
  const store = useStore();
  const messageChannel = useWebviewMessageChannel();

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
    <BaseNode ref={ref} zIndex={1} onDoubleClick={handleDoubleClick}>
      <NodeContent id={id} kind="atomic" dataAtom={dataAtom} />
    </BaseNode>
  );
}
```

Apply the same pattern to `CompoundNode`.

**Extension host side** (already handled in Step 5): The `view.ts` `handleDidReceiveMessage` processes the `revealFileRange` notification and calls `revealFileRange()`.

**Accessibility**: Also add keyboard support — when a node is focused (`tabIndex={0}`), pressing `Enter` or `Space` should trigger the same navigation.

### 6.2 Status bar

Port the `StatusBar` component from the old visualizer. This shows at the bottom-left of the canvas:

- **Errors**: A red circle + "There are N errors in the file. The rendered graph may not be accurate."
- **No errors**: A green circle (no text)
- **Empty graph**: "There are no resources or modules in the file. Nothing to display."

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/features/design-view/components/StatusBar.tsx`

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
  color: var(--vscode-foreground, #333);
  font-size: 13px;
`;

const $StatusCircle = styled.div<{ $hasErrors: boolean }>`
  width: 8px;
  height: 8px;
  background-color: ${({ $hasErrors }) =>
    $hasErrors
      ? "var(--vscode-errorForeground, red)"
      : "var(--vscode-testing-iconPassed, green)"};
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

**State management**: Store `errorCount` in a Jotai atom or pass it via the deployment graph hook:

```typescript
// In App.tsx or a context:
const [graphMeta, setGraphMeta] = useState({ errorCount: 0, hasNodes: false });

// In the deployment graph notification handler:
setGraphMeta({
  errorCount: payload.deploymentGraph?.errorCount ?? 0,
  hasNodes: (payload.deploymentGraph?.nodes.length ?? 0) > 0,
});

// In the render:
<StatusBar errorCount={graphMeta.errorCount} hasNodes={graphMeta.hasNodes} />
```

### 6.3 Error indicator on nodes

Nodes with `hasError: true` should have a visual error state. The old visualizer uses a red border (theme's `errorIndicatorColor`).

**In `ResourceDeclaration.tsx`**:

```tsx
interface ResourceDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    resourceType: string;
    isCollection?: boolean;
    hasError?: boolean;
    // ...
  };
}

const $ResourceDeclaration = styled.div<{ $hasError?: boolean }>`
  /* ... */
  border: 2px solid ${({ $hasError }) =>
    $hasError
      ? "var(--vscode-errorForeground, red)"
      : "var(--vscode-editorWidget-border, #333638)"};
`;

export function ResourceDeclaration({ data }: ResourceDeclarationProps) {
  return (
    <$ResourceDeclaration $hasError={data.hasError}>
      {/* ... */}
    </$ResourceDeclaration>
  );
}
```

Apply the same pattern to `ModuleDeclaration.tsx`.

### 6.4 Collection indicator

Resources and modules declared with `for` loops are collections. The old visualizer appends:
- `[]` to the type name for childless resources (e.g., `virtualMachines[]`)
- `<collection>` to the module name for container modules

**In `ResourceDeclaration.tsx`**:

```tsx
export function ResourceDeclaration({ data }: ResourceDeclarationProps) {
  const { symbolicName, resourceType, isCollection } = data;
  const resourceTypeDisplayName = camelCaseToWords(resourceType.split("/").pop());
  const displayType = isCollection ? `${resourceTypeDisplayName}[]` : resourceTypeDisplayName;

  return (
    <$ResourceDeclaration $hasError={data.hasError}>
      <AzureIcon resourceType={resourceType} size={36} />
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{displayType}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceDeclaration>
  );
}
```

**In `ModuleDeclaration.tsx`**:

```tsx
export function ModuleDeclaration({ data }: ModuleDeclarationProps) {
  const { symbolicName, isCollection } = data;
  const displayName = isCollection ? `${symbolicName} <collection>` : symbolicName;

  return (
    <$ModuleDeclaration $hasError={data.hasError}>
      <$DeclarationInfo>
        <AzureIcon resourceType={"folder"} size={24} />
        <$SymbolicNameContainer>{displayName}</$SymbolicNameContainer>
      </$DeclarationInfo>
    </$ModuleDeclaration>
  );
}
```

### 6.5 Auto-fit on initial load

After ELK layout completes and nodes animate to their positions, auto-fit the graph to the viewport. The `GraphControlBar` already has `fitView` logic — extract it into a reusable utility.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/features/graph-engine/layout/fit-view.ts`

```typescript
import { getDefaultStore } from "jotai";
import { nodesAtom } from "../atoms";

export function computeFitTransform(
  store: ReturnType<typeof getDefaultStore>,
  viewportWidth: number,
  viewportHeight: number,
  padding = 100,
): { x: number; y: number; scale: number } | null {
  const nodes = store.get(nodesAtom);
  const nodeList = Object.values(nodes);
  if (nodeList.length === 0) return null;

  const boxes = nodeList.map((node) => store.get(node.boxAtom));
  const minX = Math.min(...boxes.map((b) => b.min.x));
  const minY = Math.min(...boxes.map((b) => b.min.y));
  const maxX = Math.max(...boxes.map((b) => b.max.x));
  const maxY = Math.max(...boxes.map((b) => b.max.y));

  const graphWidth = maxX - minX;
  const graphHeight = maxY - minY;
  const centerX = minX + graphWidth / 2;
  const centerY = minY + graphHeight / 2;

  const scaleX = (viewportWidth - padding * 2) / graphWidth;
  const scaleY = (viewportHeight - padding * 2) / graphHeight;
  const scale = Math.min(scaleX, scaleY, 1);

  return {
    x: viewportWidth / 2 - centerX * scale,
    y: viewportHeight / 2 - centerY * scale,
    scale,
  };
}
```

Call this after layout completes (with a delay for the spring animation):

```typescript
// After runLayout():
setTimeout(() => {
  const fit = computeFitTransform(store, viewportWidth, viewportHeight);
  if (fit) {
    transform(fit.x, fit.y, fit.scale);
  }
}, 700); // Wait for spring animation (600ms duration + buffer)
```

### 6.6 Empty state handling

When the deployment graph is `null` or has no nodes, show an appropriate message instead of an empty canvas.

**In `App.tsx`**:

```tsx
{graphMeta.hasNodes ? (
  <PanZoomProvider>
    <$ControlBarContainer>
      <GraphControlBar />
    </$ControlBarContainer>
    <Canvas>
      <Graph />
    </Canvas>
  </PanZoomProvider>
) : null}
<StatusBar errorCount={graphMeta.errorCount} hasNodes={graphMeta.hasNodes} />
```

The `StatusBar` already handles the empty state message ("There are no resources or modules in the file. Nothing to display."), which matches the old visualizer behavior.

### 6.7 Text truncation

The old visualizer truncates text to prevent overflow:
- Symbolic name: truncated to 17 characters
- Resource type: truncated to 23 characters

The new components use real DOM text that can overflow. Add CSS text overflow handling:

```css
overflow: hidden;
text-overflow: ellipsis;
white-space: nowrap;
max-width: /* appropriate value */;
```

Or use the old `truncate` function:

```typescript
function truncate(text: string, lengthLimit: number): string {
  if (text.length <= lengthLimit) return text;
  const charsLength = lengthLimit - 3;
  const headLength = Math.ceil(charsLength / 2);
  const tailLength = Math.floor(charsLength / 2);
  return text.substr(0, headLength) + "..." + text.substr(text.length - tailLength);
}
```

CSS overflow is preferred since it's responsive and shows the full text on resize.

### 6.8 Keyboard accessibility

Add keyboard navigation:

1. **Tab order**: Nodes should be in tab order (`tabIndex={0}` on `BaseNode`)
2. **Enter/Space**: Activate node (same as double-click → reveal source)
3. **Arrow keys** (optional): Navigate between connected nodes
4. **Focus visible**: Use `&:focus-visible` outline using `var(--vscode-focusBorder)`

```tsx
// BaseNode.tsx
const $BaseNode = styled.div<{ $zIndex: number }>`
  /* ... existing styles ... */
  &:focus-visible {
    outline: 2px solid var(--vscode-focusBorder, #3182ce);
    outline-offset: 2px;
  }
`;

<$BaseNode
  ref={ref}
  $zIndex={zIndex}
  tabIndex={0}
  role="button"
  aria-label={/* node description */}
  onDoubleClick={onDoubleClick}
  onKeyDown={(e) => {
    if (e.key === "Enter" || e.key === " ") {
      onDoubleClick?.();
    }
  }}
>
```

### 6.9 ARIA roles for graph structure

Add ARIA attributes for screen readers:

```tsx
// Graph container
<div role="img" aria-label="Bicep deployment graph">

// Node
<div role="button" aria-label={`${symbolicName} (${resourceType})`}>

// Edge layer
<svg role="presentation" aria-hidden="true">
```

## Verification

### Manual testing checklist

1. **Double-click navigation**:
   - Open a `.bicep` file with multiple resources
   - Open the visualizer
   - Double-click a resource node → editor opens at the correct line
   - Double-click a module node → editor opens at the module declaration
   - Works when the source file is already open in another column
   - Works when the source file is not open (should open it)

2. **Status bar**:
   - File with errors → shows red dot + error count message
   - File without errors → shows green dot, no text
   - Empty file (no resources) → shows "Nothing to display" message
   - Error count updates as errors are introduced/fixed

3. **Error indicator**:
   - Resources with errors have red border
   - Resources without errors have normal border
   - Error border updates when errors are fixed
   - Works in all three themes

4. **Collection indicator**:
   - `resource 'xxx' 'type' = [for ...]` shows `[]` suffix on type
   - Module with `for` shows `<collection>` suffix
   - Non-collection resources show no suffix

5. **Auto-fit on load**:
   - Graph fits viewport on initial load
   - Large graphs are zoomed out to fit
   - Small graphs are shown at 1x (not zoomed in beyond 100%)

6. **Keyboard**:
   - Tab navigates between nodes
   - Enter/Space on focused node → navigates to source
   - Focus indicator is visible in all themes

7. **Compare with old visualizer**:
   - Open the same `.bicep` file in both old and new visualizers
   - Verify visual parity: same icons, similar layout, same behavior
   - Take screenshots for review

## Notes

- The `revealFileRange` on the extension host side is already implemented in Step 5's `view.ts`. It follows the same logic as the old visualizer: check visible editors first, then open the file if not already visible.
- The `range` data from the language server uses 0-based line/character numbers (LSP convention). The `vscode.Range` constructor also uses 0-based. No conversion needed.
- Consider adding a tooltip on hover showing the full resource type (before truncation), similar to the old visualizer's behavior via Cytoscape's `title` data field. Use the browser's native `title` attribute or a custom tooltip component.
