// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { HTMLAttributes, PropsWithChildren, ReactElement } from "react";

import { useCallback, useRef } from "react";
import styled from "styled-components";
import { usePanZoomTransformListener } from "./usePanZoomTransformListener";

type PanZoomTransformedProps = PropsWithChildren<HTMLAttributes<HTMLDivElement>>;

const $PanZoomTransformed = styled.div`
  transform-origin: 0 0;
  margin: 0;
  height: max-content;
  width: max-content;
`;

/**
 * Applies pan and zoom transformations to the provided component.
 *
 * @param {PanZoomTransformedProps} props - The component props.
 * @param {string} props.className - The class name for the component.
 * @param {ReactNode} props.children - The child elements of the component.
 * @returns {ReactElement} - The transformed component.
 */
export function PanZoomTransformed({ children, ...attributes }: PanZoomTransformedProps): ReactElement {
  const ref = useRef<HTMLDivElement>(null);

  usePanZoomTransformListener(
    useCallback((x, y, k) => {
      if (ref.current) {
        ref.current.style.transform = `translate(${x}px, ${y}px) scale(${k})`;
      }
    }, []),
  );

  return (
    <$PanZoomTransformed {...attributes} ref={ref}>
      {children}
    </$PanZoomTransformed>
  );
}
