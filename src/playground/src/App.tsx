// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IApplicationInsights } from "@microsoft/applicationinsights-web";
import React, { useEffect, useMemo, useRef, useState } from "react";
import {
  Button,
  ButtonGroup,
  Col,
  Dropdown,
  FormControl,
  Nav,
  Navbar,
  OverlayTrigger,
  Spinner,
  Tooltip,
} from "react-bootstrap";

import "./App.css";
import { BicepEditor } from "./components/BicepEditor";
import { registerBicep } from "./components/CodeEditor";
import { JsonEditor } from "./components/JsonEditor";
import { getQuickstartsLink, quickstartsPaths } from "./utils/examples";
import { DotnetInterop } from "./utils/interop";
import { getShareLink, handleShareLink } from "./utils/utils";

const maximumDecompileFileSize = 10 * 1024 * 1024;

type Operation = {
  id: number;
  label: string;
} | null;

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
  const [sourcePath, setSourcePath] = useState<string>();
  const [copied, setCopied] = useState(false);
  const [activeOperation, setActiveOperation] = useState<Operation>(null);
  const [operationError, setOperationError] = useState<string>();
  const [compilationError, setCompilationError] = useState<string>();
  const [filterText, setFilterText] = useState("");
  const uploadInputRef = useRef<HTMLInputElement>(null);
  const copiedTimeoutRef = useRef<number>(undefined);
  const operationIdRef = useRef(0);
  const sampleRequestRef = useRef<AbortController>(undefined);

  useEffect(() => {
    const registration = registerBicep(interop);
    return () => registration.dispose();
  }, [interop]);

  useEffect(() => {
    const handleHashChange = () =>
      handleShareLink((content) => {
        if (content !== null) {
          insights.trackEvent({ name: "openSharedLink" });
          setSourcePath(undefined);
          setInitialContent(content);
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

  async function loadExample(filePath: string) {
    sampleRequestRef.current?.abort();
    const controller = new AbortController();
    sampleRequestRef.current = controller;

    await runOperation("Loading sample template", async () => {
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
      setSourcePath(filePath);
    });
  }

  async function handleCopyClick() {
    setOperationError(undefined);

    try {
      const shareLink = getShareLink(bicepContent);
      await navigator.clipboard.writeText(shareLink);

      insights.trackEvent({ name: "copySharedLink" });
      setCopied(true);

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

  async function handleDecompileClick(file: File) {
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
    });
  }

  const filteredExamples = useMemo(
    () =>
      quickstartsPaths
        .filter((path) =>
          path.toLowerCase().includes(filterText.trim().toLowerCase()),
        )
        .sort((left, right) => left.localeCompare(right)),
    [filterText],
  );

  const createTooltip = (id: string, text: string) => (
    <Tooltip id={id}>{text}</Tooltip>
  );

  const isBusy = activeOperation !== null;

  return (
    <>
      <input
        type="file"
        ref={uploadInputRef}
        className="visually-hidden"
        onChange={(event) => {
          const input = event.currentTarget;
          const file = input.files?.[0];
          input.value = "";

          if (file) {
            void handleDecompileClick(file);
          }
        }}
        accept="application/json,.json"
      />
      <Navbar bg="dark" variant="dark">
        <Navbar.Brand>Bicep Playground</Navbar.Brand>
        <Nav className="ms-auto">
          <OverlayTrigger
            placement="bottom"
            overlay={createTooltip(
              "copy-link-tooltip",
              "Copy a shareable link to clipboard",
            )}
          >
            <Button
              size="sm"
              variant="primary"
              className="mx-1"
              onClick={() => void handleCopyClick()}
            >
              {copied ? "Copied" : "Copy Link"}
            </Button>
          </OverlayTrigger>
          <OverlayTrigger
            placement="bottom"
            overlay={createTooltip(
              "decompile-tooltip",
              "Upload an ARM template JSON file to decompile to Bicep",
            )}
          >
            <Button
              size="sm"
              variant="primary"
              className="mx-1"
              disabled={isBusy}
              onClick={() => uploadInputRef.current?.click()}
            >
              Decompile
            </Button>
          </OverlayTrigger>
          <Dropdown
            as={ButtonGroup}
            onSelect={(key) => {
              if (key) {
                void loadExample(key);
              }
            }}
            onToggle={() => setFilterText("")}
          >
            <OverlayTrigger
              placement="bottom"
              overlay={createTooltip(
                "sample-template-tooltip",
                "Select an Azure Quickstarts sample file",
              )}
            >
              <Dropdown.Toggle
                as={Button}
                size="sm"
                variant="primary"
                className="mx-1"
                disabled={isBusy}
              >
                Sample Template
              </Dropdown.Toggle>
            </OverlayTrigger>
            <Dropdown.Menu align="end">
              <Col>
                <FormControl
                  autoFocus
                  aria-label="Filter sample templates"
                  placeholder="Type to filter..."
                  onChange={(event) => setFilterText(event.target.value)}
                  value={filterText}
                />
              </Col>
              {filteredExamples.map((path) => (
                <Dropdown.Item key={path} eventKey={path} active={false}>
                  {path}
                </Dropdown.Item>
              ))}
            </Dropdown.Menu>
          </Dropdown>
        </Nav>
      </Navbar>
      <main className="playground-container">
        <div className="playground-editorpane">
          <BicepEditor
            interop={interop}
            onBicepChange={setBicepContent}
            onJsonChange={setJsonContent}
            onCompilationError={setCompilationError}
            initialContent={initialContent}
            sourcePath={sourcePath}
          />
        </div>
        <div className="playground-editorpane">
          <JsonEditor content={jsonContent} />
        </div>
        {activeOperation && (
          <div className="operation-overlay" role="status" aria-live="polite">
            <Spinner animation="border" variant="light" aria-hidden="true" />
            <span>{activeOperation.label}...</span>
          </div>
        )}
      </main>
      {(operationError || compilationError) && (
        <div className="playground-error" role="alert">
          <span>{operationError ?? compilationError}</span>
          <button
            type="button"
            className="btn-close"
            aria-label="Dismiss error"
            onClick={() => {
              setOperationError(undefined);
              setCompilationError(undefined);
            }}
          />
        </div>
      )}
      <div className="visually-hidden" aria-live="polite">
        {copied ? "Share link copied to the clipboard." : ""}
      </div>
    </>
  );
};
