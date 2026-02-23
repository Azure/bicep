// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createGlobalStyle } from "styled-components";

export const GlobalStyle = createGlobalStyle`
  body {
    height: 100vh;
    margin: 0;
    padding: 0;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    font-family: var(--vscode-font-family, "Segoe WPC", "Segoe UI", system-ui, "Ubuntu", "Droid Sans", sans-serif);
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    background-color: ${({ theme }) => theme.canvas.background};
    color: ${({ theme }) => theme.text.primary};
    line-height: 1.5;
  }

  #root {
    position: relative;
    flex: 1 1 auto;
    overflow: hidden;
    display: flex;
    flex-direction: column;
  }

  *, *::before, *::after {
    box-sizing: border-box;
  }
`;
