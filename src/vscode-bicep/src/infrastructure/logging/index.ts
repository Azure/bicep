// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export {
  activateWithTelemetryAndErrorHandling,
  callWithTelemetryAndErrorHandlingOnlyOnErrors,
  createLogger,
  e2eLogName,
  getLogger,
  raiseErrorWithoutTelemetry,
  resetLogger,
  WinstonLogger,
} from "./logging";
export type { Logger, LogLevel } from "./logging";
export { createAzExtOutputChannel, OutputChannelManager } from "./output-channels";
