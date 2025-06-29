// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useEffect, useState } from "react";

type colorMode = "dark" | "light";

export function getColorMode(): colorMode {
  return window.matchMedia("(prefers-color-scheme: dark)").matches
    ? "dark"
    : "light";
}

export function useColorMode() {
  const [colorMode, setColorMode] = useState<colorMode>(getColorMode());

  useEffect(() => {
    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const updateColorMode = () => setColorMode(getColorMode());

    mediaQuery.addEventListener("change", updateColorMode);
    return () => {
      mediaQuery.removeEventListener("change", updateColorMode);
    };
  }, []);

  return colorMode;
}
