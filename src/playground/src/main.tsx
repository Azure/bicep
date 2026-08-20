// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ApplicationInsights } from "@microsoft/applicationinsights-web";
import { createRoot } from "react-dom/client";
import "bootstrap/dist/css/bootstrap.min.css";
import { aiKey } from "../package.json";
import { App } from "./App";
import "./index.css";
import "./monacoEnvironment";
import { getColorMode } from "./utils/colorModes";
import { initializeInterop } from "./utils/interop";
import { configureTelemetry, getSanitizedCurrentUrl } from "./utils/telemetry";
import { handleShareLink } from "./utils/utils";

const updateTheme = () =>
  document.documentElement.setAttribute("data-bs-theme", getColorMode());

window
  .matchMedia("(prefers-color-scheme: dark)")
  .addEventListener("change", updateTheme);
window.addEventListener("DOMContentLoaded", updateTheme);

let initialSharedContent: string | null = null;
handleShareLink((content) => {
  initialSharedContent = content;
});

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("The playground root element was not found.");
}

const root = createRoot(rootElement);
const insights = configureTelemetry(
  new ApplicationInsights({
    config: {
      instrumentationKey: aiKey,
    },
  }),
);

insights.trackPageView({ uri: getSanitizedCurrentUrl() });

function renderLoading() {
  root.render(
    <main className="startup-state" role="status" aria-live="polite">
      <span className="startup-spinner" aria-hidden="true" />
      <span>Loading the Bicep compiler...</span>
    </main>,
  );
}

function renderStartupError(error: unknown) {
  const message =
    error instanceof Error
      ? error.message
      : "The Bicep compiler failed to load.";

  root.render(
    <main className="startup-state" role="alert">
      <h1>Bicep Playground could not start</h1>
      <p>{message}</p>
      <button
        type="button"
        className="btn btn-primary"
        onClick={() => window.location.reload()}
      >
        Retry
      </button>
    </main>,
  );
}

async function start() {
  renderLoading();

  try {
    const interop = await initializeInterop();
    root.render(
      <div className="app-container">
        <App
          insights={insights}
          interop={interop}
          initialSharedContent={initialSharedContent}
        />
      </div>,
    );
  } catch (error) {
    renderStartupError(error);
  }
}

void start();
