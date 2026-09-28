// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";

/**
 * Space between the panel's outer edge and its content, including the 1px edge line drawn inside it.
 * A whole number of CSS pixels, so the content lands on whole device pixels at any display scale.
 */
const PADDING = 4;
/** Space between the buttons in a panel, so neighboring hover backgrounds never touch. */
const GAP = 2;
/**
 * Only slightly rounder than the 5px buttons inside. Fully concentric corners (button radius plus
 * the 4px inset, 9px) look too round on a panel this narrow, and at 1x the button's hover background
 * then seems to crowd the curved corner and sit off-center.
 */
const RADIUS = 7;

/**
 * A panel that floats above the viewport: the chrome shared by the control bar and creation dock.
 *
 * The 1px edge line is an inset outline rather than a border. Browsers draw borders in whole device
 * pixels, so at 1.5x scaling a 1px border takes up 0.667 CSS px. That puts the content half a device
 * pixel off, and its hover background then snaps a pixel closer to one side. An outline takes no
 * layout space, so it cannot move the content.
 */
export const FloatingPanel = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${GAP}px;
  padding: ${PADDING}px;
  background-color: ${({ theme }) => theme.panel.background};
  outline: 1px solid ${({ theme }) => theme.panel.border};
  outline-offset: -1px;
  border-radius: ${RADIUS}px;
  box-shadow:
    0 1px 3px rgba(0, 0, 0, 0.08),
    0 4px 12px rgba(0, 0, 0, 0.06);
  backdrop-filter: blur(12px);
`;

/**
 * A line between groups of buttons in a vertical `FloatingPanel`.
 *
 * The flex gap already sits on each side of it, so the vertical margin adds only the rest: each group
 * is then as far from the line as the outer buttons are from the panel's edge.
 */
export const FloatingPanelDivider = styled.div.attrs({ role: "separator" })`
  height: 1px;
  margin: ${PADDING - GAP}px 3px;
  background-color: ${({ theme }) => theme.panel.border};
`;
