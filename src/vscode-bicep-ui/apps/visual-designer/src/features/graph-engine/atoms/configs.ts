// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ComponentType } from "react";
import type { NodeState } from "./nodes";

import { atom } from "jotai";

export interface Padding {
  top: number;
  right: number;
  bottom: number;
  left: number;
}

export interface NodeConfig {
  padding: Padding;
  getContentComponent: (kind: NodeState["kind"], data: unknown) => ComponentType<{ id: string; data: unknown }>;
}

export const nodeConfigAtom = atom<NodeConfig>({
  padding: { top: 40, right: 40, bottom: 40, left: 40 },
  getContentComponent: () => {
    throw new Error("getContentComponent not initialized.");
  },
});
