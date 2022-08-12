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

const insights = new ApplicationInsights({
  config: {
    instrumentationKey: aiKey,
  }
});

insights.loadAppInsights();
insights.trackPageView();

ReactDOM.render(
  <Container className="d-flex vh-100">
    <Row className="m-auto align-self-center">
      <Spinner animation="border" variant="light" />
    </Row>
  </Container>,
  document.getElementById('root')
);

initializeInterop(self)
  .then(() => ReactDOM.render(
    <div className="app-container">
      <Playground insights={insights} />
    </div>,
    document.getElementById('root')
  ));
