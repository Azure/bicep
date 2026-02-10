# Step 2: Add Theming Support to Visual Designer

## Goal

Make the visual designer respond to VS Code's dark, light, and high-contrast themes so it looks correct in all theme modes. Use VS Code CSS custom properties (`var(--vscode-*)`) instead of maintaining static theme objects.

## Background

### Old visualizer theming (`src/vscode-bicep/src/visualizer/app/themes.ts`)

The old visualizer defines three static theme objects (`darkTheme`, `lightTheme`, `highContrastTheme`) and uses a `MutationObserver` on `document.body.className` to detect theme changes (`vscode-dark`, `vscode-light`, `vscode-high-contrast`). These themes are passed through styled-components' `ThemeProvider` and consumed by components via `${({ theme }) => theme.xxx}`.

### New approach: VS Code CSS custom properties

VS Code webviews automatically inject CSS custom properties that update when the user switches themes. Examples:
- `--vscode-editor-background` — editor background color
- `--vscode-editor-foreground` — editor foreground color
- `--vscode-foreground` — general foreground
- `--vscode-font-family` — UI font family
- `--vscode-focusBorder` — focus indicator color
- `--vscode-contrastBorder` — high-contrast border (empty in non-HC themes)
- `--vscode-contrastActiveBorder` — high-contrast active border
- `--vscode-badge-background` — badge background

Using these directly in CSS eliminates the need for `MutationObserver`, `ThemeProvider`, and static theme objects entirely. Colors automatically update without React re-renders.

### Current state of new visual designer

All components use hardcoded light-theme colors:
- `ResourceDeclaration`: `#f9fafa` background, `#333638` border, `#242424` text, `#898e96` secondary text
- `ModuleDeclaration`: `#f9fafa` background, `#333638` border, `#242424` text
- `CanvasBackground`: `#f5f5f5` background, `#c4c4c4` dots
- `EdgeMarkerDefs`: `#cecccc` stroke
- `StraightEdge`: `#cecccc` stroke
- `GraphControlBar`: white background, `#4a5568` icon color

## Tasks

### 2.1 Define CSS custom property mapping

Create a CSS custom property file or a constants module that maps semantic token names to VS Code CSS variables. This provides a clear mapping layer and makes it easy to adjust. Add this as a new file in the visual designer:

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/theme.ts`

Define the mapping from semantic design tokens to VS Code CSS variables:

| Semantic Token | VS Code CSS Variable | Fallback (light) | Purpose |
|---|---|---|---|
| Canvas background | `--vscode-editor-background` | `#ffffff` | Main canvas background |
| Canvas dot color | `--vscode-editorLineNumber-foreground` | `#c4c4c4` | Background dot grid |
| Node background | `--vscode-editorWidget-background` | `#f9fafa` | Resource/module card bg |
| Node border | `--vscode-editorWidget-border` | `#333638` | Card border |
| Node text primary | `--vscode-editor-foreground` | `#242424` | Symbolic name |
| Node text secondary | `--vscode-descriptionForeground` | `#898e96` | Resource type |
| Edge color | `--vscode-editorLineNumber-foreground` | `#c4c4c4` | Edge stroke + arrow |
| Error indicator | `--vscode-errorForeground` | `red` | Error state border |
| Success indicator | `--vscode-testing-iconPassed` | `green` | No-error indicator |
| Control bar bg | `--vscode-editorWidget-background` | `white` | Floating toolbar bg |
| Control bar border | `--vscode-editorWidget-border` | `rgba(0,0,0,0.1)` | Toolbar border |
| Control bar icon | `--vscode-icon-foreground` | `#4a5568` | Toolbar button icons |
| Control bar hover | `--vscode-toolbar-hoverBackground` | `rgba(0,0,0,0.1)` | Button hover state |
| Focus border | `--vscode-focusBorder` | `#3182ce` | Focus indicator |
| HC active border | `--vscode-contrastActiveBorder` | (empty) | High-contrast border |

### 2.2 Update `CanvasBackground.tsx`

Replace hardcoded colors with CSS variables:

