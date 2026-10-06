// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Codicon } from "@vscode-bicep-ui/components";
import styled from "styled-components";
import { MotionAwareProgressBar } from "./MotionAwareProgressBar";

const $Controls = styled.div`
  position: relative;
  flex-shrink: 0;
  padding: 8px;
  border-bottom: 1px solid ${({ theme }) => theme.panel.border};
`;

const $Search = styled.label`
  display: flex;
  height: 28px;
  align-items: center;
  gap: 6px;
  padding: 0 8px;
  border: 1px solid ${({ theme }) => theme.panel.border};
  border-radius: 6px;
  color: ${({ theme }) => theme.text.secondary};
  background: ${({ theme }) => theme.viewport.background};
  transition: border-color 150ms ease;

  &:focus-within {
    border-color: ${({ theme }) => theme.focusBorder};
  }
`;

const $SearchInput = styled.input`
  min-width: 0;
  flex: 1;
  padding: 0;
  border: 0;
  outline: 0;
  color: ${({ theme }) => theme.text.primary};
  background: transparent;
  font: inherit;

  &:focus {
    outline: none;
    outline-offset: 0;
  }

  &::placeholder {
    color: ${({ theme }) => theme.text.secondary};
  }
`;

const $ProgressTrack = styled.div`
  position: absolute;
  right: 0;
  bottom: -1px;
  left: 0;
  height: 2px;
  overflow: hidden;
`;

export function PaletteControls({
  query,
  setQuery,
  showProgress,
}: {
  query: string;
  setQuery: (query: string) => void;
  showProgress: boolean;
}) {
  return (
    <$Controls>
      <$Search>
        <Codicon name="search" size={14} />
        <$SearchInput
          aria-label="Filter resource types"
          title="Filter resource types"
          autoFocus
          placeholder="Filter resource types"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </$Search>
      <$ProgressTrack aria-hidden={!showProgress}>
        {showProgress && (
          <MotionAwareProgressBar testId="resource-palette-progress" ariaLabel="Loading resource types" />
        )}
      </$ProgressTrack>
    </$Controls>
  );
}
