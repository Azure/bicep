// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import webpack from "webpack";
import CopyPlugin from "copy-webpack-plugin";
import ForkTsCheckerWebpackPlugin from "fork-ts-checker-webpack-plugin";

const extensionConfig: webpack.Configuration = {
  target: "node",
  entry: "./src/extension.ts",
  devtool: "source-map",
  output: {
    path: path.resolve(__dirname, "out"),
    filename: "extension.js",
    libraryTarget: "commonjs2",
    devtoolModuleFilenameTemplate: "file:///[absolute-resource-path]",
  },
  externals: {
    // the vscode-module is created on-the-fly and must be excluded. Add other modules that cannot be webpack'ed, 📖 -> https://webpack.js.org/configuration/externals/
    vscode: "commonjs vscode",
    // The following are optional dependencies of vscode-azureextensionui that cannot be resolved.
    "applicationinsights-native-metrics":
      "commonjs applicationinsights-native-metrics",
    "@opentelemetry/tracing": "commonjs @opentelemetry/tracing",
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: "esbuild-loader",
        options: {
          loader: "ts",
          target: "es2016",
        },
        exclude: [/node_modules/, /visualizer\/app/, /test/],
      },
    ],
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          from: "../textmate/bicep.tmlanguage",
          to: path.join(__dirname, "syntaxes/bicep.tmlanguage"),
        },
      ],
    }) as { apply(...args: unknown[]): void },
    new ForkTsCheckerWebpackPlugin(),
  ],
  resolve: {
    extensions: [".ts", ".js"],
  },
};

const visualizerConfig: webpack.Configuration = {
  target: "web",
  entry: "./src/visualizer/app/components/index.tsx",
  devtool: "source-map",
  output: {
    filename: "visualizer.js",
    path: path.resolve(__dirname, "out"),
    devtoolModuleFilenameTemplate: "file:///[absolute-resource-path]",
  },
  externals: {
    "web-worker": "commonjs web-worker",
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        loader: "esbuild-loader",
        options: {
          loader: "tsx",
          target: "es2016",
        },
        exclude: /node_modules/,
      },
      {
        test: /\.svg$/,
        use: "svg-inline-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".js", ".ts", ".tsx"],
  },
  plugins: [
    // Since React 17 it's not necessary to do "import React from 'react';" anymore.
    // This is needed for esbuild-loader to resolve react.
    new webpack.ProvidePlugin({
      React: "react",
    }),
  ],
};

module.exports = (env: unknown, argv: { mode: string }) => {
  if (argv.mode === "development") {
    // "cheap-module-source-map" is almost 2x faster than "source-map",
    // while it provides decent source map quality.
    // Note that any "eval" options cannot be used because it will be blocked by
    // the content security policy that we set in /visualizer/view.ts.
    const developmentDevtool = "cheap-module-source-map";
    extensionConfig.devtool = developmentDevtool;
    visualizerConfig.devtool = developmentDevtool;
  }

  return [extensionConfig, visualizerConfig];
};
