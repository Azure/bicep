// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { TelemetryEventMeasurements, TelemetryEventProperties, TelemetryReporter } from "@vscode/extension-telemetry";
import { Disposable } from "vscode";

type Reporter = Pick<TelemetryReporter, "dispose" | "sendTelemetryErrorEvent" | "sendTelemetryEvent">;

export interface Telemetry {
  sendEvent(eventName: string, properties?: TelemetryEventProperties, measurements?: TelemetryEventMeasurements): void;
  sendError(
    eventName: string,
    error?: unknown,
    properties?: TelemetryEventProperties,
    measurements?: TelemetryEventMeasurements,
  ): void;
}

export class BicepTelemetry implements Telemetry, Disposable {
  private readonly reporter: Reporter | undefined;

  public constructor(
    connectionString: string,
    enabled = true,
    createReporter = (value: string): Reporter => new TelemetryReporter(value),
  ) {
    this.reporter = enabled ? createReporter(connectionString) : undefined;
  }

  public sendEvent(
    eventName: string,
    properties?: TelemetryEventProperties,
    measurements?: TelemetryEventMeasurements,
  ): void {
    this.reporter?.sendTelemetryEvent(eventName, properties, measurements);
  }

  public sendError(
    eventName: string,
    error?: unknown,
    properties?: TelemetryEventProperties,
    measurements?: TelemetryEventMeasurements,
  ): void {
    const errorProperties = error
      ? {
          ...properties,
          errorType: error instanceof Error ? error.name : typeof error,
        }
      : properties;
    this.reporter?.sendTelemetryErrorEvent(eventName, errorProperties, measurements);
  }

  public dispose(): void {
    void this.reporter?.dispose();
  }
}
