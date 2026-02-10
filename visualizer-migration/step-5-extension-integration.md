# Step 5: Integrate Visual Designer Webview into VS Code Extension

## Goal

Wire the Vite-built visual designer app into the VS Code extension as a webview panel, following the established deploy-pane integration pattern. Add a feature flag so both old and new visualizers can coexist during migration.

## Background

### Existing integration patterns

The extension has two webview integration patterns:

1. **Old visualizer** (`src/vscode-bicep/src/visualizer/`): Webpack-bundled inside `vscode-bicep`, loaded as `out/visualizer.js`.
2. **Deploy pane** (`src/vscode-bicep/src/panes/deploy/`): Vite-built in `vscode-bicep-ui`, copied into `out/deploy-pane/` by webpack's `CopyPlugin`, loaded as `out/deploy-pane/index.js` + `out/deploy-pane/assets/index.css`.

The new visual designer should follow pattern #2 (deploy pane).

### Key differences from old visualizer

The new integration needs to handle the `@vscode-bicep-ui/messaging` RPC protocol instead of the old `kind`-discriminated union messages. The messaging library uses:
- **Notifications**: `{ method: string, params?: unknown }` — fire-and-forget
- **Requests**: `{ id: string, method: string, params?: unknown }` → `{ id: string, result?: unknown, error?: unknown }`

## Tasks

### 5.1 Add CopyPlugin entry to webpack config

Update `src/vscode-bicep/webpack.config.ts` to copy the visual designer's `dist/` output into the extension's `out/` directory:

```typescript
// In extensionConfig.plugins, add a new CopyPlugin pattern:
new CopyPlugin({
  patterns: [
    {
      from: "../vscode-bicep-ui/apps/visual-designer/dist",
      to: path.join(__dirname, "out/visual-designer"),
      globOptions: {
        ignore: ["**/index.html"],
      },
    },
  ],
}),
```

This copies the Vite build output (JS, CSS, chunks) into `out/visual-designer/` during the extension build.

### 5.2 Add the feature flag

Add a configuration setting in `src/vscode-bicep/package.json` under `contributes.configuration`:

```json
{
  "bicep.experimental.visualDesigner": {
    "type": "boolean",
    "default": false,
    "description": "Use the new React-based visual designer instead of the Cytoscape.js visualizer."
  }
}
```

### 5.3 Create the new view manager

**File**: `src/vscode-bicep/src/visualizer-v2/viewManager.ts`

Model this after the existing `src/vscode-bicep/src/visualizer/viewManager.ts`:

```typescript
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { VisualDesignerView } from "./view";

export class VisualDesignerViewManager extends Disposable implements vscode.WebviewPanelSerializer {
  private static readonly visualizerActiveContextKey = "bicepVisualizerFocus";
  private readonly viewsByPath = new Map<string, VisualDesignerView>();
  private activeUri: vscode.Uri | undefined = undefined;

  constructor(
    private readonly extensionUri: vscode.Uri,
    private readonly languageClient: LanguageClient,
  ) {
    super();
    this.register(
      vscode.window.registerWebviewPanelSerializer(VisualDesignerView.viewType, this)
    );

    // Hook into diagnostics middleware to re-render on file changes
    const existingMiddleware = languageClient.clientOptions.middleware?.handleDiagnostics;
    this.languageClient.clientOptions.middleware = {
      ...(this.languageClient.clientOptions.middleware ?? {}),
      handleDiagnostics: (uri, diagnostics, next) => {
        for (const view of this.viewsByPath.values()) {
          view.render();
        }
        if (existingMiddleware) {
          existingMiddleware(uri, diagnostics, next);
        } else {
          next(uri, diagnostics);
        }
      },
    };
  }

  get activeDocumentUri(): vscode.Uri | undefined {
    return this.activeUri;
  }

  public async openView(documentUri: vscode.Uri, viewColumn: vscode.ViewColumn): Promise<void> {
    const existingView = this.viewsByPath.get(documentUri.fsPath);
    if (existingView) {
      existingView.reveal();
      return;
    }
    this.registerView(
      documentUri,
      VisualDesignerView.create(this.languageClient, viewColumn, this.extensionUri, documentUri),
    );
    await this.setVisualizerActiveContext(true);
    this.activeUri = documentUri;
  }

  public async deserializeWebviewPanel(webviewPanel: vscode.WebviewPanel, documentPath: string): Promise<void> {
    const documentUri = vscode.Uri.file(documentPath);
    this.registerView(
      documentUri,
      VisualDesignerView.revive(this.languageClient, webviewPanel, this.extensionUri, documentUri),
    );
  }

  // ... registerView, dispose, setVisualizerActiveContext — same pattern as existing viewManager
}
```

### 5.4 Create the new view

**File**: `src/vscode-bicep/src/visualizer-v2/view.ts`