```tsx
const $Svg = styled.svg`
  overflow: visible;
  background-color: var(--vscode-editor-background, #ffffff);
  position: absolute;
  pointer-events: none;
`;

// In the circle element:
<circle ref={circleRef} cx="1" cy="1" r="1" fill="var(--vscode-editorLineNumber-foreground, #c4c4c4)" />
```

**Note on high-contrast theme**: The old visualizer sets `backgroundImage: "unset"` for high contrast (no dots). To replicate this, you can detect high contrast via `document.body.classList.contains('vscode-high-contrast')` and conditionally hide the dot pattern, OR use `--vscode-contrastBorder` presence as a signal (it's only set in HC themes). A simpler approach: the dot color in HC themes from `--vscode-editorLineNumber-foreground` will naturally be visible against the black background, so dots can remain.

### 2.3 Update `ResourceDeclaration.tsx`

Replace hardcoded colors:

```tsx
const $ResourceDeclaration = styled.div`
  /* ... layout styles unchanged ... */
  border: 2px solid var(--vscode-editorWidget-border, #333638);
  border-radius: 4px;
  background-color: var(--vscode-editorWidget-background, #f9fafa);
  height: 70px;
  min-width: 200px;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 18px;
  font-weight: 500;
  color: var(--vscode-editor-foreground, #242424);
`;

const $ResourceTypeContainer = styled.div`
  font-size: 12px;
  font-weight: 500;
  color: var(--vscode-descriptionForeground, #898e96);
  text-transform: uppercase;
`;
```

### 2.4 Update `ModuleDeclaration.tsx`

Same pattern — replace hardcoded colors with CSS variables:

```tsx
const $ModuleDeclaration = styled.div`
  /* ... layout unchanged ... */
  border: 2px solid var(--vscode-editorWidget-border, #333638);
  border-radius: 4px;
  background: var(--vscode-editorWidget-background, #f9fafa);
`;

const $SymbolicNameContainer = styled.div`
  font-size: 14px;
  color: var(--vscode-editor-foreground, #242424);
  /* ... */
`;
```

### 2.5 Update `EdgeMarkerDefs.tsx`

Replace hardcoded arrow stroke color:

```tsx
<polyline
  points="2,2 5,5 2,8"
  fill="none"
  strokeWidth="1"
  stroke="var(--vscode-editorLineNumber-foreground, #cecccc)"
  strokeLinecap="round"
  strokeLinejoin="round"
/>
```

**Note**: SVG elements inside `<defs>` can use CSS `var()` in modern browsers. If there are issues, consider using `currentColor` and setting `color` on the parent SVG container.

### 2.6 Update `StraightEdge.tsx`

Replace hardcoded edge stroke color:

```tsx
<path
  ref={ref}
  fill="none"
  stroke="var(--vscode-editorLineNumber-foreground, #cecccc)"
  strokeLinecap="round"
  strokeLinejoin="round"
  strokeWidth={2}
  markerEnd="url(#line-arrow)"
/>
```

### 2.7 Update `GraphControlBar.tsx`

Replace hardcoded control bar colors:

```tsx
const $GraphControlBar = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 4px;
  background-color: var(--vscode-editorWidget-background, rgba(255, 255, 255, 0.95));
  border: 1px solid var(--vscode-editorWidget-border, rgba(0, 0, 0, 0.1));
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(8px);
`;

const $ControlButton = styled.button`
  /* ... layout unchanged ... */
  background-color: transparent;
  color: var(--vscode-icon-foreground, #4a5568);
  cursor: pointer;

  &:hover {
    background-color: var(--vscode-toolbar-hoverBackground, rgba(74, 85, 104, 0.1));
  }

  &:active {
    background-color: var(--vscode-toolbar-activeBackground, rgba(74, 85, 104, 0.2));
    transform: scale(0.95);
  }

  &:focus-visible {
    outline: 2px solid var(--vscode-focusBorder, #3182ce);
    outline-offset: 1px;
  }
`;
```

### 2.8 Update `GlobalStyle.ts`

Ensure the global body uses VS Code's font family:

```tsx
const GlobalStyle = createGlobalStyle`
  body {
    /* ... */
    font-family: var(--vscode-font-family, sans-serif);
    background-color: var(--vscode-editor-background, #ffffff);
    color: var(--vscode-editor-foreground, #242424);
  }
`;
```

### 2.9 Update Canvas grab cursor for dark themes

The current grab cursor SVG uses `fill="%23000000"` (black). For dark themes, this would be invisible. Consider using `fill="currentColor"` or generating the cursor data URI dynamically, or using a neutral gray (`fill="%23808080"`).

### 2.10 (Optional) Remove styled-components ThemeProvider

Since theming is now done entirely via CSS variables, the styled-components `ThemeProvider` and `DefaultTheme` typing are no longer needed for theming. The visual designer currently doesn't use `ThemeProvider` at all (it was only used in the old visualizer), so no removal is needed — just avoid introducing it.

## Verification

1. Run `npm run dev` in `src/vscode-bicep-ui/apps/visual-designer/`.
2. Use the `<vscode-dev-toolbar>` (from `@vscode-elements/webview-playground`) in the dev HTML to toggle between light, dark, and high-contrast themes.
3. Verify for each theme:
   - Canvas background color changes appropriately
   - Dot grid is visible (appropriate contrast)
   - Node card backgrounds, borders, and text are readable
   - Edge lines and arrows are visible
   - Control bar buttons are visible and have proper hover/active states
   - Focus indicators work (tab to buttons, check outline)
4. Take screenshots for comparison with the old visualizer in each theme.

## Notes

- The `--vscode-contrastBorder` variable is only set in high-contrast themes. You can use it for conditional high-contrast styling: `border: 1px solid var(--vscode-contrastBorder, var(--vscode-editorWidget-border, #333638))`.
- VS Code's high-contrast theme uses yellow (`#FFD700`) for borders and selection — the CSS variables will naturally pick this up.
- The dev toolbar provided by `@vscode-elements/webview-playground` injects the same CSS variables that VS Code does, making testing straightforward without needing to run inside VS Code.
