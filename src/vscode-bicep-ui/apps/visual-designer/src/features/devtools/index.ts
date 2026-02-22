// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType, LazyExoticComponent, ReactNode } from "react";

import { lazy } from "react";

/**
 * Lazily load the {@link DevAppShell} component.
 *
 * Returns `undefined` in production builds (`import.meta.env.DEV === false`),
 * allowing Rollup to tree-shake the entire devtools chunk.
 */
export function loadDevAppShell(): LazyExoticComponent<ComponentType<{ children: ReactNode }>> | undefined {
  if (!import.meta.env.DEV) {
    return undefined;
  }

  return lazy(() => import("./components/DevAppShell").then((m) => ({ default: m.DevAppShell })));
}
