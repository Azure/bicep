# Use Bicep in other editors

Visual Studio Code and Visual Studio are the supported editors for Bicep.

The Bicep language server also works in any editor that supports the Language Server Protocol (LSP). The setups on this page come from the community.

## Requirements

- .NET 10 runtime. To check, run `dotnet --list-runtimes`.

## Get the language server

1. Download [bicep-langserver.zip](https://github.com/Azure/bicep/releases/latest/download/bicep-langserver.zip) from the latest release.
2. Unzip it to a folder. For example: `/opt/bicep-langserver`.
3. Note the full path to `Bicep.LangServer.dll` in that folder.

## Neovim

1. Install [nvim-lspconfig](https://github.com/neovim/nvim-lspconfig).
2. Add this to your `init.lua`:

   ```lua
   vim.filetype.add({ extension = { bicep = 'bicep', bicepparam = 'bicep-params' } })

   vim.lsp.config('bicep', {
     cmd = { 'dotnet', '/opt/bicep-langserver/Bicep.LangServer.dll' },
   })
   vim.lsp.enable('bicep')
   ```

3. Use the full path in `cmd`. Neovim does not expand `~` or `$HOME` here.
4. Open a `.bicep` file. Run `:checkhealth vim.lsp` to see that the server is attached.

If you use [mason.nvim](https://github.com/mason-org/mason.nvim):

1. Run `:MasonInstall bicep-lsp`. You do not need to download the zip.
2. Set `cmd = { 'bicep-lsp' }` in the config above.

## Sublime Text

1. Install the [LSP](https://packagecontrol.io/packages/LSP) package.
2. Install the [LSP-Bicep](https://github.com/sublimelsp/LSP-Bicep) package.
3. Follow the LSP-Bicep README.

## Other editors

Set your editor's LSP client to use these values:

| Setting | Value |
|---|---|
| Command | `dotnet /path/to/Bicep.LangServer.dll` |
| Transport | stdio |
| `.bicep` files | language ID `bicep` |
| `.bicepparam` files | language ID `bicep-params` |
