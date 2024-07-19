// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createRoot } from "react-dom/client";
import { createGlobalStyle } from "styled-components";
import { App } from "./App";

const GlobalStyle = createGlobalStyle`
  body {
    height: 100vh;
    margin: 0;
    padding: 0;
    display: flex;
    overflow: hidden;
  }

  #root {
    flex: 1 1 auto;
    overflow: hidden;
  }
`;

const container = document.getElementById("root");

if (!container) {
  throw new Error("Could not find the root element");
}

const root = createRoot(container);

root.render(
  <>
    <GlobalStyle />
    <App />
  </>,
);
