// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useTheme } from "styled-components";

export function EdgeMarkerDefs() {
  const theme = useTheme();

  return (
    <defs>
      <marker
        id="line-arrow"
        markerWidth="6"
        markerHeight="10"
        refX="5"
        refY="5"
        viewBox="0 0 6 10"
        markerUnits="strokeWidth"
        orient="auto"
      >
        <polyline
          points="1.5,1 5,5 1.5,9"
          fill="none"
          strokeWidth="1"
          stroke={theme.edge.color}
          strokeLinecap="round"
          strokeLinejoin="round"
        />
      </marker>
    </defs>
  );
}
