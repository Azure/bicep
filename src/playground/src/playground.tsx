import React, { useState } from 'react';
import { ButtonGroup, Dropdown, Nav, Navbar, NavLink } from 'react-bootstrap';

import './playground.css';
import { examples } from './examples';
import { JsonEditor } from './jsonEditor';
import { BicepEditor } from './bicepEditor';

export const Playground : React.FC = () => {
  const [jsonContent, setJsonContent] = useState('');
  const [exampleFile, setExampleFile] = useState(examples['101/1vm-2nics-2subnets-1vnet']);

  const dropdownItems = Object.keys(examples).map(example => (
    <Dropdown.Item key={example} eventKey={example}>{example}</Dropdown.Item>
  ));

  return <>
    <Navbar bg="dark" variant="dark">
      <Navbar.Brand>Bicep Playground</Navbar.Brand>
      <Nav className="ml-auto">
        <Dropdown as={ButtonGroup} onSelect={key => setExampleFile(examples[key])}>
          <Dropdown.Toggle as={NavLink}>Sample Template</Dropdown.Toggle>
          <Dropdown.Menu className="dropdown-menu-right">
            {dropdownItems}
          </Dropdown.Menu>
        </Dropdown>
      </Nav>
    </Navbar>
    <div className="playground-container">
      <div className="playground-editorpane">
        <BicepEditor handleJsonChange={setJsonContent} initialCode={exampleFile} />
      </div>
      <div className="playground-editorpane">
        <JsonEditor content={jsonContent} />
      </div>
    </div>
  </>
};