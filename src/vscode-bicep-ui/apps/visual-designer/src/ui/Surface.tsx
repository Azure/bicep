// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";

/**
 * A floating panel: the visual container for controls and islands layered over the canvas.
 */
export const Surface = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1px;
  padding: 4px;
  background-color: ${({ theme }) => theme.controlBar.background};
  border: 1px solid ${({ theme }) => theme.controlBar.border};
  border-radius: 8px;
  box-shadow:
    0 1px 3px rgba(0, 0, 0, 0.08),
    0 4px 12px rgba(0, 0, 0, 0.06);
  backdrop-filter: blur(12px);
`;
