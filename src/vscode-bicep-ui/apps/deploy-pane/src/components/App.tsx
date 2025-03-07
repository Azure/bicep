// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { ParametersInputData } from "./sections/ParametersInputView";

import { VscodeButton, VscodeProgressRing } from "@vscode-elements/react-elements";
import { useState } from "react";
import { ErrorAlert } from "./ErrorAlert";
import { useAzure } from "./hooks/useAzure";
import { useMessageHandler } from "./hooks/useMessageHandler";
import { LocalDeployOperations, LocalDeployOutputs, LocalDeployResult } from "./localDeploy";
import { DeploymentOperationsView } from "./sections/DeploymentOperationsView";
import { DeploymentOutputsView } from "./sections/DeploymentOutputsView";
import { DeploymentScopeInputView } from "./sections/DeploymentScopeInputView";
import { FormSection } from "./sections/FormSection";
import { ParametersInputView } from "./sections/ParametersInputView";
import { ResultsView } from "./sections/ResultsView";
import { WhatIfChangesView } from "./sections/WhatIfChangesView";

export const App: FC = () => {
  const [errorMessage, setErrorMessage] = useState<string>();
  const [localDeployRunning, setLocalDeployRunning] = useState(false);
  const [parameters, setParameters] = useState<ParametersInputData>({});
  const messages = useMessageHandler({ setErrorMessage, setLocalDeployRunning });
  const azure = useAzure({
    scope: messages.scope,
    acquireAccessToken: messages.acquireAccessToken,
    templateMetadata: messages.templateMetadata,
    setErrorMessage,
  });
  const isRunning = azure.deployState.status === "running" || localDeployRunning;

  function handleEnableParamEditing() {
    messages.setParamsMetadata({ ...messages.paramsMetadata, sourceFilePath: undefined });
  }

  const parametersValid = Object.values(parameters).every((param) => param.isValid);
  const azureDisabled = !messages.scope || !messages.templateMetadata || isRunning || !parametersValid;

  async function handleDeployClick() {
    messages.publishTelemetry("deployPane/deploy", {});
    await azure.deploy(parameters);
  }

  async function handleValidateClick() {
    messages.publishTelemetry("deployPane/validate", {});
    await azure.validate(parameters);
  }

  async function handleWhatIfClick() {
    messages.publishTelemetry("deployPane/whatIf", {});
    await azure.whatIf(parameters);
  }

  async function handleLocalDeployClick() {
    messages.publishTelemetry("deployPane/localDeploy", {});
    messages.startLocalDeploy();
  }

  if (!messages.messageState.initialized) {
    return <VscodeProgressRing />;
  }

  const showLocalDeployControls =
    messages.messageState.localDeployEnabled &&
    // if there's an error, this'll cause sourceFilePath to be empty - we still want to show the controls to display the error
    (errorMessage || messages.paramsMetadata.sourceFilePath?.endsWith(".bicepparam"));

  return (
    <main id="webview-body">
      {!messages.messageState.localDeployEnabled && (
        <>
          <DeploymentScopeInputView scope={messages.scope} onPickScope={messages.pickScope} />

          <ParametersInputView
            parameters={messages.paramsMetadata}
            template={messages.templateMetadata}
            disabled={isRunning}
            onParametersChange={setParameters}
            onEnableEditing={handleEnableParamEditing}
            onPickParametersFile={messages.pickParamsFile}
          />

          <FormSection title="Actions">
            {errorMessage && <ErrorAlert message={errorMessage} />}
            <div className="controls">
              <VscodeButton onClick={handleDeployClick} disabled={azureDisabled}>
                Deploy
              </VscodeButton>
              <VscodeButton onClick={handleValidateClick} disabled={azureDisabled}>
                Validate
              </VscodeButton>
              <VscodeButton onClick={handleWhatIfClick} disabled={azureDisabled}>
                What-If
              </VscodeButton>
            </div>
          </FormSection>

          {messages.scope && (
            <>
              <ResultsView scope={messages.scope} deployState={azure.deployState} />
              <DeploymentOperationsView scope={messages.scope} operations={azure.operations} />
              <DeploymentOutputsView outputs={azure.outputs} />
              <WhatIfChangesView changes={azure.whatIfChanges} />
            </>
          )}
        </>
      )}

      {messages.messageState.localDeployEnabled && (
        <>
          {showLocalDeployControls && (
            <>
              <ParametersInputView
                parameters={messages.paramsMetadata}
                template={messages.templateMetadata}
                disabled={localDeployRunning}
                onParametersChange={setParameters}
                onEnableEditing={handleEnableParamEditing}
                onPickParametersFile={messages.pickParamsFile}
              />

              <FormSection title="Actions">
                {errorMessage && <ErrorAlert message={errorMessage} />}
                <div className="controls">
                  <VscodeButton onClick={handleLocalDeployClick} disabled={localDeployRunning}>
                    Deploy
                  </VscodeButton>
                </div>
                {localDeployRunning && <VscodeProgressRing></VscodeProgressRing>}
              </FormSection>

              {!localDeployRunning && messages.localDeployResult && (
                <>
                  <LocalDeployResult result={messages.localDeployResult} />
                  <LocalDeployOperations result={messages.localDeployResult} />
                  <LocalDeployOutputs result={messages.localDeployResult} />
                </>
              )}
            </>
          )}
          {!showLocalDeployControls && (
            <ErrorAlert message="Local Deployment is only currently supported for .bicepparam files. Relaunch this pane for a .bicepparam file." />
          )}
        </>
      )}
    </main>
  );
};
