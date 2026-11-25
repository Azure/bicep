// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeToggle } from "../../hooks/use-node-toggle";

import { Codicon } from "@vscode-bicep-ui/components";
import { keyframes, styled } from "styled-components";
import { ICON_BUTTON_ICON_SIZE } from "@/ui";

const spin = keyframes`
  to {
    transform: rotate(360deg);
  }
`;

/** Replaces the toggle's icon while the module is restored, so the user sees that expanding it is under way. */
const $ProgressRing = styled.div`
  display: flex;
  animation: ${spin} 1s linear infinite;
`;

/**
 * Codicons have no diagonal expand and collapse arrows, so these are drawn in their style: a 16px grid, a 1px stroke
 * in the current color. Expand points a pair of arrows out to opposite corners; collapse points them in toward the
 * center, as the node grows or shrinks.
 */
const EXPAND_PATHS = ["M9.5 2.5h4v4", "M13.5 2.5L9 7", "M6.5 13.5h-4v-4", "M2.5 13.5L7 9"];
const COLLAPSE_PATHS = ["M9 3v4h4", "M9 7l4.5-4.5", "M7 13V9H3", "M7 9l-4.5 4.5"];

const $ArrowsIcon = styled.svg`
  display: block;
  flex: 0 0 auto;
  fill: none;
  stroke: currentColor;
  stroke-width: 1;
  stroke-linecap: round;
  stroke-linejoin: round;
`;

function ArrowsIcon({ action }: { action: NodeToggle["action"] }) {
  const paths = action === "Expand" ? EXPAND_PATHS : COLLAPSE_PATHS;

  return (
    <$ArrowsIcon
      width={ICON_BUTTON_ICON_SIZE}
      height={ICON_BUTTON_ICON_SIZE}
      viewBox="0 0 16 16"
      aria-hidden="true"
      data-testid={action === "Expand" ? "expand-node-icon" : "collapse-node-icon"}
    >
      {paths.map((d) => (
        <path key={d} d={d} />
      ))}
    </$ArrowsIcon>
  );
}

export type NodeToggleIconProps = {
  toggle: NodeToggle;
  isRestoring: boolean;
  /** Identifies the progress ring in tests. */
  progressTestId?: string;
};

/** A container toggle's icon: arrows out to expand, arrows in to collapse, or a progress ring while a module is restored. */
export function NodeToggleIcon({ toggle, isRestoring, progressTestId }: NodeToggleIconProps) {
  if (isRestoring) {
    return (
      <$ProgressRing data-testid={progressTestId}>
        <Codicon name="loading" size={ICON_BUTTON_ICON_SIZE} />
      </$ProgressRing>
    );
  }

  return <ArrowsIcon action={toggle.action} />;
}
