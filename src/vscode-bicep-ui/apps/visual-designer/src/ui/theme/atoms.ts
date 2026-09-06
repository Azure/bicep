// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { atom } from "jotai";
import { getThemeFromBody } from "./themes";

export const activeThemeAtom = atom<DefaultTheme>(getThemeFromBody());

activeThemeAtom.onMount = (setTheme) => {
  const updateTheme = () => {
    setTheme(getThemeFromBody());
  };

  updateTheme();

  const observer = new MutationObserver(updateTheme);
  observer.observe(document.body, {
    attributes: true,
    attributeFilter: ["data-vscode-theme-kind"],
  });

  return () => observer.disconnect();
};
