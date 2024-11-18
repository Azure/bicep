// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AccessToken } from "@azure/identity";
import type { TelemetryProperties } from "@microsoft/vscode-azext-utils";
import type { LocalDeployResponse, VscodeMessage } from "../../messages";
import type {
  DeploymentScope,
  DeployPaneState,
  ParametersMetadata,
  TemplateMetadata,
  UntypedError,
} from "../../models";

import { useEffect, useState } from "react";
import {
  createGetAccessTokenMessage,
  createGetDeploymentScopeMessage,
  createGetStateMessage,
  createLocalDeployMessage,
  createPickParamsFileMessage,
  createPublishTelemetryMessage,
  createReadyMessage,
  createSaveStateMessage,
} from "../../messages";
import { vscode } from "../../vscode";
import { parseParametersJson, parseTemplateJson } from "../utils";

// TODO see if there's a way to use react hooks instead of this hackery
let accessTokenResolver: {
  resolve: (accessToken: AccessToken) => void;
  reject: (error: UntypedError) => void;
};

export interface UseMessageHandlerProps {
  setErrorMessage: (message?: string) => void;
  setLocalDeployRunning: (running: boolean) => void;
}

type MessageHandlerState = {
  initialized: boolean;
  localDeployEnabled: boolean;
};

export function useMessageHandler(props: UseMessageHandlerProps) {
  const { setErrorMessage, setLocalDeployRunning } = props;
  const [persistedState, setPersistedState] = useState<DeployPaneState>();
  const [templateMetadata, setTemplateMetadata] = useState<TemplateMetadata>();
  const [paramsMetadata, setParamsMetadata] = useState<ParametersMetadata>({
    parameters: {},
  });
  const [messageState, setMessageState] = useState<MessageHandlerState>({
    initialized: false,
    localDeployEnabled: false,
  });
  const [scope, setScope] = useState<DeploymentScope>();
  const [localDeployResult, setLocalDeployResult] = useState<LocalDeployResponse>();

  useEffect(() => {
    const handleMessageEvent = (e: MessageEvent<VscodeMessage>) => {
      const message = e.data;
      switch (message.kind) {
        case "DEPLOYMENT_DATA": {
          setMessageState({
            initialized: true,
            localDeployEnabled: message.localDeployEnabled,
          });

          if (message.errorMessage || !message.templateJson) {
            setTemplateMetadata(undefined);
            setErrorMessage(message.errorMessage ?? "An error occurred compiling the Bicep file.");
            return;
          }

          const templateMetadata = parseTemplateJson(message.templateJson);

          if (!templateMetadata.scopeType) {
            setTemplateMetadata(undefined);
            setErrorMessage("Failed to obtain the deployment scope from compiled Bicep file.");
            return;
          }

          setTemplateMetadata(templateMetadata);
          if (message.parametersJson) {
            setParamsMetadata({
              sourceFilePath: message.documentPath.endsWith(".bicep") ? undefined : message.documentPath,
              parameters: parseParametersJson(message.parametersJson),
            });
          }
          setErrorMessage(undefined);
          return;
        }
        case "GET_STATE_RESULT": {
          setPersistedState(message.state);
          if (!message.state.scope.tenantId || !message.state.scope.portalUrl) {
            // If state was persisted with an older version of the extension, these properties won't have been set.
            // Force the user to re-pick the scope to reset them.
            return;
          }
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
            accessTokenResolver.reject(message.error ?? "Failed to authenticate with Azure");
          }
          return;
        }
        case "GET_DEPLOYMENT_SCOPE_RESULT": {
          setScope(message.scope);
          savePersistedState({ ...persistedState, scope: message.scope });
          return;
        }
        case "LOCAL_DEPLOY_RESULT": {
          setLocalDeployResult(message);
          setLocalDeployRunning(false);
          return;
        }
      }
    };

    window.addEventListener("message", handleMessageEvent);
    vscode.postMessage(createReadyMessage());
    vscode.postMessage(createGetStateMessage());
    return () => window.removeEventListener("message", handleMessageEvent);
    // TODO: This is an anti-pattern. We should refactor this.
    // We cannot add dependencies to this hook because it will cause infinite loops somehow.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function savePersistedState(state: DeployPaneState) {
    vscode.postMessage(createSaveStateMessage(state));
    setPersistedState(state);
  }

  function pickParamsFile() {
    vscode.postMessage(createPickParamsFileMessage());
  }

  function pickScope() {
    if (!templateMetadata?.scopeType) {
      throw `ScopeType not set`;
    }

    vscode.postMessage(createGetDeploymentScopeMessage(templateMetadata.scopeType));
  }

  function publishTelemetry(eventName: string, properties: TelemetryProperties) {
    vscode.postMessage(createPublishTelemetryMessage(eventName, properties));
  }

  function acquireAccessToken() {
    const promise = new Promise<AccessToken>((resolve, reject) => (accessTokenResolver = { resolve, reject }));

    vscode.postMessage(createGetAccessTokenMessage(scope!));
    return promise;
  }

  function startLocalDeploy() {
    setLocalDeployRunning(true);
    vscode.postMessage(createLocalDeployMessage());
  }

  return {
    pickParamsFile,
    paramsMetadata,
    setParamsMetadata,
    templateMetadata,
    acquireAccessToken,
    pickScope,
    scope,
    publishTelemetry,
    startLocalDeploy,
    localDeployResult,
    messageState,
  };
}
