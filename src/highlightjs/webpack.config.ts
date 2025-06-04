// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from 'path';
import { Configuration } from 'webpack';

const configIife: Configuration = {
  entry: {
    "usage": './src/usage.ts',
  },
  output: {
    path: path.resolve(__dirname, 'out'),
    globalObject: 'self',
    filename: 'bicep.min.js',
  },
  resolve: {
    extensions: [".ts", ".tsx", ".js"],
  },
  module: {
    rules: [{
      test: /\.tsx?$/,
      use: 'ts-loader',
      exclude: /node_modules/,
    }]
  },
  externals: {
    'highlight.js': 'hljs',
  },
  optimization: {
    minimize: true,
  },
};

const configEsm: Configuration = {
  entry: {
    "bicep": './src/bicep.ts',
  },
  output: {
    path: path.resolve(__dirname, 'out'),
    globalObject: 'self',
    filename: 'bicep.es.min.js',
    library: {
      type: 'module',
    }
  },
  resolve: {
    extensions: [".ts", ".tsx", ".js"],
  },
  module: {
    rules: [{
      test: /\.tsx?$/,
      use: 'ts-loader',
      exclude: /node_modules/,
    }],
  },
  optimization: {
    minimize: true,
  },
  experiments: {
    outputModule: true,
  }
};

module.exports = [configIife, configEsm];