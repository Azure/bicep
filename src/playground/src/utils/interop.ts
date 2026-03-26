// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, languages } from "monaco-editor";
import { getQuickstartsLink } from "./examples";

type DecompileResult = {
  bicepFile?: string;
  error?: string;
};

type CompileResult = {
  template: string;
  diagnostics: editor.IMarkerData[];
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

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function getDotnetInterop(interop: any): DotnetInterop {
  return {
    getSemanticTokensLegend: () =>
      interop.invokeMethod("GetSemanticTokensLegend"),
    getSemanticTokens: (content) =>
      interop.invokeMethodAsync("GetSemanticTokens", content),
    compileAndEmitDiagnostics: (content, sourcePath) =>
      interop.invokeMethodAsync(
        "CompileAndEmitDiagnostics",
        content,
        sourcePath,
      ),
    decompile: (content) => interop.invokeMethodAsync("Decompile", content),
  };
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function initializeInterop(self: any) {
  return new Promise<DotnetInterop>((resolve) => {
    self["LoadQuickstartsFile"] = async (filePath: string) => {
      try {
        const response = await fetch(getQuickstartsLink(filePath));

        if (!response.ok) {
          return null;
        }

        return await response.text();
      } catch {
        return null;
      }
    };

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    self["InteropInitialize"] = (newInterop: any) => {
      resolve(getDotnetInterop(newInterop));
    };

    // this is necessary to invoke the Blazor startup code - do not remove it!
    const s = document.createElement("script");
    s.setAttribute("src", "_framework/blazor.webassembly.js");
    document.body.appendChild(s);
  });
}
