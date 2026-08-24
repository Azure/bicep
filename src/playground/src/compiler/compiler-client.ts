// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { languages } from "monaco-editor";
import { getQuickstartsLink } from "../quickstarts/quickstarts";
import {
  CompileResult,
  CompilerRequest,
  CompilerResponse,
  CompilerResult,
  DecompileResult,
} from "./compiler-protocol";

const interopInitializationTimeoutMs = 30_000;
const maximumAutomaticWorkerRestarts = 1;

export class CompilerRequestSupersededError extends Error {
  public constructor() {
    super("A newer compiler request superseded this request.");
    this.name = "CompilerRequestSupersededError";
  }
}

export interface DotnetInterop {
  getSemanticTokensLegend(): languages.SemanticTokensLegend;
  getSemanticTokens(
    content: string,
    sourcePath?: string,
  ): Promise<languages.SemanticTokens>;
  compileAndEmitDiagnostics(
    content: string,
    sourcePath?: string,
  ): Promise<CompileResult>;
  decompile(jsonContent: string): Promise<DecompileResult>;
  dispose(): void;
}

type PendingRequest = {
  resolve: (result: CompilerResult) => void;
  reject: (error: Error) => void;
};

type Initialization = {
  resolve: () => void;
  reject: (error: Error) => void;
};

type RequestWithoutEnvelope<T> = T extends unknown
  ? Omit<T, "type" | "requestId">
  : never;

type CompilerOperationRequest = RequestWithoutEnvelope<
  Extract<CompilerRequest, { type: "request" }>
>;

export async function initializeInterop(
  timeoutMs = interopInitializationTimeoutMs,
): Promise<DotnetInterop> {
  const client = new CompilerWorkerClient();

  try {
    await client.initialize(timeoutMs);
    return client;
  } catch (error) {
    client.dispose();
    throw error;
  }
}

class CompilerWorkerClient implements DotnetInterop {
  private readonly pendingRequests = new Map<number, PendingRequest>();
  private nextRequestId = 0;
  private initialization: Initialization | undefined;
  private semanticTokensLegend: languages.SemanticTokensLegend | undefined;
  private worker: Worker | undefined;
  private ready: Promise<void> | undefined;
  private automaticRestartCount = 0;
  private initialized = false;
  private disposed = false;

  public async initialize(timeoutMs: number): Promise<void> {
    if (!this.ready) {
      this.ready = this.startWorker(timeoutMs);
    }

    await this.ready;
    this.initialized = true;
  }

  private async startWorker(timeoutMs: number): Promise<void> {
    const worker = new Worker(
      new URL("./compiler.worker.ts", import.meta.url),
      { type: "module" },
    );
    this.worker = worker;
    worker.addEventListener("message", this.handleMessage);
    worker.addEventListener("error", this.handleWorkerError);
    worker.addEventListener("messageerror", this.handleMessageError);

    await new Promise<void>((resolve, reject) => {
      const timeout = window.setTimeout(() => {
        this.initialization = undefined;
        this.disposeWorker(worker);
        reject(
          new Error(
            "The Bicep compiler took too long to initialize. Check your connection and try again.",
          ),
        );
      }, timeoutMs);

      this.initialization = {
        resolve: () => {
          window.clearTimeout(timeout);
          this.initialization = undefined;
          resolve();
        },
        reject: (error) => {
          window.clearTimeout(timeout);
          this.initialization = undefined;
          reject(error);
        },
      };

      this.postMessage({
        type: "initialize",
        frameworkUrl: new URL(
          "_framework/dotnet.js",
          document.baseURI,
        ).toString(),
        quickstartsBaseUrl: getQuickstartsLink(""),
      });
    });

    this.semanticTokensLegend =
      await this.sendRequestNow<languages.SemanticTokensLegend>({
        operation: "getSemanticTokensLegend",
      });
  }

  public getSemanticTokensLegend() {
    if (!this.semanticTokensLegend) {
      throw new Error("The Bicep compiler worker is not initialized.");
    }

    return this.semanticTokensLegend;
  }