Key differences from the old view:

1. **Asset loading**: Load from `out/visual-designer/index.js` and `out/visual-designer/assets/index.css`
2. **Script type**: Use `type="module"` (Vite output is ESM)
3. **Message protocol**: Handle the `@vscode-bicep-ui/messaging` format instead of `kind`-based messages

```typescript
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import crypto from "crypto";
import path from "path";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { deploymentGraphRequestType } from "../language";
import { Disposable } from "../utils/disposable";
import { getLogger } from "../utils/logger";
import { debounce } from "../utils/time";

export class VisualDesignerView extends Disposable {
  public static viewType = "bicep.visualDesigner";

  private readyToRender = false;
  // ... same pattern as BicepVisualizerView for lifecycle

  // Handle incoming messages from the webview
  private handleDidReceiveMessage(message: unknown): void {
    if (!message || typeof message !== "object") return;

    // Handle notification messages (method-based, no id)
    if ("method" in message && !("id" in message)) {
      const notification = message as { method: string; params?: unknown };

      switch (notification.method) {
        case "ready":
          getLogger().debug(`Visual Designer for ${this.documentUri.fsPath} is ready.`);
          this.readyToRender = true;
          this.render();
          return;

        case "revealFileRange":
          const payload = notification.params as { filePath: string; range: vscode.Range };
          this.revealFileRange(payload.filePath, payload.range);
          return;
      }
    }

    // Handle request messages (have id — need response)
    if ("id" in message && "method" in message) {
      const request = message as { id: string; method: string; params?: unknown };
      // Future: handle request/response patterns if needed
      // For now, no requests are expected from the webview
      getLogger().warn(`Unhandled request method: ${request.method}`);
    }
  }

  // Send deployment graph as a notification to the webview
  private async doRender() {
    if (this.isDisposed || !this.readyToRender) return;

    let document: vscode.TextDocument;
    try {
      document = await vscode.workspace.openTextDocument(this.documentUri);
    } catch {
      this.webviewPanel.webview.html = this.createDocumentNotFoundHtml();
      return;
    }

    if (this.isDisposed) return;

    const deploymentGraph = await this.languageClient.sendRequest(
      deploymentGraphRequestType,
      {
        textDocument: this.languageClient.code2ProtocolConverter.asTextDocumentIdentifier(document),
      },
    );

    if (this.isDisposed) return;

    try {
      // Send as a notification using the messaging library's format
      await this.webviewPanel.webview.postMessage({
        method: "deploymentGraph",
        params: {
          documentPath: this.documentUri.fsPath,
          deploymentGraph,
        },
      });
    } catch (error) {
      getLogger().debug((error as Error).message ?? error);
    }
  }

  // Generate webview HTML — follows deploy-pane pattern
  private createWebviewHtml() {
    const { cspSource } = this.webviewPanel.webview;
    const nonce = crypto.randomBytes(16).toString("hex");

    const scriptUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "visual-designer", "index.js"),
    );
    const cssUri = this.webviewPanel.webview.asWebviewUri(
      vscode.Uri.joinPath(this.extensionUri, "out", "visual-designer", "assets", "index.css"),
    );

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; script-src 'nonce-${nonce}' vscode-webview-resource:; font-src data: ${cspSource};">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link rel="stylesheet" nonce="${nonce}" href="${cssUri}">
      </head>
      <body>
        <div id="root"></div>
        <script nonce="${nonce}" type="module" src="${scriptUri}" />
      </body>
      </html>`;
  }
}
```

### 5.5 Create the index barrel

**File**: `src/vscode-bicep/src/visualizer-v2/index.ts`

```typescript
export { VisualDesignerViewManager } from "./viewManager";
```

### 5.6 Update command handlers to dispatch based on feature flag

Update `src/vscode-bicep/src/commands/showVisualizer.ts`:

```typescript
import { BicepVisualizerViewManager } from "../visualizer";
import { VisualDesignerViewManager } from "../visualizer-v2";

