// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { useCallback } from "react";
import { styled } from "styled-components";
import { FLOATING_PANEL_THICKNESS, FloatingPanel } from "@/ui";
import { useStatusApi } from "../api";
import { graphStatusAtom } from "../atoms";

const STATUS_ICON_SIZE = 14;

/** Occupies the `status` area of the app's bottom chrome grid, which aligns its bottom edge with the dock's. */
const $StatusBarContainer = styled.div`
  grid-area: status;
  justify-self: start;
  display: flex;
  min-width: 0;
  max-width: 100%;
`;

/** As tall as the control bar is wide, so the status reads as the same family of chrome. */
const $StatusChip = styled(FloatingPanel)`
  flex-direction: row;
  align-items: center;
  gap: 6px;
  min-width: 0;
  height: ${FLOATING_PANEL_THICKNESS}px;
  padding: 0 10px 0 9px;
  color: ${({ theme }) => theme.text.secondary};
  font-size: 12px;
  font-weight: 500;
  white-space: nowrap;
  cursor: default;
  pointer-events: auto;
`;

const $StatusIcon = styled.span<{ $tone: "error" | "info" }>`
  display: flex;
  flex-shrink: 0;
  color: ${({ $tone, theme }) => ($tone === "error" ? theme.error : theme.iconButton.color)};
`;

/** Lets a long message truncate rather than run under the dock. */
const $StatusDetail = styled.span`
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const $ErrorLink = styled.button`
  flex-shrink: 0;
  padding: 0;
  border: none;
  border-radius: 3px;
  background: none;
  color: ${({ theme }) => theme.error};
  font: inherit;
  font-weight: 600;
  cursor: pointer;
  text-decoration: underline;
  text-decoration-color: transparent;
  text-underline-offset: 2px;
  transition: text-decoration-color 150ms ease;

  &:hover {
    text-decoration-color: currentColor;
  }

  &:focus-visible {
    outline: 2px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 2px;
  }
`;

/** Speaks up only when the graph needs attention; a healthy graph shows nothing. */
export function StatusBar() {
  const graphStatus = useAtomValue(graphStatusAtom);
  const api = useStatusApi();

  const handleShowProblems = useCallback(() => {
    api.showProblems();
  }, [api]);

  const errorCount = graphStatus.kind === "errors" ? graphStatus.errorCount : 0;
  const errorLabel = `${errorCount} ${errorCount === 1 ? "error" : "errors"}`;

  return (
    <$StatusBarContainer data-testid="status-bar" data-status={graphStatus.kind} data-error-count={errorCount}>
      {graphStatus.kind === "errors" && (
        <$StatusChip>
          <$StatusIcon $tone="error" data-testid="status-indicator">
            <Codicon name="error" size={STATUS_ICON_SIZE} />
          </$StatusIcon>
          <$ErrorLink
            type="button"
            title="Show Problems"
            aria-label={`${errorLabel}. Show Problems`}
            onClick={handleShowProblems}
            data-testid="status-error-link"
          >
            {errorLabel}
          </$ErrorLink>
        </$StatusChip>
      )}
      {graphStatus.kind === "empty" && (
        <$StatusChip data-testid="status-empty-message" title="No resources or modules to display">
          <$StatusIcon $tone="info" data-testid="status-indicator">
            <Codicon name="info" size={STATUS_ICON_SIZE} />
          </$StatusIcon>
          <$StatusDetail>No resources or modules to display</$StatusDetail>
        </$StatusChip>
      )}
    </$StatusBarContainer>
  );
}