  public getSemanticTokens(content: string, sourcePath?: string) {
    return this.sendRequest<languages.SemanticTokens>({
      operation: "getSemanticTokens",
      content,
      sourcePath,
    });
  }

  public compileAndEmitDiagnostics(content: string, sourcePath?: string) {
    return this.sendRequest<CompileResult>({
      operation: "compile",
      content,
      sourcePath,
    });
  }

  public decompile(content: string) {
    return this.sendRequest<DecompileResult>({
      operation: "decompile",
      content,
    });
  }

  public dispose() {
    if (this.disposed) {
      return;
    }

    this.disposed = true;
    this.disposeWorker();

    const error = new Error("The Bicep compiler worker was disposed.");
    this.initialization?.reject(error);
    this.initialization = undefined;
    this.rejectPendingRequests(error);
  }

  private async sendRequest<TResult extends CompilerResult>(
    request: CompilerOperationRequest,
  ): Promise<TResult> {
    await this.ready;
    return await this.sendRequestNow<TResult>(request);
  }

  private sendRequestNow<TResult extends CompilerResult>(
    request: CompilerOperationRequest,
  ): Promise<TResult> {
    const requestId = ++this.nextRequestId;

    return new Promise<TResult>((resolve, reject) => {
      this.pendingRequests.set(requestId, {
        resolve: (result) => resolve(result as TResult),
        reject,
      });
      this.postMessage({
        type: "request",
        requestId,
        ...request,
      } as CompilerRequest);
    });
  }

  private readonly handleMessage = (event: MessageEvent<CompilerResponse>) => {
    const response = event.data;

    if (response.type === "ready") {
      this.initialization?.resolve();
      return;
    }

    if (response.type === "error") {
      if (response.requestId === undefined) {
        this.initialization?.reject(new Error(response.message));
        return;
      }

      const pending = this.pendingRequests.get(response.requestId);
      if (!pending) {
        return;
      }

      this.pendingRequests.delete(response.requestId);
      pending.reject(
        response.code === "requestSuperseded"
          ? new CompilerRequestSupersededError()
          : new Error(response.message),
      );
      return;
    }

    const pending = this.pendingRequests.get(response.requestId);
    if (!pending) {
      return;
    }

    this.pendingRequests.delete(response.requestId);
    pending.resolve(response.result);
  };

  private readonly handleWorkerError = (event: ErrorEvent) => {
    event.preventDefault();
    this.handleFatalError(
      new Error(event.message || "The Bicep compiler worker crashed."),
    );
  };

  private readonly handleMessageError = () => {
    this.handleFatalError(
      new Error("The Bicep compiler worker returned an invalid message."),
    );
  };

  private handleFatalError(error: Error) {
    this.initialization?.reject(error);
    this.initialization = undefined;
    this.rejectPendingRequests(error);
    this.disposeWorker();

    if (
      !this.disposed &&
      this.initialized &&
      this.automaticRestartCount < maximumAutomaticWorkerRestarts
    ) {
      ++this.automaticRestartCount;
      this.ready = this.startWorker(interopInitializationTimeoutMs);
      void this.ready.catch(() => {
        // The next compiler request surfaces the restart failure.
      });
      return;
    }

    const failedReady = Promise.reject<void>(error);
    void failedReady.catch(() => {
      // Compiler requests surface the fatal worker error.
    });
    this.ready = failedReady;
  }

  private rejectPendingRequests(error: Error) {
    for (const request of this.pendingRequests.values()) {
      request.reject(error);
    }

    this.pendingRequests.clear();
  }

  private postMessage(message: CompilerRequest) {
    if (this.disposed) {
      throw new Error("The Bicep compiler worker is disposed.");
    }

    if (!this.worker) {
      throw new Error("The Bicep compiler worker is not available.");
    }

    this.worker.postMessage(message);
  }

  private disposeWorker(worker = this.worker) {
    if (!worker) {
      return;
    }

    worker.removeEventListener("message", this.handleMessage);
    worker.removeEventListener("error", this.handleWorkerError);
    worker.removeEventListener("messageerror", this.handleMessageError);
    worker.terminate();

    if (this.worker === worker) {
      this.worker = undefined;
    }
  }
}
