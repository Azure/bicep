// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { FC, useState } from "react";
import { VSCodeButton, VSCodeDivider, VSCodeProgressRing } from "@vscode/webview-ui-toolkit/react";
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
      <VSCodeDivider />

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
        {errorMessage && <div
          style={{
            color: "var(--vscode-statusBarItem-errorForeground)",
            backgroundColor: "var(--vscode-statusBarItem-errorBackground)",
            padding: '5px 10px',
            borderRadius: '4px',
            fontSize: '14px',
            alignSelf: 'center'
          }}>
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