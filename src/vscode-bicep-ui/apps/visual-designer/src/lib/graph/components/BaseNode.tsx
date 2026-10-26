// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";
import type { NodeKind } from "../atoms";

import { motion } from "motion/react";
import { forwardRef } from "react";
import { styled } from "styled-components";
import { NODE_ENTER_TRANSITION, NODE_EXIT_TRANSITION } from "../motion";

/** Position, size, and z-index are written to the element directly, so moving a node does not re-render it. */
export type BaseNodeProps = PropsWithChildren<{
  id: string;
  kind: NodeKind;
  /** Fades the node in when it mounts. */
  isEntering: boolean;
  /** Fades the node out. It no longer responds to the pointer. */
  isExiting: boolean;
}>;

const $BaseNode = styled(motion.div)<{ $isExiting: boolean }>`
  display: flex;
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  transform-origin: 0 0;
  /* Shown once the node has a box to be drawn at. */
  visibility: hidden;
  pointer-events: ${({ $isExiting }) => ($isExiting ? "none" : "auto")};
`;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(
  ({ id, kind, isEntering, isExiting, children }, ref) => (
    <$BaseNode
      ref={ref}
      $isExiting={isExiting}
      initial={isEntering ? { opacity: 0 } : false}
      animate={{ opacity: isExiting ? 0 : 1 }}
      transition={isExiting ? NODE_EXIT_TRANSITION : NODE_ENTER_TRANSITION}
      data-testid="graph-node"
      data-node-id={id}
      data-node-kind={kind}
    >
      {children}
    </$BaseNode>
  ),
);

BaseNode.displayName = "BaseNode";
