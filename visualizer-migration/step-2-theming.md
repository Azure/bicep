# Step 2: Add Theming Support to Visual Designer

## Goal

Make the visual designer respond to VS Code's dark, light, and high-contrast themes so it looks correct in all theme modes. Define curated color palettes for each theme rather than relying on VS Code CSS variables (`--vscode-*`), since editor theme colors are not designed for graph visualization and may not produce aesthetically pleasing results for nodes, edges, and canvas backgrounds.

## Background

### Old visualizer theming (`src/vscode-bicep/src/visualizer/app/themes.ts`)

The old visualizer defines three static theme objects (`darkTheme`, `lightTheme`, `highContrastTheme`) and uses a `MutationObserver` on `document.body.className` to detect theme changes (`vscode-dark`, `vscode-light`, `vscode-high-contrast`). These themes are passed through styled-components' `ThemeProvider` and consumed by components via `${({ theme }) => theme.xxx}`.

### Why not use VS Code CSS custom properties directly?

VS Code webviews inject CSS custom properties (e.g. `--vscode-editor-background`, `--vscode-editorWidget-border`) that track the active theme. While convenient, these variables are designed for editor UI — not for a graph visualization canvas with nodes and edges. Using them directly can produce poor visual results:

- Editor widget backgrounds may lack sufficient contrast against the canvas
- Editor borders may be too subtle or too prominent for node cards
- Color combinations that work in a text editor UI may look wrong on a diagram

Instead, we define **curated color palettes** for each theme, optimized for graph readability. We derive these from a mix of the current FigJam-inspired light-theme colors, the old visualizer's dark/HC colors, and manual adjustments for visual quality.

We still use `--vscode-font-family` for typography consistency with the editor.

### Approach: styled-components `ThemeProvider`

We use styled-components' built-in theming mechanism:

1. **`DefaultTheme` type augmentation** (`styled.d.ts`) — declares the shape of the theme object so all `${({ theme }) => ...}` interpolations are type-safe.
2. **Four theme objects** (`themes.ts`) — `lightTheme`, `darkTheme`, `highContrastTheme`, `highContrastLightTheme`, each satisfying `DefaultTheme`.
3. **`useTheme()` hook** (`hooks/useTheme.ts`) — observes the `data-vscode-theme-kind` attribute on `<body>` via `MutationObserver` and returns the matching `DefaultTheme`. VS Code sets this attribute to one of four values:
   - `vscode-light` — light theme
   - `vscode-dark` — dark theme
   - `vscode-high-contrast` — high contrast dark
   - `vscode-high-contrast-light` — high contrast light

   We use this data attribute instead of `document.body.className` because the class list can contain multiple theme-related classes (e.g. `vscode-high-contrast-light vscode-high-contrast vscode-reduce-motion`), making regex matching unreliable. The data attribute always has a single, unambiguous value.
4. **`<ThemeProvider>`** in `App.tsx` — wraps the entire component tree so all styled-components and `useTheme()` calls receive the current theme.

Components access theme values in two ways:
- **Styled-component template literals**: `${({ theme }) => theme.node.border}`
- **`useTheme()` from styled-components**: for imperative access in function bodies (e.g., SVG props, data URI cursors)

### Current state of new visual designer (before this step)

All components use hardcoded light-theme colors (FigJam-inspired):
- `ResourceDeclaration`: `#f9fafa` background, `#333638` border, `#242424` text, `#898e96` secondary text
- `ModuleDeclaration`: `#f9fafa` background, `#333638` border, `#242424` text
- `CanvasBackground`: `#f5f5f5` background, `#c4c4c4` dots
- `EdgeMarkerDefs`: `#cecccc` stroke
- `StraightEdge`: `#cecccc` stroke
- `GraphControlBar`: white background, `#4a5568` icon color

## Color Palettes

### Light theme (current FigJam-inspired — keep as-is)

