// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monacoEditor from 'monaco-editor';
import React, { useRef, useState } from 'react';
import MonacoEditor from 'react-monaco-editor';
import { compileAndEmitDiagnostics, getSemanticTokens, getSemanticTokensLegend } from './lspInterop';

interface Props {
  initialCode: string,
  onBicepChange: (bicepContent: string) => void,
  onJsonChange: (jsonContent: string) => void,
}

function configureEditorForBicep(editor: monacoEditor.editor.IStandaloneCodeEditor, monaco: typeof monacoEditor) {
  monaco.languages.register({
    id: 'bicep',
    extensions: ['.bicep'],
    aliases: ['bicep'],
  });

  monaco.languages.registerDocumentSemanticTokensProvider('bicep', {
    getLegend: () => getSemanticTokensLegend(),
    provideDocumentSemanticTokens: (model) => getSemanticTokens(model.getValue()),
    releaseDocumentSemanticTokens: () => { return; }
  });

  // @ts-expect-error: Using a private method on editor
  editor._themeService._theme.getTokenStyleMetadata = (type) => {
    // see 'monaco-editor/esm/vs/editor/standalone/common/themes.js' to understand these indices
    switch (type) {
      case 'keyword':
        return { foreground: 12 };
      case 'comment':
        return { foreground: 7 };
      case 'parameter':
        return { foreground: 2 };
      case 'property':
        return { foreground: 3 };
      case 'type':
        return { foreground: 8 };
      case 'member':
        return { foreground: 6 };
      case 'string':
        return { foreground: 5 };
      case 'variable':
        return { foreground: 4 };
      case 'operator':
        return { foreground: 9 };
      case 'function':
        return { foreground: 13 };
      case 'number':
        return { foreground: 15 };
      case 'class':
      case 'enummember':
      case 'event':
      case 'modifier':
      case 'label':
      case 'typeParameter':
      case 'macro':
      case 'interface':
      case 'enum':
      case 'regexp':
      case 'struct':
      case 'namespace':
        return { foreground: 0 };
    }
  };
}

export const BicepEditor : React.FC<Props> = (props) => {
  const options: monacoEditor.editor.IStandaloneEditorConstructionOptions = {
    scrollBeyondLastLine: false,
    automaticLayout: true,
    minimap: {
      enabled: false,
    },
    'semanticHighlighting.enabled': true,
  };

  const monacoRef = useRef<MonacoEditor>();
  const [initialCode, setInitialCode] = useState(props.initialCode);
  const [bicepContent, setBicepContent] = useState(props.initialCode);

  const handleContentChange = (editor: monacoEditor.editor.IStandaloneCodeEditor, text: string) => {
    setBicepContent(text);
    const { template, diagnostics } = compileAndEmitDiagnostics(text);
    monacoEditor.editor.setModelMarkers(editor.getModel(), 'default', diagnostics);
    props.onBicepChange(text);
    props.onJsonChange(template);
  }

  const handleEditorDidMount = (editor: monacoEditor.editor.IStandaloneCodeEditor, monaco: typeof monacoEditor) => {
    configureEditorForBicep(editor, monaco);
    handleContentChange(editor, bicepContent);
  }

  if (initialCode != props.initialCode) {
    setInitialCode(props.initialCode);
    handleContentChange(monacoRef.current.editor, props.initialCode);

    // clear the selection after this completes
    setTimeout(() => {
      monacoRef.current.editor.setSelection({startColumn: 1, startLineNumber: 1, endColumn: 1, endLineNumber: 1});
      monacoRef.current.editor.setScrollPosition({scrollLeft: 0, scrollTop: 0});
    }, 0);
  }

  return <MonacoEditor
    ref={monacoRef}
    language="bicep"
    theme="vs-dark"
    value={bicepContent}
    options={options}
    onChange={text => handleContentChange(monacoRef.current.editor, text)}
    editorDidMount={handleEditorDidMount}
  />
};