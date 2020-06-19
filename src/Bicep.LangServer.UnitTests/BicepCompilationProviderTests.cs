using Bicep.Core.Samples;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationProviderTests
    {
        [TestMethod]
        public void Create_ShouldReturnValidCompilation()
        {
            var provider = new BicepCompilationProvider();

            var context = provider.Create(DataSets.Parameters_LF.Bicep);

            context.Lexer.GetErrors().Should().BeEmpty();
            context.Compilation.Should().NotBeNull();
            context.Compilation.GetSemanticModel().GetAllDiagnostics().Should().BeEmpty();
            context.LineStarts.Should().NotBeEmpty();
            context.LineStarts[0].Should().Be(0);
        }
    }
}
