import { VSCodeDivider, VSCodeDataGrid, VSCodeDataGridRow, VSCodeDataGridCell } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { DeployResult } from "../models";
import { ErrorResponse } from "@azure/arm-resources";

interface ResultsViewProps {
  result?: DeployResult;
}

export const ResultsView: FC<ResultsViewProps> = ({ result, }) => {
  if (!result) {
    return null;
  }

  return (
    <section>
      <VSCodeDivider />
      <h2>Result</h2>
      <p>{result.success ? 'Succeeded' : 'Failed'}</p>
      {result.error ? getError(result.error) : null}
    </section>
  );
};

function getError(error: ErrorResponse) {
  return (
    <VSCodeDataGrid gridTemplateColumns="max-content auto">
      {error.code ? <VSCodeDataGridRow key={0}>
        <VSCodeDataGridCell gridColumn="1">Code</VSCodeDataGridCell>
        <VSCodeDataGridCell gridColumn="2">{error.code}</VSCodeDataGridCell>
      </VSCodeDataGridRow> : null}
      {error.message ? <VSCodeDataGridRow key={1}>
        <VSCodeDataGridCell gridColumn="1">Message</VSCodeDataGridCell>
        <VSCodeDataGridCell gridColumn="2">{error.message}</VSCodeDataGridCell>
      </VSCodeDataGridRow> : null}
      {error.target ? <VSCodeDataGridRow key={2}>
        <VSCodeDataGridCell gridColumn="1">Target</VSCodeDataGridCell>
        <VSCodeDataGridCell gridColumn="2">{error.target}</VSCodeDataGridCell>
      </VSCodeDataGridRow> : null}
    </VSCodeDataGrid>
  );
}