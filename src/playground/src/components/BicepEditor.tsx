// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor } from 'monaco-editor';
import React from 'react';
import { DotnetInterop } from '../utils/interop';
import { CodeEditor } from './CodeEditor';

interface Props {
  interop: DotnetInterop,
  initialContent: string,
  onBicepChange: (bicepContent: string) => void,
  onJsonChange: (jsonContent: string) => void,
}

const editorOptions: editor.IStandaloneEditorConstructionOptions = {
  language: 'bicep',
  scrollBeyondLastLine: false,
  automaticLayout: true,
  minimap: {
    enabled: false,
  },
  insertSpaces: true,
  tabSize: 2,
  suggestSelection: 'first',
  suggest: {
    snippetsPreventQuickSuggestions: false,
    showWords: false,
  },
  'semanticHighlighting.enabled': true,
};

export const BicepEditor : React.FC<Props> = (props) => {
  const { interop, initialContent, onBicepChange, onJsonChange } = props;

  async function handleContentChange(model: editor.ITextModel, content: string) {
    const { template, diagnostics } = await interop.compileAndEmitDiagnostics(content);
    editor.setModelMarkers(model, 'default', diagnostics);
    onBicepChange(content);
    onJsonChange(template);
  }

  return (
    <CodeEditor 
      options={editorOptions} 
      initialContent={initialContent}
      onContentChange={handleContentChange} />
  )
};