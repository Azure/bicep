// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";

export const ICON_BUTTON_SIZE = 26;
export const ICON_BUTTON_RADIUS = 5;
/**
 * The codicon design size. An even size in the even-sized button leaves a whole 5px on each side, so
 * the icon is centered on whole pixels rather than half-pixel offsets.
 */
export const ICON_BUTTON_ICON_SIZE = 16;

/**
 * A compact square icon button sized for toolbars and floating panels.
 */
export const IconButton = styled.button`
  display: flex;
  align-items: center;
  justify-content: center;
  width: ${ICON_BUTTON_SIZE}px;
  height: ${ICON_BUTTON_SIZE}px;
  padding: 0;
  border: none;
  border-radius: ${ICON_BUTTON_RADIUS}px;
  background-color: transparent;
  color: ${({ theme }) => theme.iconButton.color};
  /* Matches VS Code's action bar: a pointer when enabled, the default arrow when disabled. */
  cursor: pointer;
  transition:
    background-color 150ms ease,
    transform 150ms ease;

  &:hover {
    background-color: ${({ theme }) => theme.iconButton.hoverBackground};
  }

  &:active {
    background-color: ${({ theme }) => theme.iconButton.activeBackground};
    transform: scale(0.95);
  }

  &:disabled {
    opacity: 0.45;
    cursor: default;
    transform: none;
  }

  &:disabled:hover,
  &:disabled:active {
    background-color: transparent;
    transform: none;
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 1px;
  }
`;
