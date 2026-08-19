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
  test("sends usage events as usage telemetry", () => {
    const reporter = createReporter();
    const telemetry = new BicepTelemetry("connection-string", true, () => reporter);

    telemetry.sendEvent("usage", { feature: "deploy" }, { duration: 1 });

    expect(reporter.sendTelemetryEvent).toHaveBeenCalledWith("usage", { feature: "deploy" }, { duration: 1 });
    expect(reporter.sendTelemetryErrorEvent).not.toHaveBeenCalled();
  });

  test("sends the error type without its message", () => {
    const reporter = createReporter();
    const telemetry = new BicepTelemetry("connection-string", true, () => reporter);

    telemetry.sendError("failure", new TypeError("private value"), { operation: "deploy" });

    expect(reporter.sendTelemetryErrorEvent).toHaveBeenCalledWith(
      "failure",
      { operation: "deploy", errorType: "TypeError" },
      undefined,
    );
  });

  test("does not create a reporter when disabled", () => {
    const reporterFactory = vi.fn(() => createReporter());
    const telemetry = new BicepTelemetry("connection-string", false, reporterFactory);

    telemetry.sendEvent("usage");
    telemetry.sendError("failure", new Error("private value"));
    telemetry.dispose();

    expect(reporterFactory).not.toHaveBeenCalled();
  });
});
