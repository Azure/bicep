// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";

import { Codicon } from "@vscode-bicep-ui/components";
import styled from "styled-components";

const ErrorAlertDiv = styled.div`
  color: var(--vscode-statusBarItem-errorForeground);
  background-color: var(--vscode-statusBarItem-errorBackground);
  padding: 5px 10px;
  border-radius: 4px;
  font-size: 14px;
  align-self: center;
`;

export const ErrorAlert: FC<{ message: string }> = ({ message }) => {
  return (
    <ErrorAlertDiv>
      <Codicon name="error" size={14} />
      {message}
    </ErrorAlertDiv>
  );
};
