// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import ReactDOM from "react-dom";
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

ReactDOM.render(
  <>
    <GlobalStyle />
    <App />
  </>,
  document.getElementById("root")
);
