# Visualizer Migration: Cytoscape.js → React Graph Engine

## Overview

This migration replaces the Bicep VS Code extension's Cytoscape.js-based visualizer with the new Jotai/React-based graph rendering engine built in the `vscode-bicep-ui` monorepo.

### Why

- **Cytoscape.js** renders nodes as SVG data URIs baked into background images — there's no real DOM for nodes, making it hard to add interactivity, accessibility, or richer UI.
- **The new graph engine** renders nodes as actual React components inside positioned `<div>` elements, with SVG `<path>` edges. This enables real DOM interactions, proper accessibility, and future visual editing capabilities.
- The new engine uses **Jotai atoms** for fine-grained reactivity (atom-per-node-property), **d3-drag/zoom** for pan/zoom, and **Motion** for spring animations.

### Architecture Comparison

| Aspect | Old (Cytoscape.js) | New (React Graph Engine) |
|--------|--------------------|-----------------------|
| Graph library | `cytoscape` + `cytoscape-elk` | Custom HTML/SVG + `d3-drag`/`d3-zoom` |
| Layout | ELK layered (via `cytoscape-elk`) | Manual positioning (ELK to be added) |
| State management | React `useState` | Jotai atoms (atom-per-property) |
| Node rendering | SVG data URIs as Cytoscape backgrounds | Real React components in positioned `<div>`s |
| Edge rendering | Cytoscape bezier curves | SVG `<path>` with box-intersection clipping |
| Animation | Cytoscape CSS transitions | Motion spring animations |
| Pan/zoom | Cytoscape built-in | Shared `<PanZoom>` component (d3-zoom) |
| Theming | 3 static theme objects + MutationObserver | Hardcoded (to be themed via CSS variables) |
| Data source | LSP `textDocument/deploymentGraph` | Hardcoded test data (to be integrated) |
| Build | Webpack (bundled inside `vscode-bicep`) | Vite (in `vscode-bicep-ui` monorepo) |
| Icons | Inline SVG strings via `svg-inline-loader` | `<AzureIcon>` component with lazy SVG loading |

### Relevant Paths

| Path | Description |
|------|-------------|
| `src/vscode-bicep/src/visualizer/` | Old visualizer (extension host + webview app) |
| `src/vscode-bicep-ui/apps/visual-designer/` | New graph engine + design view components |
| `src/vscode-bicep-ui/packages/components/` | Shared components (`AzureIcon`, `PanZoom`, `Codicon`) |
| `src/vscode-bicep-ui/packages/messaging/` | Shared webview messaging library |
| `src/vscode-bicep/src/commands/showVisualizer.ts` | Commands that open the visualizer |

### Migration Steps

The migration is split into 7 incremental PRs, each self-contained and shippable:

1. ~~**[Expand Azure Icon Coverage](./step-1-icons.md)** — Migrate ~80+ resource type→SVG mappings to `@vscode-bicep-ui/components`~~ ✅
2. ~~**[Add Theming Support](./step-2-theming.md)** — Theme the visual designer using styled-components ThemeProvider~~ ✅
3. ~~**[Integrate ELK.js Auto-Layout](./step-3-elk-layout.md)** — Add automatic graph layout to the new engine~~ ✅
4. ~~**[Wire Up LSP Data Source](./step-4-data-source.md)** — Connect to `textDocument/deploymentGraph` via shared messaging~~ ✅
5. **[Integrate into VS Code Extension](./step-5-extension-integration.md)** — Load the visual designer in a webview panel
6. **[Achieve Full Feature Parity](./step-6-feature-parity.md)** — Double-click navigation, status bar, error indicators, etc.
7. **[Remove Old Visualizer](./step-7-cleanup.md)** — Delete Cytoscape.js code and dependencies
