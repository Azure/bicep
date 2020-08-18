const CopyPlugin = require('copy-webpack-plugin');
const path = require('path');

module.exports = {
  entry: {
    "main": './src/index.ts',
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
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
    },{
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
  resolve: {
    extensions: [ '.tsx', '.ts', '.js' ],
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        { from: '../Bicep.Wasm/bin/Release/netstandard2.1/wwwroot/_framework', to: './_framework/' },
        { from: './src/index.html' },
        { from: './src/favicon.ico' }
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