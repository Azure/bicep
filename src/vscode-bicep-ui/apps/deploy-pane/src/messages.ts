// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { AccessToken } from "@azure/identity";
import type { TelemetryProperties } from "@microsoft/vscode-azext-utils";
import type { DeploymentScope, DeploymentScopeType, DeployPaneState, UntypedError } from "./models";

export interface LocalDeploymentOperationError {
  code: string;
  message: string;
  target: string;
}

export interface LocalDeploymentOperationContent {
  resourceName: string;
  provisioningState: string;
  error?: LocalDeploymentOperationError;
}

interface LocalDeploymentContent {
  provisioningState: string;
  outputs: Record<string, unknown>;
  error?: LocalDeploymentOperationError;
}

export interface LocalDeployResponse {
  deployment: LocalDeploymentContent;
  operations: LocalDeploymentOperationContent[];
}

interface SimpleMessage<T> {
  kind: T;
}

type MessageWithPayload<T, U = Record<string, unknown>> = SimpleMessage<T> & U;

export type ReadyMessage = SimpleMessage<"READY">;
export function createReadyMessage(): ReadyMessage {
  return createSimpleMessage("READY");
}

export type DeploymentDataMessage = MessageWithPayload<
  "DEPLOYMENT_DATA",
  {
    documentPath: string;
    localDeployEnabled: boolean;
    templateJson?: string;
    parametersJson?: string;
    errorMessage?: string;
  }
>;
export function createDeploymentDataMessage(
  documentPath: string,
  localDeployEnabled: boolean,
  templateJson?: string,
  parametersJson?: string,
  errorMessage?: string,
): DeploymentDataMessage {
  return createMessageWithPayload("DEPLOYMENT_DATA", {
    documentPath,
    localDeployEnabled,
    templateJson,
    parametersJson,
    errorMessage,
  });
}

export type GetStateMessage = SimpleMessage<"GET_STATE">;
export function createGetStateMessage(): GetStateMessage {
  return createSimpleMessage("GET_STATE");
}

export type GetStateResultMessage = MessageWithPayload<
  "GET_STATE_RESULT",
  {
    state: DeployPaneState;
  }
>;
export function createGetStateResultMessage(state: DeployPaneState): GetStateResultMessage {
  return createMessageWithPayload("GET_STATE_RESULT", {
    state,
  });
}

export type SaveStateMessage = MessageWithPayload<
  "SAVE_STATE",
  {
    state: DeployPaneState;
  }
>;
export function createSaveStateMessage(state: DeployPaneState): SaveStateMessage {
  return createMessageWithPayload("SAVE_STATE", {
    state,
  });
}

export type GetAccessTokenMessage = MessageWithPayload<
  "GET_ACCESS_TOKEN",
  {
    scope: DeploymentScope;
  }
>;
export function createGetAccessTokenMessage(scope: DeploymentScope): GetAccessTokenMessage {
  return createMessageWithPayload("GET_ACCESS_TOKEN", {
    scope,
  });
}

export type GetAccessTokenResultMessage = MessageWithPayload<
  "GET_ACCESS_TOKEN_RESULT",
  {
    accessToken?: AccessToken;
    error?: UntypedError;
  }
>;
export function createGetAccessTokenResultMessage(
  accessToken?: AccessToken,
  error?: UntypedError,
): GetAccessTokenResultMessage {
  return createMessageWithPayload("GET_ACCESS_TOKEN_RESULT", {
    accessToken,
    error,
  });
}

export type PickParamsFileMessage = SimpleMessage<"PICK_PARAMS_FILE">;
export function createPickParamsFileMessage(): PickParamsFileMessage {
  return createSimpleMessage("PICK_PARAMS_FILE");
}

export type PickParamsFileResultMessage = MessageWithPayload<
  "PICK_PARAMS_FILE_RESULT",
  {
    documentPath: string;
    parametersJson: string;
  }
>;
export function createPickParamsFileResultMessage(
  documentPath: string,
  parametersJson: string,
): PickParamsFileResultMessage {
  return createMessageWithPayload("PICK_PARAMS_FILE_RESULT", {
    documentPath,
    parametersJson,
  });
}

export type GetDeploymentScopeMessage = MessageWithPayload<
  "GET_DEPLOYMENT_SCOPE",
  {
    scopeType: DeploymentScopeType;
  }
>;
export function createGetDeploymentScopeMessage(scopeType: DeploymentScopeType): GetDeploymentScopeMessage {
  return createMessageWithPayload("GET_DEPLOYMENT_SCOPE", {
    scopeType,
  });
}

export type GetDeploymentScopeResultMessage = MessageWithPayload<
  "GET_DEPLOYMENT_SCOPE_RESULT",
  {
    scope: DeploymentScope;
  }
>;
export function createGetDeploymentScopeResultMessage(scope: DeploymentScope): GetDeploymentScopeResultMessage {
  return createMessageWithPayload("GET_DEPLOYMENT_SCOPE_RESULT", {
    scope,
  });
}

export type PublishTelemetryMessage = MessageWithPayload<
  "PUBLISH_TELEMETRY",
  {
    eventName: string;
    properties: TelemetryProperties;
  }
>;
export function createPublishTelemetryMessage(
  eventName: string,
  properties: TelemetryProperties,
): PublishTelemetryMessage {
  return createMessageWithPayload("PUBLISH_TELEMETRY", {
    eventName,
    properties,
  });
}

export type LocalDeployMessage = SimpleMessage<"LOCAL_DEPLOY">;
export function createLocalDeployMessage(): LocalDeployMessage {
  return createSimpleMessage("LOCAL_DEPLOY");
}

export type LocalDeployResultMessage = MessageWithPayload<"LOCAL_DEPLOY_RESULT", LocalDeployResponse>;
export function createLocalDeployResultMessage(response: LocalDeployResponse): LocalDeployResultMessage {
  return createMessageWithPayload("LOCAL_DEPLOY_RESULT", response);
}

export type VscodeMessage =
  | DeploymentDataMessage
  | GetStateResultMessage
  | PickParamsFileResultMessage
  | GetAccessTokenResultMessage
  | GetDeploymentScopeResultMessage
  | LocalDeployResultMessage;

export type ViewMessage =
  | ReadyMessage
  | GetStateMessage
  | SaveStateMessage
  | PickParamsFileMessage
  | GetAccessTokenMessage
  | GetDeploymentScopeMessage
  | PublishTelemetryMessage
  | LocalDeployMessage;

function createSimpleMessage<T>(kind: T): SimpleMessage<T> {
  return { kind };
}

function createMessageWithPayload<T extends string, U = Record<string, unknown>>(
  kind: T,
  payload: U,
): MessageWithPayload<T, U> {
  return {
    kind,
    ...payload,
  };
}
