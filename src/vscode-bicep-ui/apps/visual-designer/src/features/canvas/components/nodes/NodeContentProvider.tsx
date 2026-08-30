// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ReactNode } from "react";
import type { NodeContentRenderProps, NodeKind } from "@/lib/graph";
import type { ModuleNodeProps } from "./ModuleNode";
import type { ResourceNodeProps } from "./ResourceNode";

import { useStore } from "jotai";
import { useHydrateAtoms } from "jotai/utils";
import { useEffect, useRef } from "react";
import { styled } from "styled-components";
import { nodeConfigAtom } from "@/lib/graph";
import { useCanvasApi } from "../../api";
import { ModuleNode } from "./ModuleNode";
import { ResourceNode } from "./ResourceNode";

/** Extra headroom above a compound node's children, so the module label has room to sit. */
const COMPOUND_NODE_LABEL_INSET = 50;

const $NodeContent = styled.div`
  display: contents;
`;

function CanvasNodeContent({ kind, id, data }: NodeContentRenderProps & { kind: NodeKind }) {
  const ref = useRef<HTMLDivElement>(null);
  const api = useCanvasApi();

  useEffect(() => {
    const element = ref.current;

    if (!element) {
      return;
    }

    const revealSource = (event: MouseEvent) => {
      event.stopPropagation();
      api.revealNodeSource(id);
    };

    element.addEventListener("dblclick", revealSource);
    return () => element.removeEventListener("dblclick", revealSource);
  }, [api, id]);

  return (
    <$NodeContent ref={ref}>
      {kind === "compound" ? (
        <ModuleNode id={id} data={data as ModuleNodeProps["data"]} />
      ) : (
        <ResourceNode id={id} data={data as ResourceNodeProps["data"]} />
      )}
    </$NodeContent>
  );
}

function renderNodeContent(kind: NodeKind, { id, data }: NodeContentRenderProps) {
  return <CanvasNodeContent kind={kind} id={id} data={data} />;
}

/**
 * Teaches the generic graph engine how to render Bicep node content.
 *
 * `lib/graph` is Bicep-agnostic and reaches product code only through `nodeConfigAtom`. Hydrating
 * during render rather than in an effect guarantees the config is in place before any node mounts,
 * and scoping the write to the store from context keeps it out of module scope so tests can supply
 * their own store.
 */
export function NodeContentProvider({ children }: { children: ReactNode }) {
  const store = useStore();
  const defaults = store.get(nodeConfigAtom);

  useHydrateAtoms([
    [
      nodeConfigAtom,
      {
        ...defaults,
        padding: { ...defaults.padding, top: COMPOUND_NODE_LABEL_INSET },
        renderContent: renderNodeContent,
      },
    ],
  ] as const);

  return <>{children}</>;
}
