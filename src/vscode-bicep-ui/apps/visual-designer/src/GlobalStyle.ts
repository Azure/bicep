// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createGlobalStyle } from "styled-components";

export const GlobalStyle = createGlobalStyle`
  body {
    height: 100vh;
    margin: 0;
    padding: 0;
    display: flex;
    overflow: hidden;
    font-family: var(--vscode-font-family);
  }

  #root {
    flex: 1 1 auto;
    overflow: hidden;
  }
`;
