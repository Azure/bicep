// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { FC } from "react";
import { VSCodeButton, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
import "./index.css";
import { ParamData } from "./models";
import { useMessageHandler } from "./hooks/useMessageHandler";
import { WhatIfChangesView } from "./sections/WhatIfChangesView";
import { DeploymentOperationsView } from "./sections/DeploymentOperationsView";
import { DeploymentOutputsView } from "./sections/DeploymentOutputsView";
import { ResultsView } from "./sections/ResultsView";
import { ParametersInputView } from "./sections/ParametersInputView";
import { useAzure } from "./hooks/useAzure";
import { DeploymentScopeInputView } from "./sections/DeploymentScopeInputView";

export const App: FC = () => {
  const messages = useMessageHandler();
  const azure = useAzure({
    scope: messages.scope,
    acquireAccessToken: messages.acquireAccessToken,
    templateMetadata: messages.templateMetadata,
    parametersMetadata: messages.paramsMetadata,
    showErrorDialog: messages.showErrorDialog
  });

  function setParamValue(key: string, data: ParamData) {
    const parameters = Object.assign({}, messages.paramsMetadata.parameters, { [key]: data });
    messages.setParamsMetadata({ ...messages.paramsMetadata, parameters });
  }

  function handleEnableParamEditing() {
    messages.setParamsMetadata({ ...messages.paramsMetadata, sourceFilePath: undefined });
  }

  const azureDisabled = !messages.scope || !messages.templateMetadata || azure.running;

  return (
    <main id="webview-body">
      <section className="form-section">
        <DeploymentScopeInputView
          scope={messages.scope}
          onPickScope={messages.pickScope} />

        <ParametersInputView
          parameters={messages.paramsMetadata}
          template={messages.templateMetadata}
          disabled={azure.running}
          onValueChange={setParamValue}
          onEnableEditing={handleEnableParamEditing}
          onPickParametersFile={messages.pickParamsFile} />

        <div className="controls">
          <VSCodeButton onClick={azure.deploy} disabled={azureDisabled}>Deploy</VSCodeButton>
          <VSCodeButton onClick={azure.validate} disabled={azureDisabled}>Validate</VSCodeButton>
          <VSCodeButton onClick={azure.whatIf} disabled={azureDisabled}>What-If</VSCodeButton>
        </div>
      </section>

      <section className="form-section">
        {azure.running && <VSCodeProgressRing />}
        <ResultsView result={azure.result} />
        <DeploymentOperationsView operations={azure.operations} />
        <DeploymentOutputsView outputs={azure.outputs} />
        <WhatIfChangesView changes={azure.whatIfChanges} />
      </section>
    </main>
  );
};