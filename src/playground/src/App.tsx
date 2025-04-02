// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, { useEffect, useRef, useState } from 'react';
import { Button, ButtonGroup, Col, Container, Dropdown, FormControl, Nav, Navbar, OverlayTrigger, Row, Spinner, Tooltip } from 'react-bootstrap';

import './App.css';
import { JsonEditor } from './components/JsonEditor';
import { BicepEditor } from './components/BicepEditor';
import { getShareLink, handleShareLink } from './utils/utils';
import { quickstartsPaths, getQuickstartsLink } from './utils/examples';
import { DotnetInterop } from './utils/interop';
import { IApplicationInsights } from '@microsoft/applicationinsights-web';
import { registerBicep } from './components/CodeEditor';

interface Props {
  insights: IApplicationInsights,
  interop: DotnetInterop,
}

export const App : React.FC<Props> = (props) => {
  const { insights, interop } = props;
  const [jsonContent, setJsonContent] = useState('');
  const [bicepContent, setBicepContent] = useState('');
  const [initialContent, setInitialContent] = useState('');
  const [copied, setCopied] = useState(false);
  const [loading, setLoading] = useState(false);
  const [filterText, setFilterText] = useState('');
  const uploadInputRef = useRef<HTMLInputElement>(null);

  async function withLoader(action: () => Promise<void>) {
    try {
      setLoading(true);
      await action();
    } finally {
      setLoading(false);
    }
  }

  async function loadExample(filePath: string) {
    withLoader(async () => {
      const response = await fetch(getQuickstartsLink(filePath));

      if (!response.ok) {
        throw response.text();
      }

      insights.trackEvent({ name: 'loadExample' }, { path: filePath });
      const bicepText = await response.text();
      setInitialContent(bicepText);
    });
  }

  useEffect(() => registerBicep(interop), [interop]);

  useEffect(() => {
    window.addEventListener('hashchange', () => handleShareLink(content => {
      if (content !== null) {
        setInitialContent(content);
      }
    }));

    handleShareLink(content => {
      if (content !== null) {
        insights.trackEvent({ name: 'openSharedLink' });
        setInitialContent(content);
      } else {
        setInitialContent('');
      }
    });
  }, []);

  const handlCopyClick = () => {
    insights.trackEvent({ name: 'copySharedLink' });
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
    const shareLink = getShareLink(bicepContent);

    const shareHtml = `<a href="${shareLink}">View in Bicep Playground</a>`;
    const clipboardItem = new ClipboardItem({
        ["text/plain"]: new Blob([shareLink], { type: "text/plain" }),
        ["text/html"]: new Blob([shareHtml], { type: "text/html" }),
    });

    navigator.clipboard.write([clipboardItem]);
  }

  const handleDecompileClick = (file: File) => {
    const reader = new FileReader();
    reader.onload = async (e) => {
      withLoader(async () => {
        insights.trackEvent({ name: 'decompileJson' });
        const jsonContents = e.target!.result!.toString();
        const { bicepFile, error } = await interop.decompile(jsonContents);
        if (bicepFile) {
          setInitialContent(bicepFile);
        } else {
          alert(error);
        }
      });
    };

    reader.readAsText(file);
  }

  const filteredExamples = quickstartsPaths
    .filter(x => x.toLowerCase().indexOf(filterText.toLowerCase()) !== -1)
    .sort((a, b) => a > b ? 1 : -1);

  const dropdownItems = filteredExamples.map(path => (
    <Dropdown.Item key={path} eventKey={path} active={false}>{path}</Dropdown.Item>
  ));

  const createTooltip = (text: string) => (
    <Tooltip id="button-tooltip">
      {text}
    </Tooltip>
  );

  return <>
    <input type="file" ref={uploadInputRef} style={{ display: 'none' }} onChange={e => handleDecompileClick(e.currentTarget!.files![0])} accept="application/json" multiple={false} />
    <Navbar bg="dark" variant="dark">
      <Navbar.Brand>Bicep Playground</Navbar.Brand>
      <Nav className="ms-auto">
        <OverlayTrigger placement="bottom" overlay={createTooltip('Copy a shareable link to clipboard')}>
          <Button size="sm" variant="primary" className="mx-1" onClick={handlCopyClick}>{copied ? 'Copied' : 'Copy Link'}</Button>
        </OverlayTrigger>
        <OverlayTrigger placement="bottom" overlay={createTooltip('Upload an ARM template JSON file to decompile to Bicep')}>
          <Button size="sm" variant="primary" className="mx-1" onClick={() => uploadInputRef!.current!.click()}>Decompile</Button>
        </OverlayTrigger>
        <Dropdown as={ButtonGroup} onSelect={key => loadExample(key!)} onToggle={() => setFilterText('')}>
          <OverlayTrigger placement="bottom" overlay={createTooltip('Select an Azure Quickstarts sample file')}>
            <Dropdown.Toggle as={Button} size="sm" variant="primary" className="mx-1">Sample Template</Dropdown.Toggle>
          </OverlayTrigger>
          <Dropdown.Menu align="end">
          <Col>
            <FormControl
              autoFocus
              placeholder="Type to filter..."
              onChange={(e) => setFilterText(e.target.value)}
              value={filterText} />
          </Col>
            {dropdownItems}
          </Dropdown.Menu>
        </Dropdown>
      </Nav>
    </Navbar>
    <div className="playground-container">
      { loading ?
      <Container className="d-flex vh-100">
        <Row className="m-auto align-self-center">
          <Spinner animation="border" variant="light" />
        </Row>
      </Container> :
      <>
        <div className="playground-editorpane">
          <BicepEditor interop={interop} onBicepChange={setBicepContent} onJsonChange={setJsonContent} initialContent={initialContent} />
        </div>
        <div className="playground-editorpane">
          <JsonEditor content={jsonContent} />
        </div>
      </> }
    </div>
  </>
};