| Token | Value | Notes |
|---|---|---|
| Canvas bg | `#f5f5f5` | Warm light gray canvas |
| Canvas dots | `#c4c4c4` | Subtle dot grid |
| Node bg | `#f9fafa` | Near-white card |
| Node border | `#333638` | Dark charcoal border |
| Text primary | `#242424` | Near-black text |
| Text secondary | `#898e96` | Muted gray |
| Edge color | `#cecccc` | Soft gray edge |
| Control bar bg | `rgba(255, 255, 255, 0.95)` | Frosted glass toolbar |
| Control bar border | `rgba(0, 0, 0, 0.1)` | Very subtle border |
| Control bar icon | `#4a5568` | Slate gray icons |
| Control btn hover | `rgba(74, 85, 104, 0.1)` | Light slate hover |
| Control btn active | `rgba(74, 85, 104, 0.2)` | Deeper active press |
| Focus border | `#3182ce` | Blue focus ring |
| Error indicator | `#e53e3e` | Red |
| Success indicator | `#38a169` | Green |
| Grab cursor fill | `%23000000` | Black cursor (URL-encoded) |
| Grab cursor opacity | `0.6` | Semi-transparent |

### Dark theme

Derived from the old visualizer's dark theme, modernized for the FigJam-style card layout. Node cards are slightly raised from the canvas with subtle borders; edges are visible but not dominant.

| Token | Value | Notes |
|---|---|---|
| Canvas bg | `#1e1e1e` | Standard VS Code dark background |
| Canvas dots | `#3f3f3f` | Visible against dark canvas (from old visualizer) |
| Node bg | `#2d2d2d` | Dark card, slightly lighter than canvas |
| Node border | `#555555` | Medium gray border, visible but subtle |
| Text primary | `#e0e0e0` | Off-white, easy on eyes |
| Text secondary | `#a0a0a0` | Muted light gray |
| Edge color | `#555555` | Matches node border for visual cohesion |
| Control bar bg | `rgba(45, 45, 45, 0.95)` | Frosted dark toolbar |
| Control bar border | `rgba(255, 255, 255, 0.1)` | Subtle light border |
| Control bar icon | `#b0b0b0` | Light gray icons |
| Control btn hover | `rgba(255, 255, 255, 0.1)` | Subtle light hover |
| Control btn active | `rgba(255, 255, 255, 0.15)` | Slightly brighter active |
| Focus border | `#4d90fe` | Brighter blue for dark backgrounds |
| Error indicator | `#f14c4c` | VS Code dark error red |
| Success indicator | `#89d185` | VS Code dark success green |
| Grab cursor fill | `%23ffffff` | White cursor (URL-encoded) |
| Grab cursor opacity | `0.6` | Semi-transparent |

### High contrast dark theme

Derived from the old visualizer's high-contrast theme. Maximum contrast with yellow accents on black background. No dot grid (following old visualizer convention).

| Token | Value | Notes |
|---|---|---|
| Canvas bg | `#000000` | Pure black |
| Canvas dots | `transparent` | No dots — clean high-contrast canvas |
| Node bg | `#0a0a0a` | Near-black card |
| Node border | `#ffd700` | Yellow border for maximum visibility |
| Text primary | `#ffffff` | Pure white text |
| Text secondary | `#ffd700` | Yellow secondary text (from old visualizer) |
| Edge color | `#ffd700` | Yellow edges, full visibility |
| Control bar bg | `#000000` | Black toolbar |
| Control bar border | `#ffd700` | Yellow border |
| Control bar icon | `#ffffff` | White icons |
| Control btn hover | `rgba(255, 215, 0, 0.2)` | Yellow tint hover |
| Control btn active | `rgba(255, 215, 0, 0.3)` | Deeper yellow active |
| Focus border | `#ffd700` | Yellow focus ring |
| Error indicator | `#ff00ff` | Fuchsia (from old visualizer HC) |
| Success indicator | `#00ffff` | Cyan (from old visualizer HC) |
| Grab cursor fill | `%23ffffff` | White cursor (URL-encoded) |
| Grab cursor opacity | `1.0` | Full opacity for max visibility |

### High contrast light theme

Maximum contrast with dark accents on white background. No dot grid.

| Token | Value | Notes |
|---|---|---|
| Canvas bg | `#ffffff` | Pure white |
| Canvas dots | `transparent` | No dots — clean high-contrast canvas |
| Node bg | `#ffffff` | White card |
| Node border | `#000000` | Black border |
| Text primary | `#000000` | Pure black text |
| Text secondary | `#333333` | Very dark gray |
| Edge color | `#000000` | Black edges |
| Control bar bg | `#ffffff` | White toolbar |
| Control bar border | `#000000` | Black border |
| Control bar icon | `#000000` | Black icons |
| Control btn hover | `rgba(0, 0, 0, 0.1)` | Light hover |
| Control btn active | `rgba(0, 0, 0, 0.2)` | Deeper active |
| Focus border | `#0000cd` | Medium blue focus |
| Error indicator | `#ff0000` | Pure red |
| Success indicator | `#008000` | Dark green |
| Grab cursor fill | `%23000000` | Black cursor (URL-encoded) |
| Grab cursor opacity | `1.0` | Full opacity |

