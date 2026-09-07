// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Documentation;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationTemplateLoaderTests
{
    private static readonly TemplateContext Context = new();

    private static readonly SourceSpan CallerSpan = new("test", new(), new());

    [TestMethod]
    public void GetPath_ExistingRelativeTemplateName_ReturnsResolvedKey()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("root/shared/_header.md", "> Header."));
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "shared/_header.md");

        path.Should().Be(fileSet.GetUri("root/shared/_header.md").ToString());
    }

    [TestMethod]
    public void GetPath_TraversalAboveRoot_ReturnsResolvedKeyOutsideRoot()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("shared/_header.md", "> Header."));
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "../shared/_header.md");

        path.Should().Be(fileSet.GetUri("shared/_header.md").ToString());
    }

    [TestMethod]
    public void GetPath_UnresolvableTemplateName_ThrowsScriptRuntimeException()
    {
        // "CON" is a reserved device name on Windows, which IOUri.Resolve rejects with an IOException.
        if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("Reserved device name resolution failures are Windows-specific.");
            return;
        }

        var fileSet = MockFileSystemTestFileSet.Create();
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var act = () => loader.GetPath(Context, CallerSpan, "CON");

        act.Should().Throw<ScriptRuntimeException>().WithMessage("*CON*");
    }

    [TestMethod]
    public void Load_UnregisteredTemplatePath_ThrowsScriptRuntimeException()
    {
        var fileSet = MockFileSystemTestFileSet.Create();
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var act = () => loader.Load(Context, CallerSpan, "never-resolved.md");

        act.Should().Throw<ScriptRuntimeException>().WithMessage("*Unable to resolve include path*");
    }

    [TestMethod]
    public void Load_MissingIncludeFile_ThrowsScriptRuntimeException()
    {
        var fileSet = MockFileSystemTestFileSet.Create();
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "missing.md");

        var act = () => loader.Load(Context, CallerSpan, path);

        act.Should().Throw<ScriptRuntimeException>().WithMessage("*does not exist*");
    }

    [TestMethod]
    public void Load_ReadError_ThrowsScriptRuntimeException()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("root/shared/_header.md", "> Header."));
        var root = GetRootDirectoryUri(fileSet, "root");
        var throwingExplorer = new ThrowingFileExplorer(new IOException("disk error"));
        var loader = new BicepDocumentationTemplateLoader(throwingExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "shared/_header.md");

        var act = () => loader.Load(Context, CallerSpan, path);

        act.Should().Throw<ScriptRuntimeException>().WithMessage("*disk error*");
    }

    [TestMethod]
    public void Load_ExistingIncludeFile_ReturnsContents()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("root/shared/_header.md", "> Header."));
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "shared/_header.md");

        loader.Load(Context, CallerSpan, path).Should().Be("> Header.");
    }

    [TestMethod]
    public async Task LoadAsync_ExistingIncludeFile_ReturnsContents()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("root/shared/_header.md", "> Header."));
        var root = GetRootDirectoryUri(fileSet, "root");
        var loader = new BicepDocumentationTemplateLoader(fileSet.FileExplorer, root);

        var path = loader.GetPath(Context, CallerSpan, "shared/_header.md");

        (await loader.LoadAsync(Context, CallerSpan, path)).Should().Be("> Header.");
    }

    // Directory URIs (unlike file URIs) always carry a trailing slash, which IOUri.Resolve relies on to treat
    // the URI as "resolve into this directory" rather than "resolve into this file's parent directory".
    private static IOUri GetRootDirectoryUri(MockFileSystemTestFileSet fileSet, string path) =>
        fileSet.FileExplorer.GetDirectory(fileSet.GetUri(path)).Uri;
}
