// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { NodeState } from "./nodes";

import { atom } from "jotai";

export interface Padding {
  top: number;
  right: number;
  bottom: number;
  left: number;
}

export interface NodeContentRenderProps {
  id: string;
  data: unknown;
}

export interface NodeConfig {
  padding: Padding;
  renderContent: (kind: NodeState["kind"], props: NodeContentRenderProps) => ReactNode;
  /**
   * Invoked when the user activates a node (double-click). Optional: a graph with no activation
   * behaviour is legitimate, which is why this does not throw the way `renderContent` does.
   */
  onNodeActivate?: (id: string, data: unknown) => void;
}

export const nodeConfigAtom = atom<NodeConfig>({
  padding: { top: 40, right: 40, bottom: 40, left: 40 },
  renderContent: () => {
    throw new Error("renderContent not initialized.");
  },
});
