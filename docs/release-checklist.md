# Bicep Release Checklist

## Release Tasks
1. (**end-of-month releases only**) Update Bicep Az types:
    1. Verify the "Update Types" workflow run is green: [![Update Types](https://github.com/Azure/bicep-types-az/actions/workflows/update-types.yml/badge.svg)](https://github.com/Azure/bicep-types-az/actions/workflows/update-types.yml)
    1. Create a PR [here](https://github.com/Azure/bicep-types-az/compare/main...autogenerate), get it approved, and then merge it.
    1. Follow the "Release Process" instructions [here](https://msazure.visualstudio.com/One/_git/BicepMirror-Types-Az) to build and publish the Bicep.Types.Az NuGet package.
    1. Submit a Bicep PR to use the new Bicep.Types.Az NuGet package version.
        1. Update the version [here](https://github.com/Azure/bicep/blob/main/src/Bicep.Core/Bicep.Core.csproj) and run `dotnet restore` to update packages.lock.json files.
        1. Submit a PR. If CI tests fail, you may need to update baselines (run `./scripts/UpdateBaselines.ps1` in the Bicep repo) and push the changes.
1. Verify the latest build on the `main` branch is green: [![Build](https://github.com/Azure/bicep/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/Azure/bicep/actions/workflows/build.yml?query=branch%3Amain).
1. (**end-of-month releases only**) Submit a PR to increment the minor version number in [this file](https://github.com/Azure/bicep/blob/main/version.json) (example [here](https://github.com/Azure/bicep/pull/9698))
1. Run the Official Build for BicepMirror (see [this README](https://msazure.visualstudio.com/One/_git/BicepMirror) for instructions).
1. Push the version tag for the commit used to generate the official build.
    1. Obtain the version number from official build. This should be of format `vXX.YY.ZZ` - e.g `v0.14.85`.
        1. Look at either the official build artifacts or
        2. Look at random logs of the the build from [here](https://msazure.visualstudio.com/One/_build?definitionId=182734&_a=summary). One example might be bicep_windows -> Copy Bicep registry module tool package to output (windows_build_container)
    1. In the Bicep repo, run `git tag v<new_release_number> <commit_hash>`, where `<commit_hash>` is the git commit hash used for the official build (ex: `git tag v0.15.31 3ba6e06a8d412febd182e607d0f5fe607ec90274`).
    1. Run `git push origin v<new_release_number>` to push the tag (ex: `git push origin v0.15.31`).
1. [Create a draft release](https://github.com/Azure/bicep/releases/new) for the new tag and set release title to the tag name. Use the "Save draft" button to save the changes without publishing it.
1. Run `./scripts/CreateReleaseNotes -FromTag <previous tag> -ToTag <new tag>` in the Bicep repo, and copy the output into the GitHub release description.
1. Send a link to the draft release to the PM team, and ask them to clean up the release notes and update the draft release.
1. Run `./scripts/UploadSignedReleaseArtifacts.ps1` in the BicepMirror repo to add official artifacts to the release.
    - `-WorkingDir` can be any empty temporary directory that you create
    - `-TagName` the tag for the new release you're publishing in the format `v<new_release_number>` e.g. `v0.15.31`
    - `-BuildId` is only needed if the latest official build is NOT the official build you are trying to release
1. Validate VSCode extension and Bicep CLI manually on Windows, Mac & Linux:
    1. Download `vscode-bicep.vsix` from the draft release, and [Install it from VSIX](https://code.visualstudio.com/docs/editor/extension-marketplace#_install-from-a-vsix). Verify that you can open a Bicep file, that text is correctly colorized, and that error messages show up as expected.
    1. Download the appropriate Bicep executable for your platform (e.g. `bicep-linux-x64`). Verify you can invoke it with e.g. `bicep-linux-x64 --version`, and that it prints the expected output.
1. Verify that the draft release on GitHub has at least 28 artifacts associated with it before publishing it. (On the Bicep releases [page](https://github.com/Azure/bicep/releases) it should state that many assets. On the edit page of the draft release itself, there will be two fewer assets because the 2 source code files don't show up.)
1. Publish the release on GitHub.
1. Upload vscode-bicep.VSIX to the VS marketplace [here](https://marketplace.visualstudio.com/manage). You may need access permissions, request help in the team channel.
    1. Click on the ... for Bicep, then Update, then upload the .vsix file. The site will verify it then the version number should be updated to the right one.
1. Upload vs-bicep.VSIX to VS marketplace.
    1. ⚠️ **[READ THIS BEFORE PROCEED] Copy/paste the text from the current version of [src\vs-bicep\README.md](https://github.com/Azure/bicep/blob/main/src/vs-bicep/README.md) over the existing text in the "Overview" field for the next step of uploading vs-bicep.vsix to the VS marketplace (this can only be changed on the marketplace when publishing a new version)**
    1. Go [here](https://marketplace.visualstudio.com/manage), click on the ... for Bicep for Visual Studio, then Edit.
    1. Upload the new vs-bicep.VSIX file at the top. This should update the Version number automatically for you. Verify that it does.
    1. Scroll to the bottom and hit Save and Upload.
1. Upload NuGet packages to nuget.org by running `./scripts/UploadPackages.ps1` in the BicepMirror repo. This is an almost identical process to publishing the BicepMirror-Types-Az nuget packages so look at that previous step above. (Make sure to include CLI packages.) This can be done one of two ways:
    1. Easiest is to use the `__assets` directory created by the `UploadSignedReleaseArtifacts.ps1` script. This will be in the temporary folder you created before. (Example command: `.\scripts\UploadPackages.ps1 -PackageDirectory .\temporary\__assets\ -NuGetPath C:\NugetTool\`)
    2. You can also download all the files from the published release into a separate folder and run the script using that folder. (The script looks for files ending in *.nupkg)
1. Update homebrew:
    1. Go [here](https://github.com/Azure/homebrew-bicep/actions/workflows/update-homebrew.yml), click `Run workflow`, and wait for it to complete successfully.
    1. Create a PR [here](https://github.com/Azure/homebrew-bicep/compare/main...update-homebrew), get it approved, and then merge it.

## Post-release Tasks
1. Upload copyleft dependency source to 3rd party disclosure site. See [instructions](https://msazure.visualstudio.com/One/_wiki/wikis/Azure%20Deployments%20Team%20Wiki/369910/Bicep-release-step-Upload-copyleft-source-to-3rd-party-disclosure-site).
1. Let the PM team know that the release is completed; they may want to post a twitter/X thread on BicepLang twitter account, with the following information:
    * Highlights from Release Notes with link to full release
    * Link to the most recent community call
    * Links to any relevant new docs
