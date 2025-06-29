// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { usePanZoomTransformListener } from "@vscode-bicep-ui/components";
import { useRef } from "react";
import { styled } from "styled-components";

const $Svg = styled.svg`
  overflow: visible;
  position: absolute;
  pointer-events: none;
`;

const defaultSpacing = 16;
const defaultDotRadius = 1;

export function CanvasBackground() {
  const patternRef = useRef<SVGPatternElement | null>(null);
  const circleRef = useRef<SVGCircleElement | null>(null);

  usePanZoomTransformListener((x, y, scale) => {
    if (!patternRef.current || !circleRef.current) {
      return;
    }

    const spacing = defaultSpacing * scale;

    const patternX = (x % spacing).toString();
    const patternY = (y % spacing).toString();
    patternRef.current.setAttribute("x", patternX);
    patternRef.current.setAttribute("y", patternY);

    const width = spacing.toString();
    const height = width;
    patternRef.current.setAttribute("width", width);
    patternRef.current.setAttribute("height", height);

    const offsetX = (-spacing / 2).toString();
    const offsetY = offsetX;
    const patternTransform = `translate(${offsetX},${offsetY})`;
    patternRef.current.setAttribute("patternTransform", patternTransform);

    const dotRadius = (defaultDotRadius * scale).toString();
    circleRef.current.setAttribute("cx", dotRadius);
    circleRef.current.setAttribute("cy", dotRadius);
    circleRef.current.setAttribute("r", dotRadius);
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
          <circle ref={circleRef} cx="1" cy="1" r="1" fill="#e6e6e6" />
        </pattern>
      </defs>
      <rect x="0" y="0" width="100%" height="100%" fill="url(#figma-like-dots)" />
    </$Svg>
  );
}