async function showVisualizer(
  context: IActionContext,
  viewManager: BicepVisualizerViewManager,
  viewManagerV2: VisualDesignerViewManager,
  documentUri: vscode.Uri | undefined,
  sideBySide = false,
) {
  documentUri = await findOrCreateActiveBicepFile(context, documentUri, "Choose which Bicep file to visualize");
  const viewColumn = sideBySide
    ? vscode.ViewColumn.Beside
    : (vscode.window.activeTextEditor?.viewColumn ?? vscode.ViewColumn.One);

  const useNewVisualizer = vscode.workspace
    .getConfiguration("bicep.experimental")
    .get<boolean>("visualDesigner", false);

  if (useNewVisualizer) {
    await viewManagerV2.openView(documentUri, viewColumn);
  } else {
    await viewManager.openView(documentUri, viewColumn);
  }

  return viewColumn;
}
```

The command classes need to accept both view managers:

```typescript
export class ShowVisualizerCommand implements Command {
  public readonly id = "bicep.showVisualizer";
  public constructor(
    private readonly viewManager: BicepVisualizerViewManager,
    private readonly viewManagerV2: VisualDesignerViewManager,
  ) {}
  public async execute(context: IActionContext, documentUri?: vscode.Uri) {
    return await showVisualizer(context, this.viewManager, this.viewManagerV2, documentUri);
  }
}
```

Similarly update `ShowVisualizerToSideCommand` and `ShowSourceFromVisualizerCommand`.

### 5.7 Register the new view manager in the extension activation

Find the extension's activation code (where `BicepVisualizerViewManager` is created) and add `VisualDesignerViewManager` alongside it.

Search for where `BicepVisualizerViewManager` is instantiated in the extension code (likely in `src/vscode-bicep/src/extension.ts` or a similar registration file):

```typescript
const viewManagerV2 = new VisualDesignerViewManager(extensionUri, languageClient);
// Pass viewManagerV2 to commands that need it
```

### 5.8 Update build pipeline

Ensure the build order is correct:

1. `vscode-bicep-ui` builds first (produces `apps/visual-designer/dist/`)
2. `vscode-bicep` webpack build copies `dist/` into `out/visual-designer/`

The existing `.vscode/tasks.json` has a "Build VSIX" task that depends on "Build VSCode UI". Verify this dependency chain includes the visual designer.

Also update the webpack `extensionConfig` to exclude the new `visualizer-v2/` directory from the server-side TS loader (since it's purely extension-host code and should be compiled by the extensionConfig, but should NOT include any webview app code):

```typescript
// In extensionConfig.module.rules[0].exclude:
exclude: [/node_modules/, /panes\/deploy\/app/, /visualizer\/app/, /test/],
// No change needed — visualizer-v2/ contains only Node.js code, no "app" subdirectory
```

### 5.9 Handle CSP for Vite chunks

Vite may produce chunk files in `chunks/` directory (per `vite.config.ts` rollup output config). Ensure the CSP in the webview HTML allows loading scripts from `chunks/`. The CSP `script-src 'nonce-${nonce}'` should work since all scripts are loaded via `<script>` tags, but dynamic `import()` chunks may need `script-src-elem` or similar. Test and adjust CSP as needed.

If dynamic imports fail, add `'unsafe-eval'` temporarily for debugging (remove before shipping), or configure Vite to inline all chunks:

```typescript
// vite.config.ts - if chunking causes CSP issues:
build: {
  rollupOptions: {
    output: {
      manualChunks: undefined,  // disable code splitting
      inlineDynamicImports: true,
    },
  },
}
```

### 5.10 Handle `WebviewPanelSerializer` view type

The new view uses a different `viewType` (`"bicep.visualDesigner"` vs `"bicep.visualizer"`). Register it in `package.json`:

```json
{
  "contributes": {
    "webviewPanelSerializer": [
      {
        "viewType": "bicep.visualDesigner"
      }
    ]
  }
}
```

Or, if both visualizers must serialize/restore, ensure both serializers are registered.

## File Structure After This Step

```
src/vscode-bicep/src/
  visualizer/          ← OLD (unchanged)
  visualizer-v2/       ← NEW
    index.ts
    view.ts
    viewManager.ts
  commands/
    showVisualizer.ts  ← MODIFIED (dispatch based on feature flag)
    showSourceFromVisualizer.ts  ← MODIFIED
```

## Verification

1. **Build**: Run "Build VSIX" task — should succeed without errors.
2. **Feature flag off** (default): Run `Bicep: Show Visualizer` — old Cytoscape visualizer opens.
3. **Feature flag on**: Set `"bicep.experimental.visualDesigner": true` in VS Code settings, run `Bicep: Show Visualizer` — new visual designer opens.
4. **New visualizer functionality**:
   - Opens with correct title: `Visualize <filename>.bicep`
   - Shows the graph for the current file
   - Updates when the file is edited
   - Persists across window reload (serialization)
   - Shows document-not-found message if file is deleted
5. **Side-by-side**: `Bicep: Show Visualizer to Side` opens beside the editor.
6. **Show Source**: `Bicep: Show Source` navigates back to the .bicep file.
7. **Multiple files**: Opening visualizers for different files creates separate panels.

## Notes

- The two view managers should NOT interfere with each other's `handleDiagnostics` middleware. They can both hook in — each will re-render their own views when diagnostics change.
- The `bicepVisualizerFocus` context key is shared. If both old and new views exist, the last focused one sets the context. This is fine since only one view manager manages views based on the flag.
- `retainContextWhenHidden: true` is important for the webview panel — it prevents the webview from being destroyed when the user switches to another tab, preserving pan/zoom state and loaded data.
