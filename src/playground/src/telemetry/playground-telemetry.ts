// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface PlaygroundTelemetry {
  trackEvent(
    event: { name: string },
    customProperties?: Record<string, string>,
  ): void;
}
