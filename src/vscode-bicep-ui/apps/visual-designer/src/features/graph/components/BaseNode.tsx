// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { forwardRef } from "react";
import { styled } from "styled-components";

export type BaseNodeProps = PropsWithChildren<{ zIndex: number }>;

const $BaseNode = styled.div<{ $zIndex: number }>`
  display: flex;
  position: absolute;
  box-sizing: border-box;
  cursor: default;
  transform-origin: 0 0;
  z-index: ${({ $zIndex }) => $zIndex};
`;

export const BaseNode = forwardRef<HTMLDivElement, BaseNodeProps>(({ zIndex, children }, ref) => {
  return (
    <$BaseNode ref={ref} $zIndex={zIndex}>
      {children}
    </$BaseNode>
  );
});
