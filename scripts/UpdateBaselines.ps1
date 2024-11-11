# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_parsing#passing-arguments-that-contain-quote-characters
if ($PSVersionTable.PSVersion -ge [System.Version]"7.3") {
    dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name="SetBaseLine", value="true")'
} else {
    dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name=\"SetBaseLine\", value=\"true\")'
}