## Tasks

### 2.1 Create `styled.d.ts` — `DefaultTheme` type augmentation

Augment styled-components' `DefaultTheme` interface so all theme property accesses are type-safe.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/styled.d.ts`

```ts
import "styled-components";

declare module "styled-components" {
  export interface DefaultTheme {
    name: "light" | "dark" | "high-contrast" | "high-contrast-light";
    canvas: {
      background: string;
      dotColor: string;
    };
    node: {
      background: string;
      border: string;
    };
    text: {
      primary: string;
      secondary: string;
    };
    edge: {
      color: string;
    };
    controlBar: {
      background: string;
      border: string;
      icon: string;
      hoverBackground: string;
      activeBackground: string;
    };
    focusBorder: string;
    error: string;
    success: string;
    grabCursor: {
      /** URL-encoded fill color for inline SVG data URI (e.g. `%23000000`). */
      fill: string;
      opacity: number;
    };
  }
}
```

### 2.2 Create `themes.ts` — four theme objects

Define the four curated color palettes as typed `DefaultTheme` objects plus a helper to map the body class to a theme.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/theming/themes.ts`

```ts
import type { DefaultTheme } from "styled-components";

export const lightTheme: DefaultTheme = {
  name: "light",
  canvas: { background: "#f5f5f5", dotColor: "#c4c4c4" },
  node: { background: "#f9fafa", border: "#333638" },
  text: { primary: "#242424", secondary: "#898e96" },
  edge: { color: "#cecccc" },
  controlBar: {
    background: "rgba(255, 255, 255, 0.95)",
    border: "rgba(0, 0, 0, 0.1)",
    icon: "#4a5568",
    hoverBackground: "rgba(74, 85, 104, 0.1)",
    activeBackground: "rgba(74, 85, 104, 0.2)",
  },
  focusBorder: "#3182ce",
  error: "#e53e3e",
  success: "#38a169",
  grabCursor: { fill: "%23000000", opacity: 0.6 },
};

// darkTheme, highContrastTheme, highContrastLightTheme follow the same shape
// with values from the Color Palettes tables above.

export function getThemeFromBody(): DefaultTheme {
  switch (document.body.dataset.vscodeThemeKind) {
    case "vscode-dark":
      return darkTheme;
    case "vscode-high-contrast":
      return highContrastTheme;
    case "vscode-high-contrast-light":
      return highContrastLightTheme;
    default:
      return lightTheme;
  }
}
```

### 2.3 Create `useTheme` hook — observe VS Code theme changes

Create a hook that watches the `data-vscode-theme-kind` attribute on `<body>` for VS Code theme changes and returns the matching `DefaultTheme`. This is the single place that bridges VS Code's theme indicator to our React component tree.

**File**: `src/vscode-bicep-ui/apps/visual-designer/src/theming/useTheme.ts`

```ts
import type { DefaultTheme } from "styled-components";

import { useEffect, useState } from "react";
import { getThemeFromBody } from "./themes";

export function useTheme(): DefaultTheme {
  const [theme, setTheme] = useState(getThemeFromBody);

  useEffect(() => {
    const observer = new MutationObserver(() => {
      setTheme(getThemeFromBody());
    });

    observer.observe(document.body, { attributes: true, attributeFilter: ["data-vscode-theme-kind"] });

    return () => observer.disconnect();
  }, []);

  return theme;
}
```

### 2.4 Wire up `ThemeProvider` in `App.tsx`

Wrap the entire component tree in styled-components' `<ThemeProvider>` and render `<GlobalStyle />` inside it so global styles also have access to the theme.

```tsx
import { ThemeProvider } from "styled-components";
import { GlobalStyle } from "./GlobalStyle";
import { useTheme } from "./hooks/useTheme";

