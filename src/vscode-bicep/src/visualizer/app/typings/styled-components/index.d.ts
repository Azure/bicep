// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import "styled-components";

declare module "styled-components" {
  export interface DefaultTheme {
    name: "dark" | "light" | "high-contrast";
    fontFamily: string;
    common: {
      foregroundColor: string;
      foregroundSecondaryColor: string;
      errorIndicatorColor: string;
      errorFreeIndicatorColor: string;
    };
    canvas: {
      backgroundColor: string;
      backgroundImage: string;
    };
    graph: {
      childlessNode: {
        backgroundColor: string;
        borderColor: string;
        borderOpacity: number;
        borderWidth: number;
      };
      containerNode: {
        backgroundColor: string;
        backgroundOpacity: number;
        borderColor: string;
        borderOpacity: number;
        borderWidth: number;
      };
      edge: {
        color: string;
        opacity: number;
        width: number;
      };
    };
    commandBarButton: {
      backgroundColor: string;
    };
  }
}
