// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { NodeContentRenderProps, NodeKind } from "@/lib/graph";
import type { Range } from "../../api";
import type { ModuleNodeProps } from "./ModuleNode";
import type { ResourceNodeProps } from "./ResourceNode";

import { useStore } from "jotai";
import { useHydrateAtoms } from "jotai/utils";
import { useCallback } from "react";
import { nodeConfigAtom } from "@/lib/graph";
import { useDeploymentGraphApi } from "../../api";
import { ModuleNode } from "./ModuleNode";
import { ResourceNode } from "./ResourceNode";

/** Extra headroom above a compound node's children, so the module label has room to sit. */
const COMPOUND_NODE_LABEL_INSET = 50;

/** The source-location fields a node may carry. Absent on the server-driven path. */
type NodeSourceLocation = { range?: Range; filePath?: string };

function renderNodeContent(kind: NodeKind, { id, data }: NodeContentRenderProps) {
  if (kind === "compound") {
    return <ModuleNode id={id} data={data as ModuleNodeProps["data"]} />;
  }

  return <ResourceNode id={id} data={data as ResourceNodeProps["data"]} />;
}

/**
 * Teaches the generic graph engine how to render Bicep node content and what activating a node means.
 *
 * `lib/graph` is Bicep-agnostic and reaches product code only through `nodeConfigAtom`. Hydrating
 * during render rather than in an effect guarantees the config is in place before any node mounts,
 * and scoping the write to the store from context keeps it out of module scope so tests can supply
 * their own store.
 */
export function NodeContentProvider({ children }: { children: ReactNode }) {
  const store = useStore();
  const defaults = store.get(nodeConfigAtom);
  const api = useDeploymentGraphApi();

  const handleNodeActivate = useCallback(
    (id: string, data: unknown) => {
      const { range, filePath } = (data ?? {}) as NodeSourceLocation;

      if (range && filePath) {
        // Legacy push path: the node still carries an inline source location.
        api.revealFileRange({ filePath, range });
        return;
      }

      // Server-driven path: source location is resolved on demand by node id.
      api.revealNodeSource(id);
    },
    [api],
  );

  useHydrateAtoms([
    [
      nodeConfigAtom,
      {
        ...defaults,
        padding: { ...defaults.padding, top: COMPOUND_NODE_LABEL_INSET },
        renderContent: renderNodeContent,
        onNodeActivate: handleNodeActivate,
      },
    ],
  ] as const);

  return <>{children}</>;
}
