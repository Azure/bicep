// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";
import {
  VscodeCheckbox,
  VscodeTable,
  VscodeTableBody,
  VscodeTableCell,
  VscodeTableRow
} from "@vscode-elements/react-elements";
import { FC, useState } from "react";
import { DeployResult } from "../../../models";
import { getPreformattedJson } from "../utils";
import { FormSection } from "./FormSection";

interface ResultsViewProps {
  result?: DeployResult;
}

export const ResultsView: FC<ResultsViewProps> = ({ result }) => {
  if (!result) {
    return null;
  }

  const [showJson, setShowJson] = useState(false);

  return (
    <FormSection title="Result">
      <p>{result.success ? "Succeeded" : "Failed"}</p>
      {result.error && (
        <VscodeCheckbox onChange={() => setShowJson(!showJson)} checked={showJson}>
          Show JSON?
        </VscodeCheckbox>
      )}
      {result.error && getError(result.error, showJson)}
    </FormSection>
  );
};

function getError(error: ErrorResponse, showJson: boolean) {
  if (showJson) {
    return getPreformattedJson(error);
  }

  return (
    <VscodeTable>
      <VscodeTableBody slot="body">
      {error.code && (
        <VscodeTableRow key={0}>
          <VscodeTableCell id="1">Code</VscodeTableCell>
          <VscodeTableCell id="2">{error.code}</VscodeTableCell>
        </VscodeTableRow>
      )}
      {error.message && (
        <VscodeTableRow key={1}>
          <VscodeTableCell id="1">Message</VscodeTableCell>
          <VscodeTableCell id="2">{error.message}</VscodeTableCell>
        </VscodeTableRow>
      )}
      {error.target && (
        <VscodeTableRow key={2}>
          <VscodeTableCell id="1">Target</VscodeTableCell>
          <VscodeTableCell id="2">{error.target}</VscodeTableCell>
        </VscodeTableRow>
      )}
      {error.details && (
        <VscodeTableRow key={3}>
          <VscodeTableCell id="1">Details</VscodeTableCell>
          <VscodeTableCell id="2">{error.details.map((x) => getError(x, showJson))}</VscodeTableCell>
        </VscodeTableRow>
      )}
      </VscodeTableBody>
    </VscodeTable>
  );
}
