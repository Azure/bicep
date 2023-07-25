// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { FC } from "react";
import { VSCodeButton, VSCodeProgressRing, VSCodeTextField } from "@vscode/webview-ui-toolkit/react";
import "./index.css";
import { ParamData } from "./models";
import { useMessageHandler } from "./hooks/useMessageHandler";
import { WhatIfChangesView } from "./sections/WhatIfChangesView";
import { DeploymentOperationsView } from "./sections/DeploymentOperationsView";
import { DeploymentOutputsView } from "./sections/DeploymentOutputsView";
import { ResultsView } from "./sections/ResultsView";
import { ParametersInputView } from "./sections/ParametersInputView";
import { useAzure } from "./hooks/useAzure";

export const App: FC = () => {
  const messages = useMessageHandler();
  const azure = useAzure({
    scope: messages.scope,
    acquireAccessToken: messages.acquireAccessToken,
    templateMetadata: messages.templateMetadata,
    paramValues: messages.paramValues,
  });

  function setParamValue(key: string, data: ParamData) {
    const replaced = Object.assign({}, messages.paramValues, { [key]: data });
    messages.setParamValues(replaced);
  }

  const azureDisabled = !messages.scope || !messages.templateMetadata || azure.running;

  return (
    <main id="webview-body">
      <section className="form-section">
        <h2>Deployment Properties</h2>
        <div className="controls">
          <VSCodeButton onClick={messages.pickScope} appearance={!messages.scope ? "primary" : "secondary"}>Pick Scope</VSCodeButton>
          <VSCodeButton onClick={messages.pickParamsFile} appearance="secondary">Pick Parameters File</VSCodeButton>
        </div>
        <VSCodeTextField value={messages.scope?.subscriptionId} disabled={true}>
          Subscription Id
        </VSCodeTextField>
        <VSCodeTextField value={messages.scope?.resourceGroup} disabled={true}>
          Resource Group
        </VSCodeTextField>

        <ParametersInputView params={messages.paramValues} template={messages.templateMetadata} disabled={azure.running} onValueChange={setParamValue}/>

        <div className="controls">
          <VSCodeButton onClick={azure.deploy} disabled={azureDisabled}>Deploy</VSCodeButton>
          <VSCodeButton onClick={azure.validate} disabled={azureDisabled}>Validate</VSCodeButton>
          <VSCodeButton onClick={azure.whatIf} disabled={azureDisabled}>What-If</VSCodeButton>
        </div>
      </section>

      <section className="form-section">
        {azure.running ? <VSCodeProgressRing /> : null}
        <ResultsView result={azure.result} />
        <DeploymentOperationsView operations={azure.operations} />
        <DeploymentOutputsView outputs={azure.outputs} />
        <WhatIfChangesView changes={azure.whatIfChanges} />
      </section>
    </main>
  );
};