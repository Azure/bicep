// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import "styled-components";

declare module "styled-components" {
  export interface DefaultTheme {
    name: "light" | "dark" | "high-contrast" | "high-contrast-light";
    canvas: {
      background: string;
      dotColor: string;
    };
    node: {
      background: string;
      compoundBackground: string;
      border: string;
      hoverBorder: string;
      hoverShadow: string;
      hoverErrorShadow: string;
    };
    text: {
      primary: string;
      secondary: string;
    };
    edge: {
      color: string;
    };
    controlBar: {
      background: string;
      border: string;
      icon: string;
      hoverBackground: string;
      activeBackground: string;
    };
    focusBorder: string;
    error: string;
    success: string;
    grabCursor: {
      /** Semi-transparent background color for the cursor overlay (CSS color value). */
      background: string;
      /** Backdrop-filter blur radius in pixels. */
      blur: number;
    };
  }
}
