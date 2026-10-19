// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CompoundNodeState } from "../atoms/nodes";

import { useStore } from "jotai";
import { useRef } from "react";
import { useBoxUpdate, useNodeGestures, useNodeZIndex } from "../hooks";
import { BaseNode } from "./BaseNode";
import { NodeContent } from "./NodeContent";

export function CompoundNode({ id, boxAtom, dataAtom }: CompoundNodeState) {
  const ref = useRef<HTMLDivElement>(null);
  const store = useStore();

  useNodeZIndex(ref, id);
  useNodeGestures(ref, id);

  useBoxUpdate(store, boxAtom, ({ min, max }) => {
    if (ref.current) {
      ref.current.style.translate = `${min.x}px ${min.y}px`;
      ref.current.style.width = `${max.x - min.x}px`;
      ref.current.style.height = `${max.y - min.y}px`;
    }
  });

  return (
    <BaseNode ref={ref} id={id} kind="compound">
      <NodeContent id={id} kind="compound" dataAtom={dataAtom} />
    </BaseNode>
  );
}
