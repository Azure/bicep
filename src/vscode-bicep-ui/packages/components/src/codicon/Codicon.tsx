// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";

import "@vscode/codicons/dist/codicon.css";

interface CodiconProps {
  /**
   * Name of the icon to render. To find available names, see https://microsoft.github.io/vscode-codicons/dist/codicon.html.
   */
  name: string;
  /**
   * Size of the icon in pixels.
   */
  size: number;
}

const $Codicon = styled.div.attrs<{ $name: string; $size: number }>((props) => ({
  className: `codicon codicon-${props.$name}`,
}))`
  &&& {
    width: ${(props) => props.$size}px;
    height: ${(props) => props.$size}px;
    font-size: ${(props) => props.$size}px;
  }
`;

/**
 * UI component for rendering a VS Code icon.
 */
export function Codicon({ name, size }: CodiconProps) {
  return <$Codicon $name={name} $size={size} aria-hidden={true} data-testid={`${name}-codicon`} />;
}
