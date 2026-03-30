// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import editorWorker from "monaco-editor/esm/vs/editor/editor.worker?worker";
import jsonWorker from "monaco-editor/esm/vs/language/json/json.worker?worker";

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
