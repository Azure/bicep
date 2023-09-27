// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React from 'react';
import ReactDOM from 'react-dom';
import { Container, Row, Spinner } from 'react-bootstrap';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import 'bootstrap/dist/css/bootstrap.min.css';
import { aiKey } from '../package.json';
import './index.css';
import { initializeInterop } from './lspInterop';
import { Playground } from './playground';
import { initServices } from 'monaco-languageclient';
import { createLanguageClient } from './client';
import { LogMessageNotification, MessageType } from 'vscode-languageserver-protocol';

ReactDOM.render(
  // Loading spinner while we initialize Blazor
  <Container className="d-flex vh-100">
    <Row className="m-auto align-self-center">
      <Spinner animation="border" variant="light" />
    </Row>
  </Container>,
  document.getElementById('root')
);

async function initialize() {
  const insights = new ApplicationInsights({
    config: {
      instrumentationKey: aiKey,
    }
  });

  insights.loadAppInsights();
  insights.trackPageView();

  await initializeInterop(self);

  await initServices();
  const client = createLanguageClient();
  client.onNotification(LogMessageNotification.type, message => {
    switch (message.type) {
      case MessageType.Error:
        console.error(message.message);
        break;
      case MessageType.Warning:
        console.warn(message.message);
        break;
      case MessageType.Info:
        console.info(message.message);
        break;
      case MessageType.Log:
        console.log(message.message);
        break;
    }
  });
  await client.start();

  ReactDOM.render(
    <div className="app-container">
      <Playground insights={insights} client={client} />
    </div>,
    document.getElementById('root')
  );
}

initialize();