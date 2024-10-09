import { PanZoomProvider } from "@vscode-bicep-ui/components";
import React from "react";
import ReactDOM from "react-dom/client";
import { App } from "./App";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <PanZoomProvider>
      <App />
    </PanZoomProvider>
  </React.StrictMode>,
);
