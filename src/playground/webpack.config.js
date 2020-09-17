const CopyPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');
const buildVersion = require('./package.json').version;
const path = require('path');

module.exports = {
  entry: {
    "main": './src/index.tsx',
  },
  output: {
    globalObject: 'self',
    filename: '[name].bundle.js',
    path: path.resolve(__dirname, 'dist')
  },
  devtool: 'source-map',
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
      ],
    }),
    new HtmlWebpackPlugin({
      title: `Bicep Playground ${buildVersion}`,
      favicon: `./src/favicon.ico`,
      template: `./src/index.html`,
    }),
    new MonacoWebpackPlugin({
      languages: ['json']
    }),
  ],
  devServer: {
    contentBase: path.join(__dirname, 'dist'),
    compress: true,
    open: true,
    port: 9000
  }
};