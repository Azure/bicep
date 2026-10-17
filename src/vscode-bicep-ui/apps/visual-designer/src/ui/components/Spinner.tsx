// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { keyframes, styled } from "styled-components";

const spin = keyframes`
  to {
    transform: rotate(360deg);
  }
`;

const $Spinner = styled.span<{ $isAnimated: boolean }>`
  display: flex;
  animation: ${({ $isAnimated }) => ($isAnimated ? spin : "none")} 1s linear infinite;
`;

export type SpinnerProps = {
  size: number;
  /** False under reduced motion, which leaves the icon still. */
  isAnimated: boolean;
};

/** VS Code's `loading` codicon, spinning: work is in progress. */
export function Spinner({ size, isAnimated }: SpinnerProps) {
  return (
    <$Spinner $isAnimated={isAnimated} data-testid="spinner">
      <Codicon name="loading" size={size} />
    </$Spinner>
  );
}
