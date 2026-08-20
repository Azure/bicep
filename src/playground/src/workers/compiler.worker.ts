// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  CompilerRequest,
  CompilerResponse,
  CompilerResult,
} from "./compilerProtocol";

type WorkerMethods = {
  CompileAndEmitDiagnostics(
    content: string,
    sourcePath: string | null,
  ): Promise<string>;
  Decompile(content: string): Promise<string>;
  GetSemanticTokensLegend(): string;
  GetSemanticTokens(
    content: string,
    sourcePath: string | null,
  ): Promise<string>;
};

type AssemblyExports = {
  Bicep: {
    Wasm: {
      WorkerInterop: WorkerMethods;
    };
  };
};

type DotnetRuntime = {
  getAssemblyExports(assemblyName: string): Promise<AssemblyExports>;
  getConfig(): {
    mainAssemblyName: string;
  };
  setModuleImports(moduleName: string, imports: Record<string, unknown>): void;
};

type DotnetModule = {
  dotnet: {
    create(): Promise<DotnetRuntime>;
  };
};

let workerMethods: WorkerMethods | undefined;

self.addEventListener("message", (event: MessageEvent<CompilerRequest>) => {
  void handleMessage(event.data);
});

async function handleMessage(request: CompilerRequest) {
  if (request.type === "initialize") {
    await initialize(request.frameworkUrl, request.quickstartsBaseUrl);
    return;
  }

  if (!workerMethods) {
    postResponse({
      type: "error",
      requestId: request.requestId,
      message: "The Bicep compiler worker is not initialized.",
    });
    return;
  }

  try {
    let result: CompilerResult;

    switch (request.operation) {
      case "compile":
        result = JSON.parse(
          await workerMethods.CompileAndEmitDiagnostics(
            request.content,
            request.sourcePath ?? null,
          ),
        ) as CompilerResult;
        break;
      case "decompile":
        result = JSON.parse(
          await workerMethods.Decompile(request.content),
        ) as CompilerResult;
        break;
      case "getSemanticTokensLegend":
        result = JSON.parse(
          workerMethods.GetSemanticTokensLegend(),
        ) as CompilerResult;
        break;
      case "getSemanticTokens":
        result = JSON.parse(
          await workerMethods.GetSemanticTokens(
            request.content,
            request.sourcePath ?? null,
          ),
        ) as CompilerResult;
        break;
    }

    postResponse({
      type: "result",
      requestId: request.requestId,
      result,
    });
  } catch (error) {
    postResponse({
      type: "error",
      requestId: request.requestId,
      message:
        error instanceof Error ? error.message : "The compiler request failed.",
    });
  }
}

async function initialize(frameworkUrl: string, quickstartsBaseUrl: string) {
  try {
    const { dotnet } = (await import(
      /* @vite-ignore */ frameworkUrl
    )) as DotnetModule;
    const runtime = await dotnet.create();

    runtime.setModuleImports("bicepWorker", {
      loadQuickstart: async (filePath: string) => {
        const response = await fetch(new URL(filePath, quickstartsBaseUrl));

        return response.ok ? await response.text() : null;
      },
    });

    const config = runtime.getConfig();
    const exports = await runtime.getAssemblyExports(config.mainAssemblyName);
    workerMethods = exports.Bicep.Wasm.WorkerInterop;

    postResponse({ type: "ready" });
  } catch (error) {
    postResponse({
      type: "error",
      message:
        error instanceof Error
          ? error.message
          : "The Bicep compiler worker failed to start.",
    });
  }
}

function postResponse(response: CompilerResponse) {
  self.postMessage(response);
}
