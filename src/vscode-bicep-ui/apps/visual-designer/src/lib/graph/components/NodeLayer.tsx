// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue, useSetAtom } from "jotai";
import { useLayoutEffect } from "react";
import { mountedNodesByIdAtom, nodesByIdAtom } from "../atoms";
import { AtomicNode } from "./AtomicNode";
import { CompoundNode } from "./CompoundNode";

export function NodeLayer() {
  const nodes = useAtomValue(nodesByIdAtom);
  const setMountedNodes = useSetAtom(mountedNodesByIdAtom);

  // Children's layout effects run before their parent's, so every node rendered here has mounted by now.
  useLayoutEffect(() => {
    setMountedNodes(nodes);
  }, [nodes, setMountedNodes]);

  // Separate, so it runs only on unmount: a cleanup of the effect above would also run before each update's nodes
  // have measured themselves.
  useLayoutEffect(() => () => setMountedNodes(undefined), [setMountedNodes]);

  return Object.entries(nodes).map(([id, node]) =>
    node.kind === "atomic" ? <AtomicNode key={id} {...node} /> : <CompoundNode key={id} {...node} />,
  );
}
