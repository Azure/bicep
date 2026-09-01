// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// Keep Monaco's worker and language side effects explicit so the build includes
// JSON support without pulling in every language from the package-root entry point.
import editorWorker from "monaco-editor/editor/editor.worker?worker";
import "monaco-editor/language/json/monaco.contribution";
import jsonWorker from "monaco-editor/language/json/json.worker?worker";

type MonacoWorkerFactory = {
  getWorker: (_moduleId: string, label: string) => Worker;
};

declare global {
  interface Window {
    MonacoEnvironment?: MonacoWorkerFactory;
  }
}

window.MonacoEnvironment = {
  getWorker: (_moduleId: string, label: string) => {
    if (label === "json") {
      return new jsonWorker();
    }

    return new editorWorker();
  },
};
