// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, languages } from "monaco-editor";
import { getQuickstartsLink } from "./examples";

const interopInitializationTimeoutMs = 30_000;

type DecompileResult = {
  bicepFile: string | null;
  error: string | null;
};

type CompileResult = {
  template: string;
  diagnostics: editor.IMarkerData[];
  error?: string;
};

export interface DotnetInterop {
  getSemanticTokensLegend(): languages.SemanticTokensLegend;
  getSemanticTokens(content: string): Promise<languages.SemanticTokens>;
  compileAndEmitDiagnostics(
    content: string,
    sourcePath?: string,
  ): Promise<CompileResult>;
  decompile(jsonContent: string): Promise<DecompileResult>;
}

interface DotnetObject {
  invokeMethod<TResult>(methodName: string, ...args: unknown[]): TResult;
  invokeMethodAsync<TResult>(
    methodName: string,
    ...args: unknown[]
  ): Promise<TResult>;
}

interface InteropHost extends Window {
  LoadQuickstartsFile?: (filePath: string) => Promise<string | null>;
  InteropInitialize?: (interop: DotnetObject) => void;
}

function getDotnetInterop(interop: DotnetObject): DotnetInterop {
  return {
    getSemanticTokensLegend: () =>
      interop.invokeMethod<languages.SemanticTokensLegend>(
        "GetSemanticTokensLegend",
      ),
    getSemanticTokens: (content) =>
      interop.invokeMethodAsync<languages.SemanticTokens>(
        "GetSemanticTokens",
        content,
      ),
    compileAndEmitDiagnostics: (content, sourcePath) =>
      interop.invokeMethodAsync<CompileResult>(
        "CompileAndEmitDiagnostics",
        content,
        sourcePath,
      ),
    decompile: (content) =>
      interop.invokeMethodAsync<DecompileResult>("Decompile", content),
  };
}

export function initializeInterop(
  self: InteropHost,
  timeoutMs = interopInitializationTimeoutMs,
) {
  return new Promise<DotnetInterop>((resolve, reject) => {
    let settled = false;
    const script = document.createElement("script");

    const cleanupFailedInitialization = () => {
      script.remove();

      if (self.InteropInitialize === completeInitialization) {
        delete self.InteropInitialize;
      }
    };

    const failInitialization = (error: Error) => {
      if (settled) {
        return;
      }

      settled = true;
      window.clearTimeout(timeout);
      cleanupFailedInitialization();
      reject(error);
    };

    const completeInitialization = (newInterop: DotnetObject) => {
      if (settled) {
        return;
      }

      settled = true;
      window.clearTimeout(timeout);
      resolve(getDotnetInterop(newInterop));
    };

    const timeout = window.setTimeout(
      () =>
        failInitialization(
          new Error(
            "The Bicep compiler took too long to initialize. Check your connection and try again.",
          ),
        ),
      timeoutMs,
    );

    self.LoadQuickstartsFile = async (filePath: string) => {
      const response = await fetch(getQuickstartsLink(filePath));

      if (!response.ok) {
        return null;
      }

      return await response.text();
    };

    self.InteropInitialize = completeInitialization;

    // this is necessary to invoke the Blazor startup code - do not remove it!
    script.src = "_framework/blazor.webassembly.js";
    script.addEventListener("error", () =>
      failInitialization(
        new Error(
          "The Bicep compiler could not be downloaded. Check your connection and try again.",
        ),
      ),
    );
    document.body.appendChild(script);
  });
}
