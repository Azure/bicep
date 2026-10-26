// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { styled } from "styled-components";
import { ControlBar } from "./ControlBar";
import { HistoryBar } from "./HistoryBar";

const $ControlColumn = styled.div`
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 100;
  display: flex;
  flex-direction: column;
  gap: 12px;
`;

/** The top-right column of control panels: view and export controls, then Undo and Redo. */
export function Controls() {
  return (
    <$ControlColumn>
      <ControlBar />
      <HistoryBar />
    </$ControlColumn>
  );
}
