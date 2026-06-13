// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from "react";
import ReactDOM from "react-dom/client";
import { App } from "./App";

if (import.meta.env.DEV) {
  import("@vscode-elements/webview-playground");
  document.body.appendChild(document.createElement("vscode-dev-toolbar"));
}

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
