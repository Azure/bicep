// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { WebviewMessageChannelProvider } from "@vscode-bicep-ui/messaging";
import React from "react";
import ReactDOM from "react-dom/client";
import { createGlobalStyle } from "styled-components";
import { App } from "./components/App";

export const GlobalStyle = createGlobalStyle`
  body {
    margin: 0;
    padding: 0;
    overflow-x: hidden;
    font-size: 13px;
    font-family: Segoe WPC,Segoe UI,sans-serif;
    user-select: none;
  }
`;

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <GlobalStyle />
    <WebviewMessageChannelProvider>
      <App />
    </WebviewMessageChannelProvider>
  </React.StrictMode>,
);
