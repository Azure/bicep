// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom } from "jotai";
import type { NodeKind } from "../atoms";

import { useAtomValue } from "jotai";
import { nodeConfigAtom } from "../atoms";

export interface NodeContentProps {
  id: string;
  kind: NodeKind;
  dataAtom: Atom<unknown>;
}

export function NodeContent({ id, kind, dataAtom }: NodeContentProps) {
  const nodeConfig = useAtomValue(nodeConfigAtom);
  const data = useAtomValue(dataAtom);
  const NodeContentComponent = nodeConfig.getContentComponent(kind, data);

  return <NodeContentComponent id={id} data={data} />;
}
