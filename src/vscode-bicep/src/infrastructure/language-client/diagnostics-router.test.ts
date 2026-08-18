// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Diagnostic, Uri } from "vscode";
import type { LanguageClientOptions } from "vscode-languageclient/node";
import { DiagnosticsRouter } from "./diagnostics-router";

describe("DiagnosticsRouter", () => {
  const uri = {} as Uri;
  const diagnostics: Diagnostic[] = [];

  function createRouter(): { clientOptions: LanguageClientOptions; router: DiagnosticsRouter } {
    const clientOptions: LanguageClientOptions = {};
    return { clientOptions, router: new DiagnosticsRouter(clientOptions) };
  }

  test("notifies every subscriber", () => {
    const { clientOptions, router } = createRouter();
    const notifications: string[] = [];
    const firstSubscriber = () => notifications.push("first");
    const secondSubscriber = () => notifications.push("second");
    router.subscribe(firstSubscriber);
    router.subscribe(secondSubscriber);

    clientOptions.middleware?.handleDiagnostics?.(uri, diagnostics, () => undefined);

    expect(notifications).toEqual(["first", "second"]);
  });

  test("calls the next diagnostics handler", () => {
    const { clientOptions } = createRouter();
    let received: [Uri, Diagnostic[]] | undefined;

    clientOptions.middleware?.handleDiagnostics?.(uri, diagnostics, (nextUri, nextDiagnostics) => {
      received = [nextUri, nextDiagnostics];
    });

    expect(received).toEqual([uri, diagnostics]);
  });

  test("keeps other subscribers when one is disposed", () => {
    const { clientOptions, router } = createRouter();
    const notifications: string[] = [];
    const firstSubscriber = () => notifications.push("first");
    const secondSubscriber = () => notifications.push("second");
    const firstSubscription = router.subscribe(firstSubscriber);
    router.subscribe(secondSubscriber);

    firstSubscription.dispose();
    clientOptions.middleware?.handleDiagnostics?.(uri, diagnostics, () => undefined);

    expect(notifications).toEqual(["second"]);
  });

  test("removes subscribers regardless of disposal order", () => {
    const { clientOptions, router } = createRouter();
    const notifications: string[] = [];
    const firstSubscriber = () => notifications.push("first");
    const secondSubscriber = () => notifications.push("second");
    const firstSubscription = router.subscribe(firstSubscriber);
    const secondSubscription = router.subscribe(secondSubscriber);

    secondSubscription.dispose();
    firstSubscription.dispose();
    clientOptions.middleware?.handleDiagnostics?.(uri, diagnostics, () => undefined);

    expect(notifications).toEqual([]);
  });
});
