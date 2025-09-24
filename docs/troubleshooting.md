# Tracing

## Enabling
To collect troubleshooting information, it can be useful to enable detailed tracing. To do this, run one of the following commands in your console window before invoking Bicep:

1. (Mac/Linux) Run the following:
   ```sh
   export BICEP_TRACING_ENABLED=true
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   $env:BICEP_TRACING_ENABLED = $true
   ```

## Controlling Verbosity

The default verbosity for tracing is "basic". In addition to [enabling tracing](#enabling), you can also set the verbosity to "full" to capture all tracing details:

1. (Mac/Linux) Run the following:
   ```sh
   export BICEP_TRACING_VERBOSITY=full
   ```
1. (Windows) Run the following in a PowerShell window:
   ```powershell
   $env:BICEP_TRACING_VERBOSITY = "full"
   ```
