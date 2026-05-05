// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { forwardRef } from "react";
import { styled } from "styled-components";

export type BaseNodeProps = PropsWithChildren<{ id: string; zIndex: number }>;

const $BaseNode = styled.div<{ $zIndex: number }>`
  display: flex;
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  transform-origin: 0 0;
  z-index: ${({ $zIndex }) => $zIndex};
`;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(({ id, zIndex, children }, ref) => {
  return (
    <$BaseNode ref={ref} $zIndex={zIndex} data-node-id={id}>
      {children}
    </$BaseNode>
  );
});

BaseNode.displayName = "BaseNode";
