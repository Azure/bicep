// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from 'path';
import { Configuration } from 'webpack';

const config: Configuration = {
  entry: {
    "usage": './src/usage.ts',
  },
  output: {
    path: path.resolve(__dirname, 'out'),
    globalObject: 'self',
    filename: '[name].min.js',
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

module.exports = config;