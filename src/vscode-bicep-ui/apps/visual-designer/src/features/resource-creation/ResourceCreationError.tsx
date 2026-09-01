// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue, useSetAtom } from "jotai";
import styled from "styled-components";
import { resourceCreationErrorAtom } from "./atoms";

const $ResourceCreationError = styled.div`
  position: absolute;
  bottom: 32px;
  left: 50%;
  z-index: 120;
  display: flex;
  max-width: min(520px, calc(100% - 32px));
  align-items: center;
  gap: 12px;
  padding: 8px 10px;
  transform: translateX(-50%);
  border: 1px solid var(--vscode-inputValidation-errorBorder);
  border-radius: 3px;
  color: var(--vscode-notifications-foreground);
  background: var(--vscode-notifications-background);
  box-shadow: 0 2px 8px var(--vscode-widget-shadow);
`;

const $Message = styled.span`
  overflow: hidden;
  text-overflow: ellipsis;
`;

const $Dismiss = styled.button`
  padding: 0 4px;
  border: 0;
  color: inherit;
  background: transparent;
  cursor: pointer;
`;

export function ResourceCreationError() {
  const error = useAtomValue(resourceCreationErrorAtom);
  const setError = useSetAtom(resourceCreationErrorAtom);

  if (!error) {
    return null;
  }

  return (
    <$ResourceCreationError role="alert">
      <$Message>{error}</$Message>
      <$Dismiss aria-label="Dismiss resource creation error" onClick={() => setError(null)}>
        &times;
      </$Dismiss>
    </$ResourceCreationError>
  );
}
