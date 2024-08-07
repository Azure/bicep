// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ErrorResponse } from "@azure/arm-resources";
import {
  VSCodeCheckbox,
  VSCodeDataGrid,
  VSCodeDataGridCell,
  VSCodeDataGridRow,
} from "@vscode/webview-ui-toolkit/react";
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
        <VSCodeCheckbox onChange={() => setShowJson(!showJson)} checked={showJson}>
          Show JSON?
        </VSCodeCheckbox>
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
    <VSCodeDataGrid gridTemplateColumns="max-content auto">
      {error.code && (
        <VSCodeDataGridRow key={0}>
          <VSCodeDataGridCell gridColumn="1">Code</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2">{error.code}</VSCodeDataGridCell>
        </VSCodeDataGridRow>
      )}
      {error.message && (
        <VSCodeDataGridRow key={1}>
          <VSCodeDataGridCell gridColumn="1">Message</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2">{error.message}</VSCodeDataGridCell>
        </VSCodeDataGridRow>
      )}
      {error.target && (
        <VSCodeDataGridRow key={2}>
          <VSCodeDataGridCell gridColumn="1">Target</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2">{error.target}</VSCodeDataGridCell>
        </VSCodeDataGridRow>
      )}
      {error.details && (
        <VSCodeDataGridRow key={3}>
          <VSCodeDataGridCell gridColumn="1">Details</VSCodeDataGridCell>
          <VSCodeDataGridCell gridColumn="2">{error.details.map((x) => getError(x, showJson))}</VSCodeDataGridCell>
        </VSCodeDataGridRow>
      )}
    </VSCodeDataGrid>
  );
}
