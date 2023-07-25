// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import {
  DeployPaneState,
  DeploymentScope,
  UntypedError,
} from "./app/components/models";

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
    templateJson: string;
    parametersJson?: string;
  }
>;
export function createDeploymentDataMessage(
  documentPath: string,
  templateJson: string,
  parametersJson?: string,
): DeploymentDataMessage {
  return createMessageWithPayload("DEPLOYMENT_DATA", {
    documentPath,
    templateJson,
    parametersJson,
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
export function createGetStateResultMessage(
  state: DeployPaneState,
): GetStateResultMessage {
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
export function createSaveStateMessage(
  state: DeployPaneState,
): SaveStateMessage {
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
export function createGetAccessTokenMessage(
  scope: DeploymentScope,
): GetAccessTokenMessage {
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

export type GetDeploymentScopeMessage = SimpleMessage<"GET_DEPLOYMENT_SCOPE">;
export function createGetDeploymentScopeMessage(): GetDeploymentScopeMessage {
  return createSimpleMessage("GET_DEPLOYMENT_SCOPE");
}

export type GetDeploymentScopeResultMessage = MessageWithPayload<
  "GET_DEPLOYMENT_SCOPE_RESULT",
  {
    scope: DeploymentScope;
  }
>;
export function createGetDeploymentScopeResultMessage(
  scope: DeploymentScope,
): GetDeploymentScopeResultMessage {
  return createMessageWithPayload("GET_DEPLOYMENT_SCOPE_RESULT", {
    scope,
  });
}

export type ShowUserErrorDialogMessage = MessageWithPayload<
  "SHOW_USER_ERROR_DIALOG",
  {
    callbackId: string;
    error: UntypedError;
  }
>;
export function createShowUserErrorDialogMessage(
  callbackId: string,
  error: UntypedError,
): ShowUserErrorDialogMessage {
  return createMessageWithPayload("SHOW_USER_ERROR_DIALOG", {
    callbackId,
    error,
  });
}

export type VscodeMessage =
  | DeploymentDataMessage
  | GetStateResultMessage
  | PickParamsFileResultMessage
  | GetAccessTokenResultMessage
  | GetDeploymentScopeResultMessage;

export type ViewMessage =
  | ReadyMessage
  | GetStateMessage
  | SaveStateMessage
  | PickParamsFileMessage
  | GetAccessTokenMessage
  | GetDeploymentScopeMessage
  | ShowUserErrorDialogMessage;

function createSimpleMessage<T>(kind: T): SimpleMessage<T> {
  return { kind };
}

function createMessageWithPayload<
  T extends string,
  U = Record<string, unknown>,
>(kind: T, payload: U): MessageWithPayload<T, U> {
  return {
    kind,
    ...payload,
  };
}
