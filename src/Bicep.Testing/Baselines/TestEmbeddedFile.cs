// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.RegularExpressions;

namespace Bicep.Testing.Baselines;

public record TestEmbeddedFile(Assembly Assembly, string StreamPath)
{
    private readonly Lazy<BinaryData> binaryDataLazy = new(() => BinaryData.FromStream(Assembly.GetManifestResourceStream(StreamPath)!));
    private readonly Lazy<string> contentsLazy = new(() => new StreamReader(Assembly.GetManifestResourceStream(StreamPath)!).ReadToEnd());

    public string Contents => contentsLazy.Value;

    public BinaryData BinaryData => binaryDataLazy.Value;

    public string FileName => Path.GetFileName(StreamPath);

    public string RelativeSourcePath => Path.Combine("src", Assembly.GetName().Name!, StreamPath);

    public static IEnumerable<TestEmbeddedFile> LoadAll(Assembly assembly, string streamPathPrefix, Func<string, bool> shouldLoad)
    {
        var combinedPathPrefix = $"Files/{streamPathPrefix}/";

        return LoadAll(assembly, name => name.StartsWith(combinedPathPrefix, StringComparison.Ordinal) && shouldLoad(name));
    }

    public static IEnumerable<TestEmbeddedFile> LoadAll(Assembly assembly, Regex regex)
        => LoadAll(assembly, regex.IsMatch);

    public static IEnumerable<TestEmbeddedFile> LoadAll(Assembly assembly, Func<string, bool> shouldLoad)
    {
        foreach (var streamName in assembly.GetManifestResourceNames().Where(shouldLoad))
        {
            yield return new(assembly, streamName);
        }
    }

    public override string ToString() => StreamPath;
}
