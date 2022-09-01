# Syntax Highlighting Libraries

We have Bicep support for a number of different highlighting libraries available. This document covers what is currently available.

## Textmate
Bicep has a Textmate grammar available at [bicep.tmlanguage](https://github.com/Azure/bicep/blob/main/src/textmate/bicep.tmlanguage).

### Known uses
* **Bicep VSCode extension**: Used as a fallback grammar for when semantic token colorization is not ready, or is disabled. This is configured [here](https://github.com/Azure/bicep/blob/6d937220a57ae0490bcfd0e198c2dbafa32e7f64/src/vscode-bicep/package.json#L131-L135).
* **Bicep Visual Studio extension**: Used as a fallback grammar for when the Bicep Visual Studio extension is not installed or when semantic token colorization is not ready.
* **GitHub Linguist**: Used to provide colorization for `.bicep` files and Markdown snippets posted on GitHub with ` ```bicep` code blocks. The Bicep repo is configured as a [git submodule](https://github.com/github/linguist/blob/36d6903dddc9f0e9008dacd5dc8c52905eed4e5d/.gitmodules#L332-L334), so changes to the `.tmlanguage` file in this repo should be automatically picked up when a new release of Linguist is deployed.

### Notes
To recompile it, run:
```sh
cd src/textmate
npm ci
npm run build
```

To run tests:
```sh
cd src/textmate
npm ci
npm test
```

Previewing baselines (launch an HTTP server, which can be opened in a browser to view the various HTML files):
```sh
npx http-server ./src/textmate/test/baselines
```

## Highlight.js
Bicep has a [highlight.js](https://highlightjs.org/) plugin available at [bicep.ts](https://github.com/Azure/bicep/blob/main/src/highlightjs/src/bicep.ts).

### Known uses
* **MS Docs**: It's used to provide colorization for Bicep samples in [MS Docs](https://docs.microsoft.com/). This site is using a [fork](https://github.com/DuncanmaMSFT/highlight.js) of Highlight.js, so changes to the grammar must be compiled and submitted to [the fork repo](https://github.com/DuncanmaMSFT/highlight.js/blob/stable/src/languages/bicep.js) for integration.

### Notes
To build it, run:
```sh
cd src/highlightjs
npm ci
npm run build
```

To run tests:
```sh
cd src/highlightjs
npm ci
npm test
```

Previewing baselines (launch an HTTP server, which can be opened in a browser to view the various HTML files):
```sh
npx http-server ./src/highlightjs/test/baselines
```

## Monarch
Bicep has a [Monarch](https://microsoft.github.io/monaco-editor/monarch.html) plugin available at [bicep.ts](https://github.com/Azure/bicep/blob/main/src/monarch/src/bicep.ts).

### Known uses
* **Monaco**: Bundled with Monaco - contributed [here](https://github.com/microsoft/monaco-editor/tree/main/src/basic-languages/bicep).

* **Azure DevOps**: Used to provide colorization for Bicep samples in [Azure DevOps](https://azure.microsoft.com/en-us/services/devops/).

### Notes
To build it, run:
```sh
cd src/monarch
npm ci
npm run build
```

To run tests:
```sh
cd src/monarch
npm ci
npm test
```

Previewing baselines (launch an HTTP server, which can be opened in a browser to view the various HTML files):
```sh
npx http-server ./src/monarch/test/baselines
```

## Prism.js
Bicep has a community-contributed [Prism.JS](https://prismjs.com/) highlighter available [here](https://github.com/PrismJS/prism/blob/master/components/prism-bicep.js).