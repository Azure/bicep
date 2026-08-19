// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { BicepTelemetry } from "../telemetry";

function createReporter() {
  return {
    dispose: vi.fn(),
    sendTelemetryErrorEvent: vi.fn(),
    sendTelemetryEvent: vi.fn(),
  };
}

describe("BicepTelemetry", () => {
  test("BicepTelemetry_WithUsageEvent_SendsUsageTelemetry", () => {
    const reporter = createReporter();
    const telemetry = new BicepTelemetry("connection-string", true, () => reporter);

    telemetry.sendEvent("usage", { feature: "deploy" }, { duration: 1 });

    expect(reporter.sendTelemetryEvent).toHaveBeenCalledWith("usage", { feature: "deploy" }, { duration: 1 });
    expect(reporter.sendTelemetryErrorEvent).not.toHaveBeenCalled();
  });

  test("BicepTelemetry_WithError_SendsTypeWithoutMessage", () => {
    const reporter = createReporter();
    const telemetry = new BicepTelemetry("connection-string", true, () => reporter);

    telemetry.sendError("failure", new TypeError("private value"), { operation: "deploy" });

    expect(reporter.sendTelemetryErrorEvent).toHaveBeenCalledWith(
      "failure",
      { operation: "deploy", errorType: "TypeError" },
      undefined,
    );
  });

  test("BicepTelemetry_WhenDisabled_DoesNotCreateReporter", () => {
    const reporterFactory = vi.fn(() => createReporter());
    const telemetry = new BicepTelemetry("connection-string", false, reporterFactory);

    telemetry.sendEvent("usage");
    telemetry.sendError("failure", new Error("private value"));
    telemetry.dispose();

    expect(reporterFactory).not.toHaveBeenCalled();
  });
});
