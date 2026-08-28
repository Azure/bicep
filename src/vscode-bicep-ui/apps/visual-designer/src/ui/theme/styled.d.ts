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
      /** Border width for node cards. Thicker in high-contrast themes. */
      borderWidth: string;
      /** Border width for error nodes. Slightly thicker than default in normal themes. */
      errorBorderWidth: string;
      /** Resting box-shadow for default node elevation. */
      shadow: string;
      hoverBorder: string;
      hoverShadow: string;
      hoverErrorShadow: string;
      focusBorder: string;
      /** Box-shadow applied when a node is focused/selected. */
      selectedShadow: string;
      /** Box-shadow applied when an error node is focused/selected. Uses error color. */
      selectedErrorShadow: string;
      /** Subtle accent color used for the left-edge indicator on resource nodes. */
      accentBorder: string;
      /** Accent color for module (compound) nodes. */
      moduleAccent: string;
      /** Pixel offset for collection stack pseudo-element. Larger in high-contrast. */
      collectionOffset: number;
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
