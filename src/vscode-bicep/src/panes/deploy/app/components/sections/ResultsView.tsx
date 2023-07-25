import { VSCodeDivider } from "@vscode/webview-ui-toolkit/react";
import { FC } from "react";
import { getPreformattedJson } from "../utils";
import { DeployResult } from "../models";

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
      {result.error ? getPreformattedJson(result.error) : null}
    </section>
  );
};