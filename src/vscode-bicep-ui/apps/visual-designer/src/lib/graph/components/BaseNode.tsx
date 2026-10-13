// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { NodeKind } from "../atoms";

import { useAtomValue } from "jotai";
import { forwardRef } from "react";
import { styled } from "styled-components";
import { fadedNodeIdsAtom } from "../atoms";

export type BaseNodeProps = PropsWithChildren<{ id: string; kind: NodeKind; zIndex: number }>;

const $BaseNode = styled.div<{ $zIndex: number; $isFaded: boolean }>`
  display: flex;
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  transform-origin: 0 0;
  z-index: ${({ $zIndex }) => $zIndex};
  opacity: ${({ $isFaded }) => ($isFaded ? 0 : 1)};
  pointer-events: ${({ $isFaded }) => ($isFaded ? "none" : "auto")};
  transition: opacity 100ms ease;
`;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(({ id, kind, zIndex, children }, ref) => {
  const isFaded = useAtomValue(fadedNodeIdsAtom).has(id);

  return (
    <$BaseNode
      ref={ref}
      $zIndex={zIndex}
      $isFaded={isFaded}
      data-testid="graph-node"
      data-node-id={id}
      data-node-kind={kind}
    >
      {children}
    </$BaseNode>
  );
});

BaseNode.displayName = "BaseNode";
