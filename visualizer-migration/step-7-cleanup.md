# Step 7: Remove Old Cytoscape.js Visualizer

## Goal

Remove the old Cytoscape.js-based visualizer once the new visual designer has been thoroughly tested, the feature flag has been enabled by default, and confidence is high that no regressions exist.

## Prerequisites

Before starting this step:

- [ ] The new visual designer has been behind the feature flag for at least one release cycle
- [ ] No blocking bugs reported against the new visual designer
- [ ] All features from Step 6 are verified working
- [ ] Visual comparison screenshots reviewed and approved
- [ ] Performance benchmarks comparable (large graphs with 50+ nodes)

## Tasks

### 7.1 Remove the old visualizer source

Delete the entire old visualizer directory:

```
src/vscode-bicep/src/visualizer/
  index.ts
  messages.ts
  view.ts
  viewManager.ts
  app/
    themes.ts
    vscode.ts
    assets/
      icons/
        azure/          ← entire directory (~100 SVG files + index.ts)
    components/
      App.tsx
      index.tsx
      StatusBar.tsx
      Tooltip.tsx
      Graph/
        CommandBar.tsx
        Graph.tsx
        index.ts
        style.ts
    hooks/
      index.ts
      useCytoscape.ts
    typings/
      assets/
        index.d.ts
      cytoscape-elk/
        index.d.ts
      styled-components/
        index.d.ts
```

### 7.2 Remove the webpack visualizer config

In `src/vscode-bicep/webpack.config.ts`:

1. **Remove `visualizerConfig`**: Delete the entire second webpack configuration object (target: `web`, entry: `./src/visualizer/app/components/index.tsx`).
2. **Update the export**: Change `module.exports` to return only `[extensionConfig]` instead of `[extensionConfig, visualizerConfig]`.
3. **Remove the `visualizer/app/` exclusion** from `extensionConfig.module.rules[0].exclude` — it's no longer needed since the directory is deleted.

```typescript
// Before:
exclude: [/node_modules/, /panes\/deploy\/app/, /visualizer\/app/, /test/],

// After:
exclude: [/node_modules/, /panes\/deploy\/app/, /test/],
```

### 7.3 Remove old dependencies

Remove these from `src/vscode-bicep/package.json` dependencies:

```json
"cytoscape": "^3.30.4",        // Only used by old visualizer
"cytoscape-elk": "^2.3.0",     // Only used by old visualizer
"react-icons": "^5.5.0",       // Only used by old visualizer's CommandBar
```

And from devDependencies:

```json
"svg-inline-loader": "...",           // Only used by old visualizer webpack config
"@types/cytoscape": "...",            // Cytoscape type definitions
```

Also check if `elkjs` is a transitive dependency of `cytoscape-elk` — if so, removing `cytoscape-elk` removes it automatically. If `elkjs` is listed directly, keep it only if the new visual designer needs it (it shouldn't be in `vscode-bicep` — it's in `vscode-bicep-ui`).

Run `npm install` afterwards to clean up `package-lock.json`.

### 7.4 Rename `visualizer-v2/` to `visualizer/`

Rename the directory:
```
src/vscode-bicep/src/visualizer-v2/ → src/vscode-bicep/src/visualizer/
```

Update all import paths that reference `visualizer-v2`:
- `src/vscode-bicep/src/commands/showVisualizer.ts`
- `src/vscode-bicep/src/commands/showSourceFromVisualizer.ts`
- Extension activation code (wherever `VisualDesignerViewManager` is imported)
- Any barrel exports (`index.ts` files)

### 7.5 Remove the feature flag

1. **Remove the configuration setting** from `src/vscode-bicep/package.json`:
   ```json
   // Remove:
   "bicep.experimental.visualDesigner": { ... }
   ```

2. **Remove dispatch logic** from `showVisualizer.ts`:
   ```typescript
   // Before:
   const useNewVisualizer = vscode.workspace.getConfiguration("bicep.experimental").get<boolean>("visualDesigner", false);
   if (useNewVisualizer) { viewManagerV2.openView(...) } else { viewManager.openView(...) }

   // After:
   await viewManager.openView(documentUri, viewColumn);
   ```

