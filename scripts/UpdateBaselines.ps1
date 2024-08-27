if (($PSVersionTable.Keys -contains "PSEdition") -and ($PSVersionTable.PSEdition -ne 'Desktop')) {
    # PowerShell Core
    dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name="SetBaseLine", value="true")'
} else {
    # PowerShell Desktop
    dotnet test --filter "TestCategory=Baseline" -- 'TestRunParameters.Parameter(name=\"SetBaseLine\", value=\"true\")'
}
