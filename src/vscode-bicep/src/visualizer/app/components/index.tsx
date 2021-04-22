// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import ReactDOM from "react-dom";
import { createGlobalStyle } from "styled-components";

import { App } from "./App";

const GlobalStyle = createGlobalStyle`
  body {
    font-family: Segoe UI, Helvetica Neue, Helvetica, sans-serif;
    height: 100vh;
    margin: 0;
    padding: 0;
    display: flex;
  }

  #root {
    flex: 1 1 auto;
    background-color: #111111;
    background-image: radial-gradient(circle at 1px 1px, #3f3f3f 1px, transparent 0);
    background-size: 24px 24px;
    background-position: 12px 12px;
  }
`;

ReactDOM.render(
  <>
    <GlobalStyle />
    <App />
  </>,
  document.getElementById("root")
);
