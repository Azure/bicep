import React, { useEffect, useState } from 'react';
import { ButtonGroup, Dropdown, Nav, Navbar, NavLink } from 'react-bootstrap';

import './playground.css';
import { examples } from './examples';
import { JsonEditor } from './jsonEditor';
import { BicepEditor } from './bicepEditor';
import { copyShareLinkToClipboard, handleShareLink } from './utils';

let initialFile = examples['101/1vm-2nics-2subnets-1vnet'];
handleShareLink(content => initialFile = content ?? initialFile);

export const Playground : React.FC = () => {
  const [jsonContent, setJsonContent] = useState('');
  const [bicepContent, setBicepContent] = useState('');
  const [initialContent, setInitialContent] = useState(initialFile);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    window.addEventListener('hashchange', () => handleShareLink(content => {
        if (content != null) {
          setInitialContent(content);
        }
      }));
  }, []);

  const handlCopyClick = () => {
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
    copyShareLinkToClipboard(bicepContent);
  }

  const dropdownItems = Object.keys(examples).map(example => (
    <Dropdown.Item key={example} eventKey={example}>{example}</Dropdown.Item>
  ));

  return <>
    <Navbar bg="dark" variant="dark">
      <Navbar.Brand>Bicep Playground</Navbar.Brand>
      <Nav className="ml-auto">
        <NavLink onClick={handlCopyClick}>{copied ? 'Copied' : 'Copy Link'}</NavLink>
        <Dropdown as={ButtonGroup} onSelect={key => setInitialContent(examples[key])}>
          <Dropdown.Toggle as={NavLink}>Sample Template</Dropdown.Toggle>
          <Dropdown.Menu className="dropdown-menu-right">
            {dropdownItems}
          </Dropdown.Menu>
        </Dropdown>
      </Nav>
    </Navbar>
    <div className="playground-container">
      <div className="playground-editorpane">
        <BicepEditor onBicepChange={setBicepContent} onJsonChange={setJsonContent} initialCode={initialContent} />
      </div>
      <div className="playground-editorpane">
        <JsonEditor content={jsonContent} />
      </div>
    </div>
  </>
};