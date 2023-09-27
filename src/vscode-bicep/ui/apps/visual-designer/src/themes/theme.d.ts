// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import "styled-components";

declare module "styled-components" {
  export interface DefaultTheme {
    fontFamily: string;
    boarderRadius: string;
    boarderWidth: string;
    edgeWidth: string;
    colors: {
      foreground: string;
      secondaryForeground: string;
      error: string;
      warning: string;
      canvasBackground: string;
      nodeBackground: string;
      nodeBorder: string;
      edge: string;
      commandBarBackground: string;
      commandBarHoverBackground: string;
    };
  }
}