3. **Remove the old view manager parameter** from command classes — they should only accept the (renamed) new view manager.

4. **Remove old view manager instantiation** from extension activation code.

### 7.6 Update view type

If you want to keep backward compatibility with serialized panels (so users don't lose open visualizers on upgrade), consider keeping the same view type `"bicep.visualizer"`. Otherwise, update `VisualDesignerView.viewType` from `"bicep.visualDesigner"` to `"bicep.visualizer"`.

If changing:
- Update `viewType` in the view class
- Update `contributes.webviewPanelSerializer` in `package.json`

### 7.7 Clean up class naming

Rename classes to remove "v2" naming:
- `VisualDesignerViewManager` → `BicepVisualizerViewManager` (or keep `VisualDesignerViewManager` if you prefer the new name)
- `VisualDesignerView` → `BicepVisualizerView` (or keep)

Update all references.

### 7.8 Remove old SVG assets (if not already done in Step 1)

If the old SVG icon files in `src/vscode-bicep/src/visualizer/app/assets/icons/azure/` were not already deleted in Step 1 (they were copied to the shared components), delete them now along with the entire `visualizer/` directory.

### 7.9 Verify no remaining references

Search the codebase for any remaining references to the old visualizer:

```bash
# Search for old imports/references
grep -r "cytoscape" src/vscode-bicep/ --include="*.ts" --include="*.tsx" --include="*.json"
grep -r "visualizer/app" src/vscode-bicep/ --include="*.ts"
grep -r "useCytoscape" src/vscode-bicep/ --include="*.ts"
grep -r "svg-inline-loader" src/vscode-bicep/ --include="*.ts" --include="*.json"
grep -r "importResourceIconInline" src/vscode-bicep/ --include="*.ts"
grep -r "createChildlessNodeBackgroundUri" src/vscode-bicep/ --include="*.ts"
grep -r "react-icons" src/vscode-bicep/ --include="*.ts" --include="*.json"
```

### 7.10 Update documentation

- Update `README.md` or any developer docs that mention the visualizer architecture
- Update `CONTRIBUTING.md` if it references the old build/test process
- Update any CI/CD pipeline configurations that reference the old visualizer

## Verification

1. **Build**:
   - `dotnet build Bicep.sln` — no errors
   - `npm run build` in `src/vscode-bicep-ui` — no errors
   - "Build VSIX" task — no errors
   - Bundle size should be noticeably smaller without Cytoscape.js (~300KB less)

2. **Tests**:
   - `dotnet test` — all pass
   - `npm test` in `src/vscode-bicep` — all pass (remove/update any old visualizer tests)
   - `npm test` in `src/vscode-bicep-ui` — all pass

3. **Functional**:
   - `Bicep: Show Visualizer` works without any feature flag
   - All features from Step 6 still work
   - Webview panel serialization works (close and reopen VS Code)
   - Side-by-side view works
   - Show Source from visualizer works

4. **No regressions**:
   - No console errors in the webview developer tools
   - No extension host errors in the Output panel
   - Memory usage is stable (no leaks from Jotai atom subscriptions)

5. **Clean workspace**:
   - `git status` shows only expected deletions and modifications
   - No orphaned files remaining

## Notes

- **Timing**: This step should only be executed after the new visualizer has been stable in production behind the feature flag for sufficient time. There's no rush — the old code doesn't cost anything while it sits there.
- **Bundle size impact**: Removing Cytoscape.js (~300KB minified), cytoscape-elk (~20KB), elkjs (~600KB, if it was bundled via cytoscape-elk), react-icons (~varies), and svg-inline-loader removes significant bundle weight. However, the new visual designer adds its own dependencies (jotai, motion, d3-drag/zoom, elkjs). Net change should be roughly neutral or slightly smaller.
- **Git history**: The rename from `visualizer-v2/` to `visualizer/` should be done with `git mv` to preserve history.
- **Test data**: If there are integration tests or E2E tests that test the visualizer (search for `showVisualizer`, `deploymentGraph`, `bicep.visualizer` in test files), they need to be updated for the new view type and behavior.
