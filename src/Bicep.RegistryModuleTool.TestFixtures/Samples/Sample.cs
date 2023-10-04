// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public class Sample
    {
        private const string SampleResourcePrefix = "Bicep.RegistryModuleTool.TestFixtures.Samples";

        private readonly IEnumerable<(string Path, string ResourceName)> resources;

        public Sample(IEnumerable<(string, string)> resources)
        {
            this.resources = resources;
        }

        public static Sample Empty { get; } = new(Enumerable.Empty<(string, string)>());

        public static Sample NewlyGenerated { get; } = LoadSample();

        public static Sample Modified { get; } = LoadSample();

        public static Sample Modified_Experimental { get; } = LoadSample();

        public static Sample Valid { get; } = LoadSample();

        public static Sample Valid_Experimental { get; } = LoadSample();

        public static Sample Invalid { get; } = LoadSample();

        public IEnumerable<(string Path, string ResourceName)> EnumerateResources() => this.resources;

        private static Sample LoadSample([CallerMemberName] string? category = null)
        {
            var prefix = $"{SampleResourcePrefix}.{category}.";
            var sampleResourceNames = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(x => x.StartsWith(prefix));

            if (!sampleResourceNames.Any())
            {
                throw new InvalidOperationException("Could not find any embeded sample files.");
            }

            var testPrefix = $"{prefix}test.";

            var resources = sampleResourceNames.Select(resourceName =>
            {
                string path = resourceName.StartsWith(testPrefix)
                    ? $"test/{resourceName[testPrefix.Length..]}"
                    : resourceName[prefix.Length..];

                return (path, resourceName);
            });

            return new(resources);
        }
    }
}
