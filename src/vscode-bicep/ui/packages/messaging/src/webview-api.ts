import type { WebviewApi } from "vscode-webview";

export const webviewApi: WebviewApi<unknown> = acquireVsCodeApi();
