// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IApplicationInsights } from "@microsoft/applicationinsights-web";
import { editor, MarkerSeverity } from "monaco-editor";
import React, { useEffect, useMemo, useRef, useState } from "react";
import { version as bicepVersion } from "../package.json";
import { DotnetInterop } from "./compiler/compiler-client";
import { BicepEditor, CompilationStatus } from "./components/BicepEditor";
import { CodeEditorHandle, registerBicep } from "./components/CodeEditor";
import { AppHeader } from "./components/AppHeader";
import { EditorPane, StatusTone } from "./components/EditorPane";
import { JsonEditor } from "./components/JsonEditor";
import { ProblemsPanel } from "./components/ProblemsPanel";
import { StatusBar } from "./components/StatusBar";
import { getQuickstartsLink } from "./quickstarts/quickstarts";
import { getShareLink, handleShareLink } from "./sharing/share-link";
import { setColorMode, useColorMode } from "./theme/color-mode";
import "./styles/tokens.css";
import "./styles/layout.css";
import "./styles/components.css";

const maximumDecompileFileSize = 10 * 1024 * 1024;

type Operation = {
  id: number;
  label: string;
} | null;

type ActivePane = "bicep" | "arm";

interface Props {
  insights: IApplicationInsights;
  interop: DotnetInterop;
  initialSharedContent: string | null;
}

