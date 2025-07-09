// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Preview } from "@storybook/react-vite";

import { withThemeFromJSXProvider } from "@storybook/addon-themes";
import { createGlobalStyle } from "styled-components";

const GlobalStyles = createGlobalStyle`
  body {
    font-family: Segoe WPC,Segoe UI,sans-serif;
    font-size: 13px;
  }
`;

const preview: Preview = {
  parameters: {
    actions: { argTypesRegex: "^on[A-Z].*" },
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/,
      },
    },
  },
};

export const decorators = [
  withThemeFromJSXProvider({
    GlobalStyles, // Adds your GlobalStyle component to all stories
  }),
];

export default preview;
