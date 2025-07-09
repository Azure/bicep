// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { usePanZoomTransformListener } from "@vscode-bicep-ui/components";
import { useRef } from "react";
import { styled } from "styled-components";

const $Svg = styled.svg`
  overflow: visible;
  background-color: #f5f5f5;
  position: absolute;
  pointer-events: none;
`;

const defaultSpacing = 16;
const defaultDotRadius = 1;
const minScale = 0.95;

function getEffectiveScale(actualScale: number): number {
  // This creates the looping effect where dots get dense, then suddenly sparse again
  if (actualScale <= minScale) {
    const scaleFactor = Math.floor(Math.log2(minScale / actualScale));
    return actualScale * Math.pow(2, scaleFactor);
  }

  return actualScale;
}

export function CanvasBackground() {
  const patternRef = useRef<SVGPatternElement | null>(null);
  const circleRef = useRef<SVGCircleElement | null>(null);

  usePanZoomTransformListener((x, y, scale) => {
    if (!patternRef.current || !circleRef.current) {
      return;
    }

    const effectiveScale = getEffectiveScale(scale);
    const spacing = defaultSpacing * effectiveScale;

    const patternX = (x % spacing).toString();
    const patternY = (y % spacing).toString();
    patternRef.current.setAttribute("x", patternX);
    patternRef.current.setAttribute("y", patternY);

    const width = spacing.toString();
    const height = width;
    patternRef.current.setAttribute("width", width);
    patternRef.current.setAttribute("height", height);

    const offset = (-spacing / 2).toString();
    const patternTransform = `translate(${offset},${offset})`;
    patternRef.current.setAttribute("patternTransform", patternTransform);

    const dotRadius = defaultDotRadius * effectiveScale;
    const clampedDotRadius = Math.max(dotRadius, 0.5).toString();
    circleRef.current.setAttribute("cx", clampedDotRadius);
    circleRef.current.setAttribute("cy", clampedDotRadius);
    circleRef.current.setAttribute("r", clampedDotRadius);
  });

  return (
    <$Svg width="100%" height="100%">
      <defs>
        <pattern
          ref={patternRef}
          id="figma-like-dots"
          x={0}
          y={0}
          width={defaultSpacing}
          height={defaultSpacing}
          patternUnits="userSpaceOnUse"
          patternTransform={`translate(-${defaultSpacing / 2},-${defaultSpacing / 2})`}
        >
          <circle ref={circleRef} cx="1" cy="1" r="1" fill="#c4c4c4" />
        </pattern>
      </defs>
      <rect x="0" y="0" width="100%" height="100%" fill="url(#figma-like-dots)" />
    </$Svg>
  );
}
