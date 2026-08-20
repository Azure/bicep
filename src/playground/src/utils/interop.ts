// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { languages } from "monaco-editor";
import { getQuickstartsLink } from "./examples";
import {
  CompileResult,
  CompilerRequest,
  CompilerResponse,
  CompilerResult,
  DecompileResult,
} from "../workers/compilerProtocol";

const interopInitializationTimeoutMs = 30_000;

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
  private readonly worker = new Worker(
    new URL("../workers/compiler.worker.ts", import.meta.url),
    { type: "module" },
  );
  private readonly pendingRequests = new Map<number, PendingRequest>();
  private nextRequestId = 0;
  private initialization: Initialization | undefined;
  private semanticTokensLegend: languages.SemanticTokensLegend | undefined;
  private disposed = false;

  public constructor() {
    this.worker.addEventListener("message", this.handleMessage);
    this.worker.addEventListener("error", this.handleWorkerError);
    this.worker.addEventListener("messageerror", this.handleMessageError);
  }

  public async initialize(timeoutMs: number): Promise<void> {
    if (this.initialization) {
      throw new Error("The Bicep compiler worker is already initializing.");
    }

    await new Promise<void>((resolve, reject) => {
      const timeout = window.setTimeout(() => {
        this.initialization = undefined;
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
      await this.sendRequest<languages.SemanticTokensLegend>({
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
    this.worker.removeEventListener("message", this.handleMessage);
    this.worker.removeEventListener("error", this.handleWorkerError);
    this.worker.removeEventListener("messageerror", this.handleMessageError);
    this.worker.terminate();

    const error = new Error("The Bicep compiler worker was disposed.");
    this.initialization?.reject(error);
    this.initialization = undefined;
    this.rejectPendingRequests(error);
  }

  private sendRequest<TResult extends CompilerResult>(
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
      pending.reject(new Error(response.message));
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

    this.worker.postMessage(message);
  }
}
