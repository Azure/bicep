// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { NodeKind } from "./nodes";

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
  renderContent: (kind: NodeKind, props: NodeContentRenderProps) => ReactNode;
}

export const nodeConfigAtom = atom<NodeConfig>({
  padding: { top: 40, right: 40, bottom: 40, left: 40 },
  renderContent: () => {
    throw new Error("renderContent not initialized.");
  },
});
