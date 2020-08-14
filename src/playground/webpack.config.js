const CopyPlugin = require('copy-webpack-plugin');
const path = require('path');

module.exports = {
  entry: {
    "main": './index.js',
    // monaco workers
    "editor.worker": 'monaco-editor/esm/vs/editor/editor.worker.js',
    "json.worker": 'monaco-editor/esm/vs/language/json/json.worker',
  },
  output: {
    globalObject: 'self',
    filename: '[name].bundle.js',
    path: path.resolve(__dirname, 'dist')
  },
  module: {
    rules: [{
      test: /\.css$/,
      use: ['style-loader', 'css-loader']
    }, {
      test: /\.ttf$/,
      use: ['file-loader']
    }, {
      test: /\.bicep$/,
      use: ['raw-loader']
    }]
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: '../Bicep.Wasm/bin/Release/netstandard2.1/wwwroot/_framework', to: './_framework/' },
        { from: './index.html' },
        { from: './favicon.ico' }
      ],
    }),
  ],
  devServer: {
    contentBase: path.join(__dirname, 'dist'),
    compress: true,
    open: true,
    port: 9000
  }
};