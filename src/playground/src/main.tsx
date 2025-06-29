// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createRoot } from 'react-dom/client'
import { Container, Row, Spinner } from 'react-bootstrap';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import 'bootstrap/dist/css/bootstrap.min.css';
import { aiKey } from '../package.json';
import './index.css';
import { initializeInterop } from './utils/interop';
import { App } from './App';
import { getColorMode } from './utils/colorModes';

const updateTheme = () => document.documentElement.setAttribute('data-bs-theme', getColorMode());

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', updateTheme);
window.addEventListener('DOMContentLoaded', updateTheme);

const root = createRoot(document.getElementById('root')!);
root.render(
  // Loading spinner while we initialize Blazor
  <Container className="d-flex vh-100">
    <Row className="m-auto align-self-center">
      <Spinner animation="border" variant="light" />
    </Row>
  </Container>);

const insights = new ApplicationInsights({
  config: {
    instrumentationKey: aiKey,
  }
});

insights.loadAppInsights();
insights.trackPageView();

initializeInterop(window).then(interop => {
  root.render(
    <div className="app-container">
      <App insights={insights} interop={interop} />
    </div>);
});