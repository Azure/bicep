// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export { activateExternalSourceFeature, ShowModuleSourceFileCommand } from "./commands";
export { BicepExternalSourceContentProvider } from "./external-source-content";
export { BicepExternalSourceScheme, decodeExternalSourceUri, type ExternalSource } from "./external-source-uri";
export {
  type BicepExternalSourceParams,
  bicepExternalSourceRequestType,
  type BicepExternalSourceResponse,
} from "./protocol";
