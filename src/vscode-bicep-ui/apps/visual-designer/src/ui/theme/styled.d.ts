// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import "styled-components";

import type { GraphTheme } from "@/lib/graph";

declare module "styled-components" {
  /**
   * `GraphTheme` carries the tokens `lib/graph` requires. Extending it makes the app's theme the
   * thing that satisfies the engine's contract, so dropping a token the engine reads fails to
   * compile.
   */
  export interface DefaultTheme extends GraphTheme {
    name: "light" | "dark" | "high-contrast" | "high-contrast-light";
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
    /** Chrome for floating panels layered over the viewport. */
    panel: {
      background: string;
      border: string;
    };
    /** Icon button states, used inside panels and toolbars. */
    iconButton: {
      color: string;
      hoverBackground: string;
      activeBackground: string;
    };
    focusBorder: string;
    error: string;
    success: string;
  }
}
