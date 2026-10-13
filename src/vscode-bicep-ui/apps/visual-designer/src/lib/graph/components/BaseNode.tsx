// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { NodeKind } from "../atoms";

import { atom, useAtomValue } from "jotai";
import { forwardRef, useMemo } from "react";
import { styled } from "styled-components";
import { fadedNodeIdsAtom } from "../atoms";

/** The node's z-index is written by `useNodeZIndex`, so focus changes do not re-render nodes. */
export type BaseNodeProps = PropsWithChildren<{ id: string; kind: NodeKind }>;

const $BaseNode = styled.div<{ $isFaded: boolean }>`
  display: flex;
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  transform-origin: 0 0;
  opacity: ${({ $isFaded }) => ($isFaded ? 0 : 1)};
  pointer-events: ${({ $isFaded }) => ($isFaded ? "none" : "auto")};
  transition: opacity 100ms ease;
`;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(({ id, kind, children }, ref) => {
  // Derived, so fading some nodes re-renders only those nodes.
  const isFadedAtom = useMemo(() => atom((get) => get(fadedNodeIdsAtom).has(id)), [id]);
  const isFaded = useAtomValue(isFadedAtom);

  return (
    <$BaseNode ref={ref} $isFaded={isFaded} data-testid="graph-node" data-node-id={id} data-node-kind={kind}>
      {children}
    </$BaseNode>
  );
});

BaseNode.displayName = "BaseNode";
