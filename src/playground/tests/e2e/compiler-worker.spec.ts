// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  expect,
  getCompileRequestCount,
  openPlayground,
  test,
} from "./fixtures/playground";

type WorkerResponse = {
  type: "ready" | "result" | "error";
  requestId?: number;
  code?: string;
};

test.describe("compiler worker", () => {
  test("shows retry UI when initialization times out", async ({ page }) => {
    await page.addInitScript(() => {
      const nativeSetTimeout = window.setTimeout.bind(window);
      window.setTimeout = ((
        handler: TimerHandler,
        timeout?: number,
        ...arguments_: unknown[]
      ) =>
        nativeSetTimeout(
          handler,
          timeout === 30_000 ? 50 : timeout,
          ...arguments_,
        )) as typeof window.setTimeout;

      class UnresponsiveWorker extends EventTarget {
        public postMessage(): void {}
        public terminate(): void {}
      }

      Object.defineProperty(window, "Worker", {
        configurable: true,
        value: UnresponsiveWorker,
      });
    });

    await page.goto("/");

    await expect(
      page.getByRole("heading", { name: "Bicep Playground could not start" }),
    ).toBeVisible();
    await expect(page.getByText("took too long to initialize")).toBeVisible();
    await expect(page.getByRole("button", { name: "Retry" })).toBeVisible();
  });

  test("settles queued requests once and identifies superseded work", async ({
    page,
    playground,
    request,
  }) => {
    test.setTimeout(90_000);
    const sourcePath =
      "microsoft.desktopvirtualization/azure-virtual-desktop-with-fslogix/main.bicep";
    const quickstartsBaseUrl =
      "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/c3cf6301a0820e0ef1f3725f6b4a86a07bd177c8/quickstarts/";
    const quickstartResponse = await request.get(
      new URL(sourcePath, quickstartsBaseUrl).href,
    );
    expect(quickstartResponse.ok()).toBe(true);

    await expect
      .poll(() =>
        page.evaluate(() =>
          performance
            .getEntriesByType("resource")
            .map((entry) => entry.name)
            .find((name) => name.includes("compiler.worker")),
        ),
      )
      .toBeTruthy();
    const workerUrl = await page.evaluate(
      () =>
        performance
          .getEntriesByType("resource")
          .map((entry) => entry.name)
          .find((name) => name.includes("compiler.worker")) as string,
    );

    await expect(playground.bicepEditor).toBeVisible();
    const responses = await page.evaluate(
      ({
        compilerWorkerUrl,
        initialContent,
        initialSourcePath,
        moduleBaseUrl,
      }) =>
        new Promise<WorkerResponse[]>((resolve, reject) => {
          const worker = new Worker(compilerWorkerUrl, { type: "module" });
          const timeout = window.setTimeout(() => {
            worker.terminate();
            reject(
              new Error("Timed out waiting for compiler worker responses."),
            );
          }, 60_000);
          const responses: WorkerResponse[] = [];

          worker.addEventListener("error", (event) => {
            window.clearTimeout(timeout);
            worker.terminate();
            reject(new Error(event.message));
          });
          worker.addEventListener(
            "message",
            (event: MessageEvent<WorkerResponse>) => {
              if (event.data.type === "ready") {
                worker.postMessage({
                  type: "request",
                  requestId: 1,
                  operation: "compile",
                  content: initialContent,
                  sourcePath: initialSourcePath,
                });
                worker.postMessage({
                  type: "request",
                  requestId: 2,
                  operation: "compile",
                  content: "param superseded string",
                });
                worker.postMessage({
                  type: "request",
                  requestId: 3,
                  operation: "compile",
                  content: "param latest string",
                });
                return;
              }

              responses.push(event.data);
              if (responses.length === 3) {
                window.clearTimeout(timeout);
                worker.terminate();
                resolve(responses);
              }
            },
          );
          worker.postMessage({
            type: "initialize",
            frameworkUrl: new URL("_framework/dotnet.js", document.baseURI)
              .href,
            quickstartsBaseUrl: moduleBaseUrl,
          });
        }),
      {
        compilerWorkerUrl: workerUrl,
        initialContent: await quickstartResponse.text(),
        initialSourcePath: sourcePath,
        moduleBaseUrl: quickstartsBaseUrl,
      },
    );

    expect(responses).toHaveLength(3);
    expect(responses.map(({ requestId }) => requestId).sort()).toEqual([
      1, 2, 3,
    ]);
    expect(responses.find(({ requestId }) => requestId === 1)?.type).toBe(
      "result",
    );
    expect(responses.find(({ requestId }) => requestId === 2)).toMatchObject({
      type: "error",
      code: "requestSuperseded",
    });
    expect(responses.find(({ requestId }) => requestId === 3)?.type).toBe(
      "result",
    );
  });

  test("recovers once after a post-start crash", async ({ page }) => {
    test.setTimeout(90_000);
    await page.addInitScript(() => {
      const NativeWorker = window.Worker;
      const workers: Worker[] = [];
      let compileRequestCount = 0;

      class TrackedWorker extends NativeWorker {
        public constructor(scriptUrl: string | URL, options?: WorkerOptions) {
          super(scriptUrl, options);

          if (scriptUrl.toString().includes("compiler.worker")) {
            workers.push(this);
            const nativePostMessage = this.postMessage.bind(this);
            this.postMessage = ((
              message: unknown,
              transfer?: Transferable[],
            ) => {
              if (
                typeof message === "object" &&
                message !== null &&
                "operation" in message &&
                message.operation === "compile"
              ) {
                ++compileRequestCount;
              }

              nativePostMessage(message, transfer ?? []);
            }) as typeof this.postMessage;
          }
        }
      }

      Object.defineProperties(window, {
        __playgroundCompileRequestCount: {
          get: () => compileRequestCount,
        },
        __playgroundWorkers: {
          value: workers,
        },
        Worker: {
          configurable: true,
          value: TrackedWorker,
        },
      });
    });
    const playground = await openPlayground(page);
    const slowSource = Array.from(
      { length: 1_000 },
      (_, index) => `var value${index} = ${index}`,
    ).join("\n");
    await playground.replaceEditorText(playground.bicepEditor, slowSource);
    await expect.poll(() => getCompileRequestCount(page)).toBeGreaterThan(0);

    await page.evaluate(() => {
      const workers = (
        window as typeof window & { __playgroundWorkers: Worker[] }
      ).__playgroundWorkers;
      workers[0]?.dispatchEvent(
        new ErrorEvent("error", {
          cancelable: true,
          message: "Simulated compiler worker crash.",
        }),
      );
    });

    await expect
      .poll(() =>
        page.evaluate(
          () =>
            (window as typeof window & { __playgroundWorkers: Worker[] })
              .__playgroundWorkers.length,
        ),
      )
      .toBe(2);

    await playground.replaceEditorText(
      playground.bicepEditor,
      "param recovered string = 'yes'",
    );
    await expect
      .poll(() => playground.readEditorText(playground.armTemplate))
      .toContain('"defaultValue": "yes"');
  });
});
