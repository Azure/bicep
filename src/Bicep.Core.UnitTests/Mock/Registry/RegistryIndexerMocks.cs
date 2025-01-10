using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry.Indexing;
using FluentAssertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Registry
{
    public static class RegistryIndexerMocks
    {
        private const string PublicRegistry = "mcr.microsoft.com";

        public static Mock<IPublicModuleMetadataProvider> CreatePublicMetadataProviderMock(
            IEnumerable<(string moduleName, string? description, string? documentationUri, IEnumerable<RegistryModuleVersionMetadata> versions)> modules)
        {
            modules.Should().SatisfyRespectively(
                m => m.moduleName.Should().StartWith("bicep/", "All public modules should start with '/bicep'"),
            );

            var publicModuleMetadataProvider = StrictMock.Of<IPublicModuleMetadataProvider>();
            publicModuleMetadataProvider.Setup(x => x.GetModulesAsync()).ReturnsAsync(
                modules.Select(m => new RegistryModuleMetadata(PublicRegistry, m.moduleName, m.description, m.documentationUri);
            publicModuleMetadataProvider.Setup(x => x.GetModuleVersionsAsync(It.IsAny<string>())).ReturnsAsync((string modulePath) =>
                [.. modules.Single(m => m.moduleName.EqualsOrdinally(modulePath)).versions]);

            return publicModuleMetadataProvider;
        }
    }
}
