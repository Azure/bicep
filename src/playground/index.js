import './main.css';
import * as monaco from 'monaco-editor';
import exampleFile from './template.arm';

self.MonacoEnvironment = {
  getWorkerUrl: function (moduleId, label) {
    if (label === 'json') {
      return './json.worker.bundle.js';
    }
    return './editor.worker.bundle.js';
  }
}

self.BicepInitialize = interop => {
  monaco.languages.register({
      id: 'bicep',
      extensions: ['.arm'],
      aliases: ['bicep'],
  });

  monaco.languages.registerDocumentSemanticTokensProvider('bicep', {
    getLegend: () => interop.invokeMethod('GetSemanticTokensLegend'),
    provideDocumentSemanticTokens: (model, lastResultId, token) => interop.invokeMethod('GetSemanticTokens', model.getValue()),
    releaseDocumentSemanticTokens: () => { }
  });

  const editorLhs = monaco.editor.create(document.getElementById('editor_lhs'), {
      theme: 'vs-dark',
      automaticLayout: true,
      language: 'bicep',
      minimap: {
          enabled: false,
      },
      value: exampleFile,
      'semanticHighlighting.enabled': true
  });

  editorLhs._themeService._theme.getTokenStyleMetadata = (type, modifiers) => {
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

  const editorRhs = monaco.editor.create(document.getElementById('editor_rhs'), {
      theme: 'vs-dark',
      automaticLayout: true,
      language: 'json',
      minimap: {
          enabled: false,
      },
      readOnly: true,
  });

  function compileAndEmitDiagnostics() {
    const bicepModel = editorLhs.getModel();
    const { template, diagnostics } = interop.invokeMethod('CompileAndEmitDiagnostics', bicepModel.getValue());

    editorRhs.setValue(template);
    monaco.editor.setModelMarkers(bicepModel, 'default', diagnostics);
  }

  editorLhs.onDidChangeModelContent(e => compileAndEmitDiagnostics());
  compileAndEmitDiagnostics();

  document.getElementById('loader').style.display = 'none';
}