// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { FC } from "react";
import { CgMaximize, CgZoomIn, CgZoomOut } from "react-icons/cg";
import { VscWand } from "react-icons/vsc";
import styled, { css, DefaultTheme, withTheme } from "styled-components";
import { TooltipHost } from "../Tooltip";

interface CommandBarProps {
  theme: DefaultTheme;
  onZoomIn: () => void;
  onZoomOut: () => void;
  onLayout: () => void;
  onFit: () => void;
}

const CommandBarContainer = styled.div`
  position: absolute;
  top: 20px;
  right: 20px;
  z-index: 99999;
  filter: drop-shadow(0px 0px 4px rgba(0, 0, 0, 0.25));
`;

const CommandBarButton = styled.div`
  display: flex;
  padding: 6px;
  align-items: center;
  justify-content: center;
  background-color: ${({ theme }) => theme.commandBarButton.backgroundColor};
  ${({ theme }) => {
    switch (theme.name) {
      case "dark":
        return css`
          &:hover {
            filter: brightness(125%);
          }
          &:active {
            filter: brightness(85%);
          }
        `;
      case "light":
        return css`
          &:hover {
            filter: brightness(90%);
          }
          &:active {
            filter: brightness(80%);
          }
        `;
      case "high-contrast":
        return css`
          &:hover {
            outline: 1px solid;
          }
          &:active {
            filter: invert(100%);
          }
        `;
    }
  }}
`;

const CommandBarComponent: FC<CommandBarProps> = (props) => (
  <CommandBarContainer>
    <TooltipHost content="Zoom in">
      <CommandBarButton onClick={props.onZoomIn}>
        <CgZoomIn size={20} color={props.theme.common.foregroundSecondaryColor} />
      </CommandBarButton>
    </TooltipHost>
    <TooltipHost content="Zoom out">
      <CommandBarButton onClick={props.onZoomOut}>
        <CgZoomOut size={20} color={props.theme.common.foregroundSecondaryColor} />
      </CommandBarButton>
    </TooltipHost>
    <TooltipHost content="Reset layout">
      <CommandBarButton onClick={props.onLayout}>
        <VscWand size={20} color={props.theme.common.foregroundSecondaryColor} />
      </CommandBarButton>
    </TooltipHost>
    <TooltipHost content="Fit">
      <CommandBarButton onClick={props.onFit}>
        <CgMaximize size={20} color={props.theme.common.foregroundSecondaryColor} />
      </CommandBarButton>
    </TooltipHost>
  </CommandBarContainer>
);

// Workaround for https://github.com/styled-components/styled-components/issues/4082.
export const CommandBar = withTheme(CommandBarComponent) as FC<Omit<CommandBarProps, "theme">>;
