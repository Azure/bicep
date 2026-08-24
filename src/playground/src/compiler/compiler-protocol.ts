// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, languages } from "monaco-editor";

export type CompileResult = {
  template: string;
  diagnostics: editor.IMarkerData[];
  error: string | null;
};

export type DecompileResult = {
  bicepFile: string | null;
  error: string | null;
};

export type CompilerRequest =
  | {
      type: "initialize";
      frameworkUrl: string;
      quickstartsBaseUrl: string;
    }
  | {
      type: "request";
      requestId: number;
      operation: "compile";
      content: string;
      sourcePath?: string;
    }
  | {
      type: "request";
      requestId: number;
      operation: "decompile";
      content: string;
    }
  | {
      type: "request";
      requestId: number;
      operation: "getSemanticTokensLegend";
    }
  | {
      type: "request";
      requestId: number;
      operation: "getSemanticTokens";
      content: string;
      sourcePath?: string;
    };

export type CompilerResult =
  | CompileResult
  | DecompileResult
  | languages.SemanticTokensLegend
  | languages.SemanticTokens;

export type CompilerResponse =
  | {
      type: "ready";
    }
  | {
      type: "result";
      requestId: number;
      result: CompilerResult;
    }
  | {
      type: "error";
      requestId?: number;
      code?: "requestSuperseded";
      message: string;
    };