export function App() {
  // ...existing setup...

  const theme = useTheme();

  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      <PanZoomProvider>
        {/* ...children... */}
      </PanZoomProvider>
    </ThemeProvider>
  );
}
```

The entry point (`index.tsx`) renders only `<App />` — it does not reference theming at all:

```tsx
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
```

### 2.5 Update `GlobalStyle.ts`

Use `--vscode-font-family` for typography consistency and theme props for colors:

```ts
export const GlobalStyle = createGlobalStyle`
  body {
    height: 100vh;
    margin: 0;
    padding: 0;
    display: flex;
    overflow: hidden;
    font-family: var(--vscode-font-family, sans-serif);
    background-color: ${({ theme }) => theme.canvas.background};
    color: ${({ theme }) => theme.text.primary};
  }

  #root {
    flex: 1 1 auto;
    overflow: hidden;
  }
`;
```

### 2.6 Update `CanvasBackground.tsx`

Replace hardcoded colors with theme props. The dot color uses a `currentColor` trick: set `color` on the `<svg>` element via a style prop, and use `fill="currentColor"` on the `<circle>`. This allows the SVG pattern inside `<defs>` to pick up the theme color without issues.

```tsx
const $Svg = styled.svg`
  overflow: visible;
  background-color: ${({ theme }) => theme.canvas.background};
  position: absolute;
  pointer-events: none;
`;

export function CanvasBackground() {
  const theme = useTheme(); // from styled-components

  return (
    <$Svg width="100%" height="100%" style={{ color: theme.canvas.dotColor }}>
      <defs>
        <pattern ...>
          <circle ref={circleRef} cx="1" cy="1" r="1" fill="currentColor" />
        </pattern>
      </defs>
      <rect ... fill="url(#figma-like-dots)" />
    </$Svg>
  );
}
```

In high-contrast themes, `dotColor` is `transparent`, which hides the dot grid (matching the old visualizer's `backgroundImage: "unset"` behavior).

### 2.7 Update `ResourceDeclaration.tsx`

Replace hardcoded colors with styled-component theme interpolations:

```tsx
const $ResourceDelcarton = styled.div`
  /* ...layout styles unchanged... */
  border: 2px solid ${({ theme }) => theme.node.border};
  border-radius: 4px;
  background-color: ${({ theme }) => theme.node.background};
  height: 70px;
  min-width: 200px;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 18px;
  font-weight: 500;
  color: ${({ theme }) => theme.text.primary};
`;

const $ResourceTypeContainer = styled.div`
  font-size: 12px;
  font-weight: 500;
  color: ${({ theme }) => theme.text.secondary};
  text-transform: uppercase;
`;
```

### 2.8 Update `ModuleDeclaration.tsx`

Same pattern as `ResourceDeclaration`:

```tsx
const $ModuleDelcarton = styled.div`
  /* ...layout unchanged... */
  border: 2px solid ${({ theme }) => theme.node.border};
  border-radius: 4px;
  background: ${({ theme }) => theme.node.background};
`;

const $SymbolicNameContainer = styled.div`
  font-size: 14px;
  color: ${({ theme }) => theme.text.primary};
  /* ... */
`;
```

### 2.9 Update `EdgeMarkerDefs.tsx`

SVG elements inside `<defs>` cannot reliably use CSS-in-JS class names. Use `useTheme()` from styled-components to get the theme object and pass the color as a JSX prop:

```tsx
import { useTheme } from "styled-components";

export function EdgeMarkerDefs() {
  const theme = useTheme();

  return (
    <defs>
      <marker ...>
        <polyline
          points="2,2 5,5 2,8"
          fill="none"
          strokeWidth="1"
          stroke={theme.edge.color}
          strokeLinecap="round"
          strokeLinejoin="round"
        />
      </marker>
    </defs>
  );
}
```

### 2.10 Update `StraightEdge.tsx`

Same `useTheme()` approach for the edge stroke color:

```tsx
import { useTheme } from "styled-components";

export function StraightEdge({ fromId, toId }: EdgeAtomValue) {
  const theme = useTheme();

  return (
    <path
      ref={ref}
      fill="none"
      stroke={theme.edge.color}
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={2}
      markerEnd="url(#line-arrow)"
    />
  );
}
```

### 2.11 Update `GraphControlBar.tsx`

Replace hardcoded control bar colors with theme interpolations:

```tsx
const $GraphControlBar = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 4px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  backdrop-filter: blur(8px);
`;

const $ControlButton = styled.button`
  /* ...layout unchanged... */
  background-color: transparent;
  color: ${({ theme }) => theme.controlBar.icon};
  cursor: pointer;

  &:hover {
    background-color: ${({ theme }) => theme.controlBar.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.controlBar.activeBackground};
    transform: scale(0.95);
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 1px;
  }
