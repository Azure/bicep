// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { DefaultTheme } from "styled-components";

const fontFamily = getComputedStyle(document.body)
  .getPropertyValue("--vscode-font-family")
  .replace(/"/g, "");

export const darkTheme: DefaultTheme = {
  fontFamily,
  common: {
    foregroundColor: "#ffffff",
    foregroundSecondaryColor: "#c1c1c1",
    errorIndicatorColor: "Red",
    errorFreeIndicatorColor: "Green",
  },
  canvas: {
    backgroundColor: "#111111",
    backgroundImage:
      "radial-gradient(circle at 1px 1px, #3f3f3f 1px, transparent 0)",
  },
  graph: {
    childlessNode: {
      backgroundColor: "#333333",
      borderColor: "#c1c1c1",
      borderOpacity: 0.6,
      borderWidth: 1,
    },
    containerNode: {
      backgroundColor: "Black",
      backgroundOpacity: 0.2,
      borderColor: "#c1c1c1",
      borderOpacity: 0.6,
      borderWidth: 1,
    },
    edge: {
      width: 2,
      opacity: 0.6,
      color: "#c1c1c1",
    },
  },
};

export const lightTheme: DefaultTheme = {
  fontFamily,
  common: {
    foregroundColor: "#323130",
    foregroundSecondaryColor: "#484644",
    errorIndicatorColor: "red",
    errorFreeIndicatorColor: "MediumSpringGreen",
  },
  canvas: {
    backgroundColor: "#ffffff",
    backgroundImage:
      "radial-gradient(circle at 1px 1px, #a0a6af 1px, transparent 0)",
  },
  graph: {
    childlessNode: {
      backgroundColor: "White",
      borderColor: "#484644",
      borderOpacity: 0.6,
      borderWidth: 1,
    },
    containerNode: {
      backgroundColor: "#c0c6cf",
      backgroundOpacity: 0.2,
      borderColor: "#484644",
      borderOpacity: 0.6,
      borderWidth: 1,
    },
    edge: {
      width: 2,
      opacity: 0.6,
      color: "#484644",
    },
  },
};

export const highContrastTheme: DefaultTheme = {
  fontFamily,
  common: {
    foregroundColor: "White",
    foregroundSecondaryColor: "Yellow",
    errorIndicatorColor: "Fuchsia",
    errorFreeIndicatorColor: "Cyan",
  },
  canvas: {
    backgroundColor: "Black",
    backgroundImage: "unset",
  },
  graph: {
    childlessNode: {
      backgroundColor: "#111111",
      borderColor: "Yellow",
      borderOpacity: 1,
      borderWidth: 1,
    },
    containerNode: {
      backgroundColor: "Black",
      backgroundOpacity: 0.2,
      borderColor: "Yellow",
      borderOpacity: 1,
      borderWidth: 1,
    },
    edge: {
      width: 2,
      opacity: 1,
      color: "Yellow",
    },
  },
};
