// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useAtomValue } from "jotai";
import { useCallback } from "react";
import { styled } from "styled-components";
import { errorCountAtom, hasNodesAtom } from "../features/graph-engine";
import { SHOW_PROBLEMS_PANEL_NOTIFICATION } from "../messages";

const $StatusBarContainer = styled.div`
  position: absolute;
  height: 32px;
  left: 20px;
  bottom: 20px;
  display: flex;
  flex-direction: row;
  align-items: center;
  z-index: 100;
  font-size: 13px;
  color: ${({ theme }) => theme.text.primary};
  user-select: none;
  cursor: default;
`;

const $StatusCircle = styled.div<{ $hasErrors: boolean }>`
  width: 8px;
  height: 8px;
  background-color: ${({ $hasErrors, theme }) => ($hasErrors ? theme.error : theme.success)};
  border-radius: 50%;
  margin-right: 8px;
`;

const $ErrorLink = styled.span`
  cursor: pointer;
  color: ${({ theme }) => theme.error};
  text-decoration: underline;
  text-decoration-color: transparent;
  transition: text-decoration-color 0.15s ease;

  &:hover {
    text-decoration-color: ${({ theme }) => theme.error};
  }
`;

export function StatusBar() {
  const errorCount = useAtomValue(errorCountAtom);
  const hasNodes = useAtomValue(hasNodesAtom);
  const messageChannel = useWebviewMessageChannel();

  const handleShowProblems = useCallback(() => {
    messageChannel.sendNotification({
      method: SHOW_PROBLEMS_PANEL_NOTIFICATION,
    });
  }, [messageChannel]);

  return (
    <$StatusBarContainer>
      <$StatusCircle $hasErrors={errorCount > 0} />
      {errorCount > 0 && (
        <span>
          There {errorCount === 1 ? "is" : "are"}{" "}
          <$ErrorLink onClick={handleShowProblems}>
            {errorCount} {errorCount === 1 ? "error" : "errors"}
          </$ErrorLink>{" "}
          in the file. The rendered graph may not be accurate.
        </span>
      )}
      {errorCount === 0 && !hasNodes && (
        <span>There are no resources or modules in the file. Nothing to display.</span>
      )}
    </$StatusBarContainer>
  );
}
