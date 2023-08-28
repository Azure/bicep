// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
    CancellationToken,
    TestController,
    TestItem,
    TestMessage,
    TestRunProfileKind,
    TestRunRequest,
    TestTag,
    tests,
  } from "vscode";
  import { LanguageClient, TextDocumentIdentifier } from "vscode-languageclient/node";
  import {
    TestsDiscoveredParams,
    runTestRequestType,
    testsDiscoveredNotificationType,
  } from "../language";
  import { callWithTelemetryAndErrorHandlingOnlyOnErrors } from "./telemetry";
  
  const fileTestTag = new TestTag('file');
  
  async function runHandler(
    controller: TestController,
    client: LanguageClient,
    request: TestRunRequest,
    token: CancellationToken,
  ) {
    const run = controller.createTestRun(request);
    const queue: TestItem[] = [];
  
    // Loop through all included tests, or all known tests, and add them to our queue
    if (request.include) {
      request.include.forEach((test) => queue.push(test));
    } else {
      controller.items.forEach((test) => queue.push(test));
    }
  
    while (queue.length > 0 && !token.isCancellationRequested) {
      const test = queue.pop()!;
  
      // all tests should have an associated URI
      if (!test.uri) {
        continue;
      }
  
      // Skip tests the user asked to exclude
      if (request.exclude?.includes(test)) {
        continue;
      }
  
      test.children.forEach((test) => queue.push(test));
  
      // skip executing "file" tests
      if (test.tags.some(t => t.id === fileTestTag.id)) {
        continue;
      }
  
      const response = await client.sendRequest(runTestRequestType, {
        textDocument: TextDocumentIdentifier.create(client.code2ProtocolConverter.asUri(test.uri)),
        testId: test.id,
      });
  
      if (response.success) {
        run.passed(test);
      } else {
        run.failed(test, new TestMessage(response.message ?? ""));
      }
      run.end()
    }
  }
  
  function handleTestsDiscovered(
    controller: TestController,
    client: LanguageClient,
    params: TestsDiscoveredParams,
  ) {
    const testFileId = params.textDocument.uri;
    const documentUri = client.protocol2CodeConverter.asUri(
      params.textDocument.uri,
    );
  
    if (params.tests.length === 0) {
      controller.items.delete(testFileId);
      return;
    }
  
    const fileItem = controller.createTestItem(
      testFileId,
      `$(symbol-file)${testFileId}`,
      documentUri,
    );
    fileItem.tags = [fileTestTag];
  
    for (const test of params.tests) {
      const testItem = controller.createTestItem(test.id, `$(symbol-module)${test.id}`, documentUri);
      testItem.range = client.protocol2CodeConverter.asRange(test.range);
      fileItem.children.add(testItem);
    }
  
    controller.items.add(fileItem);
  }
  
  export function createTestController(client: LanguageClient) {
    const controller = tests.createTestController("bicep-tests", "Bicep Tests");
  
    controller.createRunProfile("Run", TestRunProfileKind.Run, (request, token) =>
      runHandler(controller, client, request, token),
    );
  
    client.onNotification(testsDiscoveredNotificationType, async (params) => await callWithTelemetryAndErrorHandlingOnlyOnErrors(
        'handleTestsDiscovered',
        () => handleTestsDiscovered(controller, client, params))
    );
  
    return controller;
  }