// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import "@vscode-elements/elements/dist/vscode-progress-bar/vscode-progress-bar.js";

import styled, { css, keyframes } from "styled-components";

export type ProgressBarMotionPolicy = "system" | "reduce" | "animate";

export interface ProgressBarProps {
  ariaLabel: string;
  motionPolicy?: ProgressBarMotionPolicy;
  testId?: string;
}

const progressAnimation = keyframes`
  from {
    transform: translateX(0%) scaleX(1);
  }
  50% {
    transform: translateX(2500%) scaleX(3);
  }
  to {
    transform: translateX(4900%) scaleX(1);
  }
`;

const $ProgressBarPolicy = styled.div<{ $policy: ProgressBarMotionPolicy }>`
  ${({ $policy }) =>
    $policy === "animate"
      ? css`
          & > vscode-progress-bar::part(indicator) {
            width: 2% !important;
            animation: ${progressAnimation} 4s linear infinite !important;
          }
        `
      : $policy === "reduce"
        ? css`
            & > vscode-progress-bar::part(indicator) {
              width: 100% !important;
              animation: none !important;
            }
          `
        : ""}
`;

export function ProgressBar({ ariaLabel, motionPolicy = "system", testId }: ProgressBarProps) {
  return (
    <$ProgressBarPolicy $policy={motionPolicy}>
      <vscode-progress-bar data-testid={testId} aria-label={ariaLabel}></vscode-progress-bar>
    </$ProgressBarPolicy>
  );
}