export const App: React.FC<Props> = ({
  insights,
  interop,
  initialSharedContent,
}) => {
  const initialBicepContent = initialSharedContent ?? "";
  const [jsonContent, setJsonContent] = useState("");
  const [bicepContent, setBicepContent] = useState(initialBicepContent);
  const [initialContent, setInitialContent] = useState(initialBicepContent);
  const [contentRevision, setContentRevision] = useState(0);
  const [sourcePath, setSourcePath] = useState<string>();
  const [sampleLoadingPath, setSampleLoadingPath] = useState<string>();
  const [copied, setCopied] = useState(false);
  const [activeOperation, setActiveOperation] = useState<Operation>(null);
  const [operationError, setOperationError] = useState<string>();
  const [compilationError, setCompilationError] = useState<string>();
  const [compilationStatus, setCompilationStatus] =
    useState<CompilationStatus>("pending");
  const [compilationDurationMs, setCompilationDurationMs] = useState<number>();
  const [diagnostics, setDiagnostics] = useState<editor.IMarkerData[]>([]);
  const [problemsOpen, setProblemsOpen] = useState(false);
  const [activePane, setActivePane] = useState<ActivePane>("bicep");
  const [announcement, setAnnouncement] = useState("");
  const colorMode = useColorMode();
  const uploadInputRef = useRef<HTMLInputElement>(null);
  const sampleSelectRef = useRef<HTMLSelectElement>(null);
  const bicepEditorRef = useRef<CodeEditorHandle>(null);
  const copiedTimeoutRef = useRef<number>(undefined);
  const operationIdRef = useRef(0);
  const sampleRequestRef = useRef<AbortController>(undefined);
  const sourcePathRef = useRef(sourcePath);
  sourcePathRef.current = sourcePath;

  useEffect(() => {
    const registration = registerBicep(interop, () => sourcePathRef.current);
    return () => {
      registration.dispose();
      interop.dispose();
    };
  }, [interop]);

  useEffect(() => {
    const handleHashChange = () =>
      handleShareLink((content) => {
        if (content !== null) {
          insights.trackEvent({ name: "openSharedLink" });
          setSourcePath(undefined);
          setInitialContent(content);
          setContentRevision((revision) => revision + 1);
        }
      });

    window.addEventListener("hashchange", handleHashChange);

    if (initialSharedContent !== null) {
      insights.trackEvent({ name: "openSharedLink" });
    }

    return () => {
      window.removeEventListener("hashchange", handleHashChange);
    };
  }, [initialSharedContent, insights]);

  useEffect(() => {
    if (!compilationError && diagnostics.length === 0) {
      setProblemsOpen(false);
    }
  }, [compilationError, diagnostics]);

  useEffect(() => {
    return () => {
      sampleRequestRef.current?.abort();

      if (copiedTimeoutRef.current !== undefined) {
        window.clearTimeout(copiedTimeoutRef.current);
      }
    };
  }, []);

  async function runOperation(label: string, action: () => Promise<void>) {
    const id = ++operationIdRef.current;
    setActiveOperation({ id, label });
    setOperationError(undefined);

    try {
      await action();
    } catch (error) {
      if (error instanceof DOMException && error.name === "AbortError") {
        return;
      }

      if (operationIdRef.current === id) {
        setOperationError(
          error instanceof Error ? error.message : `${label} failed.`,
        );
      }
    } finally {
      setActiveOperation((operation) =>
        operation?.id === id ? null : operation,
      );
    }
  }

  async function loadExample(filePath: string, focusSampleSelect: boolean) {
    sampleRequestRef.current?.abort();
    const controller = new AbortController();
    sampleRequestRef.current = controller;
    setOperationError(undefined);
    setSampleLoadingPath(filePath);

    try {
      const response = await fetch(getQuickstartsLink(filePath), {
        signal: controller.signal,
      });

      if (!response.ok) {
        throw new Error(
          `The sample template could not be loaded (${response.status} ${response.statusText}).`,
        );
      }

      const bicepText = await response.text();
      if (controller.signal.aborted) {
        return;
      }

      insights.trackEvent({ name: "loadExample" }, { path: filePath });
      setInitialContent(bicepText);
      setContentRevision((revision) => revision + 1);
      setSourcePath(filePath);
      setActivePane("bicep");
    } catch (error) {
      if (error instanceof DOMException && error.name === "AbortError") {
        return;
      }

      if (sampleRequestRef.current === controller) {
        setOperationError(
          error instanceof Error
            ? error.message
            : "Loading sample template failed.",
        );
      }
    } finally {
      if (sampleRequestRef.current === controller) {
        sampleRequestRef.current = undefined;
        setSampleLoadingPath(undefined);
        if (focusSampleSelect) {
          window.requestAnimationFrame(() => sampleSelectRef.current?.focus());
        }
      }
    }
  }

  async function handleCopyLink() {
    setOperationError(undefined);

    try {
      const shareLink = getShareLink(bicepContent);
      await navigator.clipboard.writeText(shareLink);

      insights.trackEvent({ name: "copySharedLink" });
      setCopied(true);
      setAnnouncement("Share link copied to the clipboard.");

      if (copiedTimeoutRef.current !== undefined) {
        window.clearTimeout(copiedTimeoutRef.current);
      }

      copiedTimeoutRef.current = window.setTimeout(
        () => setCopied(false),
        2_000,
      );
    } catch (error) {
      setCopied(false);
      setOperationError(
        error instanceof Error
          ? `The share link could not be copied: ${error.message}`
          : "The share link could not be copied.",
      );
    }
  }

  async function handleDecompile(file: File) {
    sampleRequestRef.current?.abort();
    sampleRequestRef.current = undefined;
    setSampleLoadingPath(undefined);

    await runOperation("Decompiling ARM template", async () => {
      if (file.size > maximumDecompileFileSize) {
        throw new Error("Select an ARM template smaller than 10 MB.");
      }

      const jsonContents = await file.text();
      const { bicepFile, error } = await interop.decompile(jsonContents);

      if (bicepFile === null) {
        throw new Error(error ?? "The ARM template could not be decompiled.");
      }

      insights.trackEvent({ name: "decompileJson" });
      setSourcePath(undefined);
      setInitialContent(bicepFile);
      setContentRevision((revision) => revision + 1);
      setActivePane("bicep");
    });
  }

  async function handleCopyArmTemplate() {
    if (!armOutputIsCurrent) {
      setOperationError(
        "Compile the current Bicep source before copying ARM output.",
      );
      return;
    }

    try {
      await navigator.clipboard.writeText(jsonContent);
      setAnnouncement("ARM template copied to the clipboard.");
    } catch (error) {
      setOperationError(
        error instanceof Error
          ? `The ARM template could not be copied: ${error.message}`
          : "The ARM template could not be copied.",
      );
    }
  }

  function handleDownloadArmTemplate() {
    if (!armOutputIsCurrent) {
      setOperationError(
        "Compile the current Bicep source before downloading ARM output.",
      );
      return;
    }

    const objectUrl = URL.createObjectURL(
      new Blob([jsonContent], { type: "application/json" }),
    );
    const link = document.createElement("a");
    link.href = objectUrl;
    link.download = "main.json";
    link.click();
    URL.revokeObjectURL(objectUrl);
    setAnnouncement("ARM template download started.");
  }

  function handleDiagnosticsChange(nextDiagnostics: editor.IMarkerData[]) {
    setDiagnostics(nextDiagnostics);
    if (nextDiagnostics.length > 0) {
      setProblemsOpen(true);
    }
  }

  function handleCompilationError(message: string | undefined) {
    setCompilationError(message);
    if (message) {
      setProblemsOpen(true);
    }
  }

  function focusDiagnostic(diagnostic: editor.IMarkerData) {
    setActivePane("bicep");
    window.requestAnimationFrame(() =>
      bicepEditorRef.current?.focusAt(
        diagnostic.startLineNumber,
        diagnostic.startColumn,
      ),
    );
  }

  function closeProblems() {
    setProblemsOpen(false);
    window.requestAnimationFrame(() =>
      document.getElementById("compilation-status-summary")?.focus(),
    );
  }

  function handlePaneTabKeyDown(event: React.KeyboardEvent<HTMLButtonElement>) {
    let nextPane: ActivePane | undefined;
    switch (event.key) {
      case "ArrowLeft":
      case "ArrowUp":
      case "Home":
        nextPane = "bicep";
        break;
      case "ArrowRight":
      case "ArrowDown":
      case "End":
        nextPane = "arm";
        break;
      default:
        return;
    }

    event.preventDefault();
    setActivePane(nextPane);
    window.requestAnimationFrame(() =>
      document.getElementById(`${nextPane}-tab`)?.focus(),
    );
  }

  const errorCount = useMemo(
    () =>
      diagnostics.filter(
        (diagnostic) => diagnostic.severity === MarkerSeverity.Error,
      ).length,
    [diagnostics],
  );
  const warningCount = useMemo(
    () =>
      diagnostics.filter(
        (diagnostic) => diagnostic.severity === MarkerSeverity.Warning,
      ).length,
    [diagnostics],
  );
  const hasProblems = diagnostics.length > 0 || compilationError !== undefined;
  const armOutputIsCurrent =
    compilationStatus === "upToDate" && jsonContent.length > 0;
  const sourceName = sourcePath?.split("/").pop() ?? "main.bicep";
  const sourceSubtitle = sourcePath ?? `Untitled / ${sourceName}`;
  const armPaneStatus = getArmPaneStatus(compilationStatus);

  return (
    <div className="app-shell">
      <AppHeader
        activeOperation={activeOperation?.label}
        colorMode={colorMode}
        copied={copied}
        sampleSelectRef={sampleSelectRef}
        selectedSample={sampleLoadingPath ?? sourcePath}
        uploadInputRef={uploadInputRef}
        onCopyLink={() => void handleCopyLink()}
        onDecompile={(file) => void handleDecompile(file)}
        onSampleChange={(path) => void loadExample(path, true)}
        onToggleColorMode={() =>
          setColorMode(colorMode === "dark" ? "light" : "dark")
        }
      />

      <div className="mobile-tabs" role="tablist" aria-label="Editor panes">
        <button
          id="bicep-tab"
          className="mobile-tab"
          type="button"
          role="tab"
          aria-selected={activePane === "bicep"}
          aria-controls="bicep-pane"
          tabIndex={activePane === "bicep" ? 0 : -1}
          onClick={() => setActivePane("bicep")}
          onKeyDown={handlePaneTabKeyDown}
        >
          Bicep
        </button>
        <button
          id="arm-tab"
          className="mobile-tab"
          type="button"
          role="tab"
          aria-selected={activePane === "arm"}
          aria-controls="arm-pane"
          tabIndex={activePane === "arm" ? 0 : -1}
          onClick={() => setActivePane("arm")}
          onKeyDown={handlePaneTabKeyDown}
        >
          ARM template
        </button>
      </div>

      <main
        className="workspace"
        data-active-pane={activePane}
        data-problems-open={problemsOpen && hasProblems}
        aria-label="Bicep compilation workspace"
      >
        <EditorPane
          id="bicep-pane"
          labelId="bicep-pane-title"
          pane="bicep"
          title="Bicep"
          subtitle={sourceSubtitle}
          ariaBusy={sampleLoadingPath !== undefined}
          actions={
            sourcePath ? (
              <button
                type="button"
                className="pane-action"
                disabled={activeOperation !== null}
                aria-label="Reload selected sample"
                title="Restore Bicep file from selected sample"
                onClick={() => void loadExample(sourcePath, false)}
              >
                <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M19 12a7 7 0 0 1-11.8 5.1L5 15m0 4v-4h4M5 12A7 7 0 0 1 16.8 6.9L19 9m0-4v4h-4"
                    stroke="currentColor"
                    strokeWidth="1.8"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>
            ) : undefined
          }
        >
          <BicepEditor
            ref={bicepEditorRef}
            interop={interop}
            onBicepChange={setBicepContent}
            onJsonChange={setJsonContent}
            onDiagnosticsChange={handleDiagnosticsChange}
            onCompilationError={handleCompilationError}
            onCompilationDurationChange={setCompilationDurationMs}
            onCompilationStatusChange={setCompilationStatus}
            initialContent={initialContent}
            contentRevision={contentRevision}
            sourcePath={sourcePath}
          />
        </EditorPane>

        <EditorPane
          id="arm-pane"
          labelId="arm-pane-title"
          pane="arm"
          title="ARM template"
          subtitle="Generated JSON"
          status={armPaneStatus.label}
          statusTone={armPaneStatus.tone}
          isStale={compilationStatus !== "upToDate"}
          ariaBusy={compilationStatus === "compiling"}
          busyLabel={
            compilationStatus === "compiling"
              ? "Compiling ARM template..."
              : undefined
          }
          actions={
            <>
              <button
                type="button"
                className="pane-action"
                disabled={!armOutputIsCurrent}
                aria-label="Copy ARM template"
                title={
                  armOutputIsCurrent
                    ? "Copy ARM template"
                    : "Compile the current source before copying"
                }
                onClick={() => void handleCopyArmTemplate()}
              >
                <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
                  <rect
                    x="8"
                    y="8"
                    width="11"
                    height="11"
                    rx="2"
                    stroke="currentColor"
                    strokeWidth="1.8"
                  />
                  <path
                    d="M16 8V6a2 2 0 0 0-2-2H6a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h2"
                    stroke="currentColor"
                    strokeWidth="1.8"
                  />
                </svg>
              </button>
              <button
                type="button"
                className="pane-action"
                disabled={!armOutputIsCurrent}
                aria-label="Download ARM template"
                title={
                  armOutputIsCurrent
                    ? "Download ARM template"
                    : "Compile the current source before downloading"
                }
                onClick={handleDownloadArmTemplate}
              >
                <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
                  <path
                    d="M12 4v11m0 0 4-4m-4 4-4-4M5 19h14"
                    stroke="currentColor"
                    strokeWidth="1.8"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>
            </>
          }
        >
          <JsonEditor content={jsonContent} />
        </EditorPane>

        {activeOperation && (
          <div className="operation-overlay" role="status" aria-live="polite">
            <span className="spinner" aria-hidden="true" />
            <span>{activeOperation.label}...</span>
          </div>
        )}
      </main>

      {problemsOpen && hasProblems && (
        <ProblemsPanel
          compilationError={compilationError}
          diagnostics={diagnostics}
          sourceName={sourceName}
          onClose={closeProblems}
          onSelect={focusDiagnostic}
        />
      )}

      <StatusBar
        compilationStatus={compilationStatus}
        durationMs={compilationDurationMs}
        errorCount={errorCount}
        hasProblems={hasProblems}
        problemsOpen={problemsOpen}
        version={bicepVersion}
        warningCount={warningCount}
        onToggleProblems={() => setProblemsOpen((open) => !open)}
      />

      {operationError && (
        <div
          className="playground-alert"
          role="alert"
          aria-label="Playground error"
        >
          <span>{operationError}</span>
          <button
            type="button"
            className="pane-action"
            aria-label="Dismiss error"
            onClick={() => setOperationError(undefined)}
          >
            <svg aria-hidden="true" viewBox="0 0 24 24" fill="none">
              <path
                d="m7 7 10 10M17 7 7 17"
                stroke="currentColor"
                strokeWidth="1.8"
                strokeLinecap="round"
              />
            </svg>
          </button>
        </div>
      )}

      <div className="visually-hidden" aria-live="polite">
        {announcement}
      </div>
    </div>
  );
};

function getArmPaneStatus(status: CompilationStatus): {
  label: string;
  tone: StatusTone;
} {
  switch (status) {
    case "compiling":
      return { label: "Compiling", tone: "neutral" };
    case "upToDate":
      return { label: "Up to date", tone: "success" };
    case "failed":
    case "pending":
      return { label: "Out of date", tone: "warning" };
  }
}
