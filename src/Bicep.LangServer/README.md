# Bicep Language Server

`Bicep.LangServer` implements the Bicep language server and its Bicep-specific language tooling protocol.

## Folder structure

The project is organized primarily by protocol feature. The standard feature groups map to the [Language Server Protocol 3.18 specification](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/).

```text
Features/
  Language/    # Text document language features.
  Workspace/   # Workspace protocol features.
  Custom/      # Bicep-specific requests, notifications, and commands.
```

### Language features

`Features/Language` contains standard LSP text-document capabilities. Each folder is named after the protocol feature it implements, not after its handler type.

| Folder | LSP method family |
| --- | --- |
| `CodeAction` | [`textDocument/codeAction`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_codeAction) |
| `CodeLens` | [`textDocument/codeLens`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_codeLens) |
| `Completion` | [`textDocument/completion`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_completion) |
| `Definition` | [`textDocument/definition`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_definition) |
| `DocumentFormatting` | [`textDocument/formatting`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_formatting) |
| `DocumentHighlight` | [`textDocument/documentHighlight`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_documentHighlight) |
| `DocumentLink` | [`textDocument/documentLink`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_documentLink) |
| `DocumentSymbol` | [`textDocument/documentSymbol`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_documentSymbol) |
| `Hover` | [`textDocument/hover`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_hover) |
| `PrepareRename` | [`textDocument/prepareRename`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_prepareRename) |
| `References` | [`textDocument/references`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_references) |
| `Rename` | [`textDocument/rename`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_rename) |
| `SemanticTokens` | [`textDocument/semanticTokens/*`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_semanticTokens) |
| `SignatureHelp` | [`textDocument/signatureHelp`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_signatureHelp) |
| `TextDocumentSync` | [`textDocument/didOpen`, `didChange`, `didClose`, and `didSave`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#textDocument_synchronization) |

`Completion/Snippets` and `Completion/SyntaxPatterns` are feature-local subdomains. They are kept separate because each contains several related types; completion providers and services remain directly in `Completion`.

### Workspace features

`Features/Workspace` contains standard LSP workspace capabilities and state established during initialization.

| Folder | LSP method family or lifecycle |
| --- | --- |
| `DidChangeConfiguration` | [`workspace/didChangeConfiguration`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#workspace_didChangeConfiguration) |
| `DidChangeWatchedFiles` | [`workspace/didChangeWatchedFiles`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#workspace_didChangeWatchedFiles) |

### Custom features

`Features/Custom` contains Bicep-specific functionality that is not a standard LSP feature. A custom feature may use `workspace/executeCommand` as its transport, but its handler belongs to the capability folder rather than to a transport-specific folder.

| Folder | Bicep protocol methods or commands |
| --- | --- |
| `Build` | `build`, `buildParams` |
| `Configuration` | `createConfigFile`, `bicep/getRecommendedConfigLocation` |
| `Decompile` | `decompile`, `decompileForPaste`, `decompileParams`, `decompileSave` |
| `Deployments` | `bicep/getDeploymentData`, `deploy/start`, `deploy/waitForCompletion`, deployment scope and parameter commands |
| `ImportKubernetesManifest` | `bicep/importKubernetesManifest` |
| `InsertResource` | `textDocument/insertResource` |
| `Linter` | `bicep.EditLinterRule` |
| `LocalDeploy` | `bicep/localDeploy` |
| `ModuleRestore` | `forceModulesRestore` and the module restore scheduler |
| `Parameters` | `generateParams` |
| `Telemetry` | Telemetry command handling and telemetry event types |
| `Visualization` | `textDocument/visualGraphUpdate`, `textDocument/visualGraphLayout`, and `textDocument/visualGraphNodeSource` |

`Deployments/Services` and `Visualization/Models` are feature-local subdomains with several supporting types. Their handlers, requests, and responses remain directly in the feature folder.

### Shared infrastructure

Code shared across multiple features remains outside `Features`:

- `Compilation` contains language-server compilation state, compilation contexts, and the compilation provider.
- `ClientCapabilities` contains cross-feature client capability negotiation state established during [`initialize`](https://microsoft.github.io/language-server-protocol/specifications/lsp/3.18/specification/#initialize).
- `BicepConfig` contains Bicep configuration lifecycle support shared by language and workspace features.
- `Extensions`, `Options`, and `Utils` contain cross-cutting support code; `Utils` includes reusable JSON editing.
- `Files` contains embedded assets.
- `Settings` contains the client settings cache shared by the configuration notification and completion features.

## Organization guidelines

- Place a type in the folder for the feature or shared domain it implements.
- Keep feature folders shallow. Add a child folder only for a cohesive multi-file subdomain, such as `Deployments/Services`, `Completion/Snippets`, `Completion/SyntaxPatterns`, or `Visualization/Models`.
- Keep Bicep-specific protocol features under `Features/Custom`, even when they use an LSP transport method.
- Namespaces currently preserve the pre-reorganization layout to keep this move-focused change easy to review. Update namespaces only in a dedicated follow-up change.
