// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ResourceTypeReference } from "../types";

import { createContext } from "react";

export interface CanvasActions {
  /**
   * Create a resource at a client-coordinate point. Omit `clientPoint` to use the canvas's default
   * placement, which is how keyboard activation creates a resource.
   */
  createResource: (resourceType: ResourceTypeReference, clientPoint?: Point) => Promise<void>;
  /** Whether a resource can be placed at a client-coordinate point on the canvas. */
  canPlaceResourceAt: (clientPoint: Point) => boolean;
  /** Re-run graph layout without changing the user's viewport. */
  resetGraphLayout: () => Promise<void>;
}

export const CanvasActionsContext = createContext<CanvasActions | undefined>(undefined);
