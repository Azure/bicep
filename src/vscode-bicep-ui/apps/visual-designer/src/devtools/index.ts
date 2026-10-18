// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType, LazyExoticComponent, ReactNode } from "react";

import { lazy } from "react";

/**
 * Development scaffolding: a fake extension host so the webview runs standalone.
 *
 * This is deliberately *not* a feature. Features are slices of the product; devtools impersonates
 * the other side of the wire, so it consumes host contracts exposed by core and feature barrels.
 * Nothing but `app` may import it.
 */

/**
 * Lazily load the {@link DevAppShell} component. Returns `undefined` in production builds
 * (`import.meta.env.DEV === false`), allowing Rollup to tree-shake the entire devtools chunk.
 */
export function loadDevAppShell(): LazyExoticComponent<ComponentType<{ children: ReactNode }>> | undefined {
  if (!import.meta.env.DEV) {
    return undefined;
  }

  return lazy(() => import("./components/DevAppShell").then((m) => ({ default: m.DevAppShell })));
}
