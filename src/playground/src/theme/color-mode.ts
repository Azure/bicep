// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useEffect, useState } from "react";

export type ColorMode = "dark" | "light";
const colorModeChangeEvent = "playground-color-mode-change";

export function getPreferredColorMode(): ColorMode {
  return window.matchMedia("(prefers-color-scheme: dark)").matches
    ? "dark"
    : "light";
}

export function getColorMode(): ColorMode {
  return document.documentElement.dataset.colorMode === "dark"
    ? "dark"
    : "light";
}

export function setColorMode(colorMode: ColorMode): void {
  document.documentElement.dataset.colorMode = colorMode;
  window.dispatchEvent(new Event(colorModeChangeEvent));
}

export function useColorMode() {
  const [colorMode, setCurrentColorMode] = useState<ColorMode>(getColorMode());

  useEffect(() => {
    const updateColorMode = () => setCurrentColorMode(getColorMode());
    window.addEventListener(colorModeChangeEvent, updateColorMode);
    return () => {
      window.removeEventListener(colorModeChangeEvent, updateColorMode);
    };
  }, []);

  return colorMode;
}
