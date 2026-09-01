// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createRoot } from "react-dom/client";
import { aiKey } from "../package.json";
import { App } from "./App";
import { initializeInterop } from "./compiler/compiler-client";
import "./index.css";
import { handleShareLink } from "./sharing/share-link";
import { PlaygroundTelemetry } from "./telemetry/playground-telemetry";
import {
  getPreferredColorMode,
  setColorMode,
} from "./theme/color-mode";

const colorModeQuery = window.matchMedia("(prefers-color-scheme: dark)");
const updateColorMode = () => setColorMode(getPreferredColorMode());
colorModeQuery.addEventListener("change", updateColorMode);
updateColorMode();

let initialSharedContent: string | null = null;
handleShareLink((content) => {
  initialSharedContent = content;
});

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("The playground root element was not found.");
}

const root = createRoot(rootElement);
const pendingTelemetryEvents: Parameters<PlaygroundTelemetry["trackEvent"]>[] =
  [];
let activeTelemetry: PlaygroundTelemetry | undefined;
const insights: PlaygroundTelemetry = {
  trackEvent: (...args) => {
    if (activeTelemetry) {
      activeTelemetry.trackEvent(...args);
    } else {
      pendingTelemetryEvents.push(args);
    }
  },
};

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
        className="button primary-button"
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
    void connectTelemetry();
  } catch (error) {
    renderStartupError(error);
  }
}

async function connectTelemetry(): Promise<void> {
  try {
    const { initializeTelemetry } = await import(
      "./telemetry/application-insights"
    );
    activeTelemetry = initializeTelemetry(aiKey);
    for (const event of pendingTelemetryEvents.splice(0)) {
      activeTelemetry.trackEvent(...event);
    }
  } catch (error) {
    console.warn("Application Insights failed to initialize.", error);
  }
}

void start();
