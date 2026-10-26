// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * The theme tokens the graph engine needs.
 *
 * The engine reads these through styled-components' `ThemeProvider` context, which means it consumes
 * them without importing anything — invisible to both the reader and the layer lint rule. Declaring
 * the requirement here makes it explicit and compile-checked: `ui/theme` satisfies this interface, so
 * removing a token the engine depends on is a type error rather than a runtime surprise.
 *
 * Reading theme is not a layer violation. The test for `lib` is Bicep knowledge, not styling, and a
 * dot grid and an edge colour have none. What matters is that the engine states what it needs instead
 * of reaching into a shape the app happens to own.
 */
export interface GraphTheme {
  viewport: {
    background: string;
    dotColor: string;
  };
  edge: {
    color: string;
  };
  grabCursor: {
    /** Semi-transparent background color for the cursor overlay (CSS color value). */
    background: string;
    /** Backdrop-filter blur radius in pixels. */
    blur: number;
  };
}
