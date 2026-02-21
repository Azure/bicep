// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { errorCountAtom, hasNodesAtom } from "../features/graph-engine";

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
`;

const $StatusCircle = styled.div<{ $hasErrors: boolean }>`
  width: 8px;
  height: 8px;
  background-color: ${({ $hasErrors, theme }) => ($hasErrors ? theme.error : theme.success)};
  border-radius: 50%;
  margin-right: 8px;
`;

export function StatusBar() {
  const errorCount = useAtomValue(errorCountAtom);
  const hasNodes = useAtomValue(hasNodesAtom);

  return (
    <$StatusBarContainer>
      <$StatusCircle $hasErrors={errorCount > 0} />
      {errorCount > 0 && (
        <span>
          There {errorCount === 1 ? "is" : "are"} {errorCount}
          {errorCount === 1 ? " error" : " errors"} in the file. The rendered graph may not be accurate.
        </span>
      )}
      {errorCount === 0 && !hasNodes && (
        <span>There are no resources or modules in the file. Nothing to display.</span>
      )}
    </$StatusBarContainer>
  );
}
