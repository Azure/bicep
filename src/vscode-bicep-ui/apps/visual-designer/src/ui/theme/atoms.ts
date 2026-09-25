// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";

import { atom } from "jotai";
import { getThemeFromBody } from "./themes";

export const activeThemeAtom = atom<DefaultTheme>(getThemeFromBody());

activeThemeAtom.onMount = (setTheme) => {
  let disposed = false;
  const updateTheme = () => {
    if (!disposed) {
      setTheme(getThemeFromBody());
    }
  };

  // onMount runs before the mounting subscriber's listener is attached, and Jotai does not re-read
  // after subscribing, so a synchronous set here would be missed (e.g. when the host sets the theme
  // kind between module evaluation and first mount). Defer the initial sync until listeners exist.
  queueMicrotask(updateTheme);

  const observer = new MutationObserver(updateTheme);
  observer.observe(document.body, {
    attributes: true,
    attributeFilter: ["data-vscode-theme-kind"],
  });

  return () => {
    disposed = true;
    observer.disconnect();
  };
};
