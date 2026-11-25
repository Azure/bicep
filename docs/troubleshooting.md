# Tracing

## Tracing the CLI

These instructions apply to the Bicep CLI, whether invoked directly with `bicep` or through Azure CLI commands that invoke Bicep, such as `az bicep build`. For the VS Code extension, see [Tracing the VS Code extension](#tracing-the-vs-code-extension).

### Enabling

To collect troubleshooting information, it can be useful to enable detailed tracing. To do this, run one of the following commands in your console window before invoking Bicep or Azure CLI from the same window:

1. (Mac/Linux) Run the following:
   ```sh
   export BICEP_TRACING_ENABLED=true
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   $env:BICEP_TRACING_ENABLED = $true
   ```

### Controlling Verbosity

The default verbosity for tracing is "basic". In addition to [enabling tracing](#enabling), you can also set the verbosity to "full" to capture all tracing details:

1. (Mac/Linux) Run the following:
   ```sh
   export BICEP_TRACING_VERBOSITY=full
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   $env:BICEP_TRACING_VERBOSITY = "full"
   ```

## Tracing the VS Code extension

### 1. Enable compiler and Azure SDK tracing

Fully quit all instances of VS Code, then open a terminal and run:

**macOS/Linux**
```sh
export BICEP_TRACING_ENABLED=true
export BICEP_TRACING_VERBOSITY=full
code .
```

**Windows PowerShell**
```powershell
$env:BICEP_TRACING_ENABLED = "true"
$env:BICEP_TRACING_VERBOSITY = "full"
code .
```

Launch VS Code from this terminal so the Bicep extension inherits these environment variables. If `code` is not available on macOS, run **Shell Command: Install 'code' command in PATH** from the VS Code Command Palette before quitting VS Code.

### 2. Enable language-server message tracing

1. Open **Settings**: **Ctrl+,** on Windows/Linux or **Cmd+,** on macOS.
1. Select the **User** tab.
1. Search for **Bicep Trace Server**.
1. Set **Bicep: Trace: Server** to **Verbose**.
1. Open a Bicep file and reproduce the issue.

### 3. Collect the logs

Open **View > Output**. Use the channel dropdown to locate and copy the **Bicep** logs and Bicep language-server trace output.

Review logs before sharing: they may contain source code, file paths, configuration values, and HTTP details.

### 4. Disable tracing afterward

1. Return to **Settings** and search for **Bicep Trace Server**.
1. Change the dropdown back to **Off**.
1. Fully quit all instances of VS Code and reopen it normally, not from the terminal where tracing was enabled, to stop compiler and Azure SDK tracing too.

If you launch VS Code from the same terminal again, first remove the environment variables:

**macOS/Linux**
```sh
unset BICEP_TRACING_ENABLED BICEP_TRACING_VERBOSITY
```

**Windows PowerShell**
```powershell
Remove-Item Env:BICEP_TRACING_ENABLED
Remove-Item Env:BICEP_TRACING_VERBOSITY
```
