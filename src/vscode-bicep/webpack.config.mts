// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type webpack from "webpack";

import path from "path";
import { fileURLToPath } from "url";
import CopyPlugin from "copy-webpack-plugin";
import TerserPlugin from "terser-webpack-plugin";

const configDirectory = path.dirname(fileURLToPath(import.meta.url));
const outputPath = path.resolve(configDirectory, "out");

const extensionConfig: webpack.Configuration = {
  target: "node",
  entry: "./src/extension.ts",
  devtool: "source-map",
  output: {
    path: outputPath,
    filename: "extension.js",
    libraryTarget: "commonjs2",
    devtoolModuleFilenameTemplate: "file:///[absolute-resource-path]",
  },
  externals: {
    // the vscode-module is created on-the-fly and must be excluded. Add other modules that cannot be webpack'ed, 📖 -> https://webpack.js.org/configuration/externals/
    vscode: "commonjs vscode",
  },
  optimization: {
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          keep_classnames: true,
          keep_fnames: true,
        },
      }),
    ],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: "esbuild-loader",
        options: {
          loader: "ts",
          target: "es2019",
        },
        exclude: [/node_modules/, /panes\/deploy\/app/, /test/],
      },
    ],
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          from: "../vscode-bicep-ui/apps/deploy-pane/dist",
          to: path.join(configDirectory, "out/deploy-pane"),
          globOptions: {
            ignore: ["**/index.html"],
          },
        },
      ],
    }),
    new CopyPlugin({
      patterns: [
        {
          from: "../vscode-bicep-ui/apps/visual-designer/dist",
          to: path.join(configDirectory, "out/visual-designer"),
          globOptions: {
            ignore: ["**/index.html"],
          },
        },
      ],
    }),
    new CopyPlugin({
      patterns: [
        {
          from: "../textmate/bicep.tmlanguage",
          to: path.join(configDirectory, "resources/language/bicep.tmlanguage"),
        },
      ],
    }),
    new CopyPlugin({
      patterns: [
        {
          from: "../textmate/language-configuration.json",
          to: path.join(configDirectory, "resources/language/language-configuration.json"),
        },
      ],
    }),
  ],
  resolve: {
    extensions: [".ts", ".js"],
    conditionNames: ["node", "import", "require"],
  },
};

export default () => {
  return [extensionConfig];
};
