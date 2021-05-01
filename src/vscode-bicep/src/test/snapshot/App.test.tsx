// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// eslint-disable-next-line @typescript-eslint/no-explicit-any
(window as any).acquireVsCodeApi = () => ({
  postMessage: jest.fn(),
  setState: jest.fn(),
  getState: jest.fn(),
});

import renderer from "react-test-renderer";

import { App } from "../../visualizer/app/components/App";

describe("component App", () => {
  it("should render", () => {
    const tree = renderer.create(<App />).toJSON();

    expect(tree).toMatchInlineSnapshot(`
      Array [
        <div
          className="sc-bdvvaa fcnrhJ"
        >
          <div
            className="sc-gsDJrp geWPka"
          />
          <div>
            There is no resources or modules in the file. Nothing to render.
          </div>
        </div>,
        <div
          className="sc-dkPtyc isBCBH"
        />,
      ]
    `);
  });
});
