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
        exclude: [/node_modules/, /panes\/deploy\/app/, /visualizer\/app/, /test/],
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

const visualizerConfig: webpack.Configuration = {
  target: "web",
  entry: "./src/visualizer/app/components/index.tsx",
  devtool: "source-map",
  output: {
    filename: "visualizer.js",
    path: outputPath,
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
          target: "es2019",
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

module.exports = (_env: unknown, argv: { mode: string }) => {
  if (argv.mode === "development") {
    // "cheap-module-source-map" is almost 2x faster than "source-map",
    // while it provides decent source map quality.
    // Note that any "eval" options cannot be used because it will be blocked by
    // the content security policy that we set in /visualizer/view.ts.
    const developmentDevtool = "cheap-module-source-map";
    visualizerConfig.devtool = developmentDevtool;

    // I don't notice any difference in F5 time when using the cheap version, but using it
    // causes many problems recognizing breakpoints in some code, especially tests, so don't use for
    // the main extension code.
    //extensionConfig.devtool = developmentDevtool;
  }

  return [extensionConfig, visualizerConfig];
};
