// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { useEffect, useState } from "react";
import { vscode } from "../../vscode";
import {
  VscodeMessage,
  createGetAccessTokenMessage,
  createGetDeploymentScopeMessage,
  createGetStateMessage,
  createPickParamsFileMessage,
  createPublishTelemetryMessage,
  createReadyMessage,
  createSaveStateMessage,
} from "../../../messages";
import { parseParametersJson, parseTemplateJson } from "../utils";
import {
  DeployPaneState,
  DeploymentScope,
  ParametersMetadata,
  TemplateMetadata,
  UntypedError,
} from "../../../models";
import { AccessToken } from "@azure/identity";
import { TelemetryProperties } from "@microsoft/vscode-azext-utils";

export interface UseMessageHandlerProps {
  setErrorMessage: (message?: string) => void;
}

export function useMessageHandler(props: UseMessageHandlerProps) {
  const { setErrorMessage } = props;
  const [persistedState, setPersistedState] = useState<DeployPaneState>();
  const [templateMetadata, setTemplateMetadata] = useState<TemplateMetadata>();
  const [paramsMetadata, setParamsMetadata] = useState<ParametersMetadata>({
    parameters: {},
  });
  const [scope, setScope] = useState<DeploymentScope>();
   const [accessTokenResolver, setAccessTokenResolver] = useState(null);

  const handleMessageEvent = (e: MessageEvent<VscodeMessage>) => {
    const message = e.data;
    switch (message.kind) {
      case "DEPLOYMENT_DATA": {
        if (!message.templateJson) {
          setTemplateMetadata(undefined);
          setErrorMessage(
            message.errorMessage ??
              "An error occurred building the deployment object.",
          );
          return;
        }

        const templateMetadata = parseTemplateJson(message.templateJson);

        if (
          templateMetadata.template["$schema"] !==
          "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"
        ) {
          setTemplateMetadata(undefined);
          setErrorMessage(
            "The deployment pane currently only supports resourceGroup-scoped Bicep files.",
          );
          return;
        }

        setTemplateMetadata(templateMetadata);
        if (message.parametersJson) {
          setParamsMetadata({
            sourceFilePath: message.documentPath.endsWith(".bicep")
              ? undefined
              : message.documentPath,
            parameters: parseParametersJson(message.parametersJson),
          });
        }
        setErrorMessage(undefined);
        return;
      }
      case "GET_STATE_RESULT": {
        setPersistedState(message.state);
        setScope(message.state.scope);
        return;
      }
      case "PICK_PARAMS_FILE_RESULT": {
        setParamsMetadata({
          sourceFilePath: message.documentPath,
          parameters: parseParametersJson(message.parametersJson),
        });
        return;
      }
      case "GET_ACCESS_TOKEN_RESULT": {
        if (message.accessToken) {
          accessTokenResolver.resolve(message.accessToken);
        } else {
          accessTokenResolver.reject(
            message.error ?? "Failed to authenticate with Azure",
          );
        }
        return;
      }
      case "GET_DEPLOYMENT_SCOPE_RESULT": {
        setScope(message.scope);
        savePersistedState({ ...persistedState, scope: message.scope });
        return;
      }
    }
  };

  useEffect(() => {
    window.addEventListener("message", handleMessageEvent);
    vscode.postMessage(createReadyMessage());
    vscode.postMessage(createGetStateMessage());
    return () => window.removeEventListener("message", handleMessageEvent);
  }, []);

  function savePersistedState(state: DeployPaneState) {
    vscode.postMessage(createSaveStateMessage(state));
    setPersistedState(state);
  }

  function pickParamsFile() {
    vscode.postMessage(createPickParamsFileMessage());
  }

  function pickScope() {
    vscode.postMessage(createGetDeploymentScopeMessage());
  }

  function publishTelemetry(
    eventName: string,
    properties: TelemetryProperties,
  ) {
    vscode.postMessage(createPublishTelemetryMessage(eventName, properties));
  }

 function acquireAccessToken() {
    const promise = new Promise<AccessToken>(
      (resolve, reject) => setAccessTokenResolver({ resolve, reject }),
    );

    vscode.postMessage(createGetAccessTokenMessage(scope!));
    return promise;
  }

  return {
    acquireAccessToken,
  };

  return {
    pickParamsFile,
    paramsMetadata,
    setParamsMetadata,
    templateMetadata,
    acquireAccessToken,
    pickScope,
    scope,
    publishTelemetry,
  };
}
