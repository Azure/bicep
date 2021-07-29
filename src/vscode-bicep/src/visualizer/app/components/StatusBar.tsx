// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { memo, VFC } from "react";
import styled from "styled-components";

interface StatusBarProps {
  errorCount: number;
  hasNodes: boolean;
}

const StatusBarContainer = styled.div`
  position: absolute;
  height: 32px;
  left: 20px;
  bottom: 20px;
  display: flex;
  flex-direction: row;
  align-items: center;
`;

const StatusCircle = styled.div<{ hasErrors: boolean }>`
  width: 8px;
  height: 8px;
  background-color: ${({ hasErrors, theme }) =>
    hasErrors
      ? theme.common.errorIndicatorColor
      : theme.common.errorFreeIndicatorColor};
  border-radius: 50%;
  color: white;
  margin-top: 2px;
  margin-right: 8px;
`;

const StatusBarComponent: VFC<StatusBarProps> = ({ errorCount, hasNodes }) => (
  <StatusBarContainer>
    <StatusCircle hasErrors={errorCount > 0} />
    {errorCount > 0 && (
      <div>
        There {errorCount === 1 ? "is " : "are "}
        {errorCount}
        {errorCount === 1 ? " error" : " errors"} in the file. The rendered
        graph may not be accurate.
      </div>
    )}
    {errorCount === 0 && !hasNodes && (
      <div>
        There is no resources or modules in the file. Nothing to render.
      </div>
    )}
  </StatusBarContainer>
);

export const StatusBar = memo(StatusBarComponent);
