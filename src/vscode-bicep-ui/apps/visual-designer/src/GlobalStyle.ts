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
    font-family: var(--vscode-font-family, sans-serif);
    background-color: ${({ theme }) => theme.canvas.background};
    color: ${({ theme }) => theme.text.primary};
  }

  #root {
    flex: 1 1 auto;
    overflow: hidden;
  }
`;
