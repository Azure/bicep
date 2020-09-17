import React from 'react';
import ReactDOM from 'react-dom';

import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';
import { initializeInterop } from './lspInterop';
import { Playground } from './playground';

ReactDOM.render(
  <div id="loader"></div>,
  document.getElementById('root')
);

initializeInterop(self)
  .then(() => ReactDOM.render(
    <div className="app-container">
      <Playground/>
    </div>,
    document.getElementById('root')
  ));