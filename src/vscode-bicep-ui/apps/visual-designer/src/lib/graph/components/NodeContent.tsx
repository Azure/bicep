// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeKind } from "../atoms";

import { useAtomValue } from "jotai";
import { nodeConfigAtom } from "../atoms";

export interface NodeContentProps {
  id: string;
  kind: NodeKind;
  data: unknown;
}

export function NodeContent({ id, kind, data }: NodeContentProps) {
  const nodeConfig = useAtomValue(nodeConfigAtom);

  return nodeConfig.renderContent(kind, { id, data });
}
