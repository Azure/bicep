// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { nodesByIdAtom } from "../atoms";
import { AtomicNode } from "./AtomicNode";
import { CompoundNode } from "./CompoundNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesByIdAtom);

  return Object.entries(nodes).map(([id, node]) =>
    node.kind === "atomic" ? <AtomicNode key={id} {...node} /> : <CompoundNode key={id} {...node} />,
  );
}