`;
```

### 2.12 Update Canvas grab cursor for dark themes

The grab cursor bakes a fill color into an inline SVG data URI. Since ThemeProvider makes the full theme object available via `useTheme()`, the cursor is simply rebuilt from theme props — no `getComputedStyle` needed:

```tsx
import styled, { useTheme } from "styled-components";

function buildGrabCursor(fill: string, opacity: number): string {
  return (
    `url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" ` +
    `fill="${fill}" fill-opacity="${opacity}" width="32px" height="32px" ` +
    `viewBox="0 0 10 10"><circle cx="5" cy="5" r="4"/></svg>') 16 16, auto`
  );
}

const $PanZoom = styled(PanZoom)<{ $grabCursorUrl: string }>`
  /* ...layout unchanged... */
  &:active {
    cursor: ${({ $grabCursorUrl }) => $grabCursorUrl};
  }
`;

export function Canvas({ children }: PropsWithChildren) {
  const theme = useTheme();
  const grabCursorUrl = buildGrabCursor(theme.grabCursor.fill, theme.grabCursor.opacity);

  return (
    <$PanZoom $grabCursorUrl={grabCursorUrl}>
      <CanvasBackground />
      {children}
    </$PanZoom>
  );
}
```

## Verification

1. Run `npm run dev` in `src/vscode-bicep-ui/apps/visual-designer/`.
2. Use the `<vscode-dev-toolbar>` (from `@vscode-elements/webview-playground`) in the dev HTML to toggle between light, dark, high-contrast dark, and high-contrast light themes.
3. Verify for each theme:
   - Canvas background color changes appropriately
   - Dot grid is visible in light/dark themes and hidden in high-contrast themes
   - Node card backgrounds, borders, and text are readable with good contrast
   - Edge lines and arrows are visible
   - Control bar buttons are visible and have proper hover/active states
   - Focus indicators work (tab to buttons, check outline)
   - Grab cursor is visible against the canvas background
4. Compare with the old visualizer in each theme to confirm parity or improvement.

## Design Decisions

- **Custom palettes over `--vscode-*` variables**: Editor theme colors are optimized for text-editor UI, not for graph visualization. By curating our own palettes we ensure nodes, edges, and canvas always look intentional and readable.
- **styled-components `ThemeProvider` over CSS custom properties**: Provides type-safe theme access throughout the component tree via `${({ theme }) => ...}` interpolations and the `useTheme()` hook. All theme values are plain TypeScript objects, making them easy to test and refactor. The `MutationObserver` in `useTheme` detects VS Code theme switches and triggers a single React re-render to propagate the new theme.
- **`DefaultTheme` augmentation**: Extends styled-components' empty `DefaultTheme` interface with our custom shape. This gives compile-time safety — typos like `theme.nod.border` are caught by TypeScript.
- **`useTheme()` hook + `MutationObserver`**: Observes the `data-vscode-theme-kind` attribute on `<body>` (not `className`, which can contain multiple overlapping theme classes) and maps it to the matching `DefaultTheme` object.
- **`currentColor` trick for SVG patterns**: The dot grid circle uses `fill="currentColor"` with the `color` style prop set on the parent `<svg>`. This avoids issues with styled-component class names not applying inside `<defs>`.
- **`useTheme()` for SVG element props**: SVG elements in `<defs>` (markers) and plain SVG elements (paths) receive theme colors as direct JSX props via `useTheme()` from styled-components, rather than through styled-component template literals.
- **Four theme variants**: Light, dark, high-contrast dark, and high-contrast light. The old visualizer only supported three (no HC light). Adding HC light ensures complete accessibility coverage.
- **High-contrast dot suppression**: Following the old visualizer's convention, high-contrast themes hide the dot grid by setting `dotColor: "transparent"` rather than conditional JavaScript.
- **Selective VS Code variable reuse**: Only `--vscode-font-family` is used for typography consistency. All visual design tokens are defined in our own theme objects.
- **Old visualizer color heritage**: Dark theme canvas (`#1e1e1e`), dot color (`#3f3f3f`), and HC yellow/fuchsia/cyan accents are carried forward from the old visualizer with adjustments for the new card-based layout.
