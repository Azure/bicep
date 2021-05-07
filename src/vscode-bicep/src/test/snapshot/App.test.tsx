// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// eslint-disable-next-line @typescript-eslint/no-explicit-any
(window as any).acquireVsCodeApi = () => ({
  postMessage: jest.fn(),
  setState: jest.fn(),
  getState: jest.fn(),
});

import renderer from "react-test-renderer";
import "jest-styled-components";

import { App } from "../../visualizer/app/components/App";

describe("component App", () => {
  it("should render", () => {
    const tree = renderer.create(<App />).toJSON();

    // eslint-disable-next-line jest/no-large-snapshots
    expect(tree).toMatchInlineSnapshot(`
      Array [
        .c0 {
        position: absolute;
        left: 20px;
        bottom: 20px;
        display: -webkit-box;
        display: -webkit-flex;
        display: -ms-flexbox;
        display: flex;
        -webkit-flex-direction: row;
        -ms-flex-direction: row;
        flex-direction: row;
        -webkit-align-items: center;
        -webkit-box-align: center;
        -ms-flex-align: center;
        align-items: center;
      }

      .c1 {
        width: 8px;
        height: 8px;
        background-color: green;
        border-radius: 50%;
        color: white;
        margin-top: 2px;
        margin-right: 8px;
      }

      <div
          className="c0"
        >
          <div
            className="c1"
          />
          <div>
            There is no resources or modules in the file. Nothing to render.
          </div>
        </div>,
        .c0 {
        position: absolute;
        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;
      }

      <div
          className="c0"
        />,
      ]
    `);
  });
});
