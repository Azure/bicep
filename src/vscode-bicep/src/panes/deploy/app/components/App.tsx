// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { FC, useState } from "react";
import { VSCodeButton, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
import "./index.css";
import { ParamData } from "../../models";
import { useMessageHandler } from "./hooks/useMessageHandler";
import { WhatIfChangesView } from "./sections/WhatIfChangesView";
import { DeploymentOperationsView } from "./sections/DeploymentOperationsView";
import { DeploymentOutputsView } from "./sections/DeploymentOutputsView";
import { ResultsView } from "./sections/ResultsView";
import { ParametersInputView } from "./sections/ParametersInputView";
import { useAzure } from "./hooks/useAzure";
import { DeploymentScopeInputView } from "./sections/DeploymentScopeInputView";
import { FormSection } from "./sections/FormSection";

export const App: FC = () => {
  const [errorMessage, setErrorMessage] = useState<string>();
  const messages = useMessageHandler({ setErrorMessage });
  const azure = useAzure({
    scope: messages.scope,
    acquireAccessToken: messages.acquireAccessToken,
    templateMetadata: messages.templateMetadata,
    parametersMetadata: messages.paramsMetadata,
    setErrorMessage
  });

  function setParamValue(key: string, data: ParamData) {
    const parameters = Object.assign({}, messages.paramsMetadata.parameters, { [key]: data });
    messages.setParamsMetadata({ ...messages.paramsMetadata, parameters });
  }

  function handleEnableParamEditing() {
    messages.setParamsMetadata({ ...messages.paramsMetadata, sourceFilePath: undefined });
  }

  const azureDisabled = !messages.scope || !messages.templateMetadata || azure.running;

  async function handleDeployClick() {
    messages.publishTelemetry('deployPane/deploy', {});
    await azure.deploy();
  }

  async function handleValidateClick() {
    messages.publishTelemetry('deployPane/validate', {});
    await azure.validate();
  }

  async function handleWhatIfClick() {
    messages.publishTelemetry('deployPane/whatIf', {});
    await azure.whatIf();
  }

  return (
    <main id="webview-body">
      <FormSection title="Experimental Warning">
        <div className="alert-error">
          <span className="codicon codicon-beaker" />
          The Bicep Deployment Pane is an experimental feature.
          <br/>
          Documentation is available <a href="https://github.com/Azure/bicep/blob/main/docs/deploy-ui.md">here</a>. Please raise issues or feature requests <a href={createNewIssueUrl()}>here</a>.
        </div>
      </FormSection>
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

      <FormSection title="Actions">
        {errorMessage && <div className="alert-error">
          <span className="codicon codicon-error" />
          {errorMessage}
        </div>}
        <div className="controls">
          <VSCodeButton onClick={handleDeployClick} disabled={azureDisabled}>Deploy</VSCodeButton>
          <VSCodeButton onClick={handleValidateClick} disabled={azureDisabled}>Validate</VSCodeButton>
          <VSCodeButton onClick={handleWhatIfClick} disabled={azureDisabled}>What-If</VSCodeButton>
        </div>
        {azure.running && <VSCodeProgressRing></VSCodeProgressRing>}
      </FormSection>

      {messages.scope && <>
        <ResultsView result={azure.result} />
        <DeploymentOperationsView scope={messages.scope} operations={azure.operations} />
        <DeploymentOutputsView outputs={azure.outputs} />
        <WhatIfChangesView changes={azure.whatIfChanges} />
      </>}
    </main>
  );
};

function createNewIssueUrl() {
  const title = 'Deployment Pane: <description>'
  const labels = ['story: deploy pane']
  const body = `

`;

  return `https://github.com/Azure/bicep/issues/new?title=${encodeURIComponent(title)}&body=${encodeURIComponent(body)}&labels=${encodeURIComponent(labels.join(','))}`;
}