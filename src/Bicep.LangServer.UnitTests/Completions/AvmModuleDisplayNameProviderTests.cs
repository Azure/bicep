// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Net.Http;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class AvmModuleDisplayNameProviderTests
    {
        [TestMethod]
        public async Task StartCache_WhenCsvFilesAreAvailable_LoadsDisplayNamesAndStatuses()
        {
            const string csvContent = """
                ModuleName,ModuleDisplayName,ModuleStatus
                bicep/avm/ptn/ai-platform/baseline,AI platform baseline,Available
                bicep/avm/res/compute/virtual-machine,Virtual machine,Deprecated
                """;

            var client = new TestAvmModuleCsvIndexHttpClient((_, _) => Task.FromResult(csvContent));
            var provider = new AvmModuleDisplayNameProvider(client);

            provider.StartCache();
            await WaitForLoadTaskAsync(provider);

            provider.TryGetModuleDisplayName("bicep/avm/ptn/ai-platform/baseline", out var patternDisplayName).Should().BeTrue();
            patternDisplayName.Should().Be("AI platform baseline");

            provider.TryGetModuleStatus("bicep/avm/ptn/ai-platform/baseline", out var patternModuleStatus).Should().BeTrue();
            patternModuleStatus.Should().Be("Available");

            provider.TryGetModuleDisplayName("avm/res/compute/virtual-machine", out var resourceDisplayName).Should().BeTrue();
            resourceDisplayName.Should().Be("Virtual machine");

            provider.TryGetModuleStatus("avm/res/compute/virtual-machine", out var resourceModuleStatus).Should().BeTrue();
            resourceModuleStatus.Should().Be("Deprecated");

            client.RequestCount.Should().Be(3);
        }

        [TestMethod]
        public async Task StartCache_WhenCsvFetchFails_CompletesAndUsesEmptyLookup()
        {
            var client = new TestAvmModuleCsvIndexHttpClient((_, _) =>
                Task.FromException<string>(new HttpRequestException("CSV unavailable")));
            var provider = new AvmModuleDisplayNameProvider(client);

            var startCache = provider.StartCache;
            startCache.Should().NotThrow();

            await WaitForLoadTaskAsync(provider);

            provider.TryGetModuleDisplayName("bicep/avm/ptn/ai-platform/baseline", out var displayName).Should().BeFalse();
            displayName.Should().BeNull();

            provider.TryGetModuleStatus("bicep/avm/ptn/ai-platform/baseline", out var moduleStatus).Should().BeFalse();
            moduleStatus.Should().BeNull();

            client.RequestCount.Should().Be(3);
        }

        private static async Task WaitForLoadTaskAsync(AvmModuleDisplayNameProvider provider)
        {
            var loadTask = GetLoadTask(provider);
            var completedTask = await Task.WhenAny(loadTask, Task.Delay(TimeSpan.FromSeconds(5)));

            completedTask.Should().Be(loadTask);
            await loadTask;
        }

        private static Task GetLoadTask(AvmModuleDisplayNameProvider provider)
        {
            var loadTaskField = typeof(AvmModuleDisplayNameProvider).GetField("loadTask", BindingFlags.NonPublic | BindingFlags.Instance);
            loadTaskField.Should().NotBeNull();

            var loadTask = loadTaskField?.GetValue(provider) as Task;
            loadTask.Should().NotBeNull();

            return loadTask ?? Task.CompletedTask;
        }

        private sealed class TestAvmModuleCsvIndexHttpClient : IAvmModuleCsvIndexHttpClient
        {
            private readonly Func<Uri, CancellationToken, Task<string>> getCsvAsync;
            private int requestCount;

            public TestAvmModuleCsvIndexHttpClient(Func<Uri, CancellationToken, Task<string>> getCsvAsync)
            {
                this.getCsvAsync = getCsvAsync;
            }

            public int RequestCount => Volatile.Read(ref requestCount);

            public async Task<string> GetCsvAsync(Uri csvUri, CancellationToken cancellationToken)
            {
                try
                {
                    return await getCsvAsync(csvUri, cancellationToken);
                }
                finally
                {
                    Interlocked.Increment(ref requestCount);
                }
            }
        }
    }
}
