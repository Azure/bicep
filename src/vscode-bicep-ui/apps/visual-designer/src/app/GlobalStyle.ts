// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createGlobalStyle } from "styled-components";
import { getCursors } from "@/ui";

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
    color: ${({ theme }) => theme.text.primary};
    line-height: 1.5;
  }

  #root {
    position: relative;
    flex: 1 1 auto;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    background-color: ${({ theme }) => theme.viewport.background};
    /* The designer's own arrow, like its tool and button cursors. Text fields keep the text cursor. */
    cursor: ${({ theme }) => getCursors(theme).arrow};
  }

  input:not([type="checkbox"], [type="radio"], [type="range"], [type="button"]),
  textarea {
    cursor: text;
  }

  /* Browsers give labels the system arrow; let them show whatever is around them instead. */
  label {
    cursor: inherit;
  }

  *, *::before, *::after {
    box-sizing: border-box;
  }
`;
