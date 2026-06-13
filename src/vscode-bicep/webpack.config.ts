// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import CopyPlugin from "copy-webpack-plugin";
import ForkTsCheckerWebpackPlugin from "fork-ts-checker-webpack-plugin";
import TerserPlugin from "terser-webpack-plugin";
import webpack from "webpack";

const outputPath = path.resolve(__dirname, "out");

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
    // The following are optional dependencies of microsoft/vscode-azext-utils that cannot be resolved.
    "applicationinsights-native-metrics": "commonjs applicationinsights-native-metrics",
    "@opentelemetry/tracing": "commonjs @opentelemetry/tracing",
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
          to: path.join(__dirname, "out/deploy-pane"),
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
          to: path.join(__dirname, "out/visual-designer"),
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
          to: path.join(__dirname, "syntaxes/bicep.tmlanguage"),
        },
      ],
    }),
    new CopyPlugin({
      patterns: [
        {
          from: "../textmate/language-configuration.json",
          to: path.join(__dirname, "syntaxes/language-configuration.json"),
        },
      ],
    }),
    new ForkTsCheckerWebpackPlugin(),
  ],
  resolve: {
    extensions: [".ts", ".js"],
    conditionNames: ["import", "require"],
  },
};

module.exports = () => {
  return [extensionConfig];
};
