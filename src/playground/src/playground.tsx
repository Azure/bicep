// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, { useEffect, useRef, useState } from 'react';
import { Button, Container, Nav, Navbar, OverlayTrigger, Row, Spinner, Tooltip } from 'react-bootstrap';

import './playground.css';
import { JsonEditor } from './jsonEditor';
import { BicepEditor } from './bicepEditor';
import { copyShareLinkToClipboard, handleShareLink } from './utils';
import { decompile } from './lspInterop';
import { IAppInsights } from '@microsoft/applicationinsights-common';

interface Props {
  insights: IAppInsights,
}

export const Playground : React.FC<Props> = (props) => {
  const { insights } = props;
  const [jsonContent, setJsonContent] = useState('');
  const [bicepContent, setBicepContent] = useState('');
  const [initialContent, setInitialContent] = useState('');
  const [copied, setCopied] = useState(false);
  const [loading, setLoading] = useState(false);
  const uploadInputRef = useRef<HTMLInputElement>();

  async function withLoader(action: () => Promise<void>) {
    try {
      setLoading(true);
      await action();
    } finally {
      setLoading(false);
    }
  }

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
        setInitialContent('')
      }
    });
  }, []);

  const handlCopyClick = () => {
    insights.trackEvent({ name: 'copySharedLink' });
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
    copyShareLinkToClipboard(bicepContent);
  }

  const handleDecompileClick = (file: File) => {
    const reader = new FileReader();
    reader.onload = async (e) => {
      withLoader(async () => {
        try {
          insights.trackEvent({ name: 'decompileJson' });
          const jsonContents = e.target.result.toString();
          const bicepContents = decompile(jsonContents);
          setInitialContent(bicepContents);
        } catch (err) {
          alert(err);
        }
      });
    };

    reader.readAsText(file);
  }

  const createTooltip = (text: string) => (
    <Tooltip id="button-tooltip">
      {text}
    </Tooltip>
  );

  return <>
    <input type="file" ref={uploadInputRef} style={{ display: 'none' }} onChange={e => handleDecompileClick(e.currentTarget.files[0])} accept="application/json" multiple={false} />
    <Navbar bg="dark" variant="dark">
      <Navbar.Brand>Bicep Playground</Navbar.Brand>
      <Nav className="ms-auto">
        <OverlayTrigger placement="bottom" overlay={createTooltip('Copy a shareable link to clipboard')}>
          <Button size="sm" variant="primary" className="mx-1" onClick={handlCopyClick}>{copied ? 'Copied' : 'Copy Link'}</Button>
        </OverlayTrigger>
        <OverlayTrigger placement="bottom" overlay={createTooltip('Upload an ARM template JSON file to decompile to Bicep')}>
          <Button size="sm" variant="primary" className="mx-1" onClick={() => uploadInputRef.current.click()}>Decompile</Button>
        </OverlayTrigger>
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
          <BicepEditor onBicepChange={setBicepContent} onJsonChange={setJsonContent} initialCode={initialContent} />
        </div>
        <div className="playground-editorpane">
          <JsonEditor content={jsonContent} />
        </div>
      </> }
    </div>
  </>
};
