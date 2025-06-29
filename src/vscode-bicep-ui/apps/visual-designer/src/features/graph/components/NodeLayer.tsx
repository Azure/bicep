// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { nodesAtom } from "../atoms";
import { CompoundNode } from "./CompoundNode";
import { PrimitiveNode } from "./PrimitiveNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesAtom);

  return Object.entries(nodes).map(([id, node]) =>
    node.kind === "primitive" ? <PrimitiveNode key={id} {...node} /> : <CompoundNode key={id} {...node} />,
  );
}
