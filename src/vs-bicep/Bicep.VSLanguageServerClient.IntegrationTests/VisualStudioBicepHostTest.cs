// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Test.Apex.VisualStudio.Solution;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    [DeploymentItem(RootDirectoryName, RootDirectoryName)]
    public class VisualStudioBicepHostTest : VisualStudioHostTest
    {
        private static VisualStudioHost? VsHost;
        private const string RootDirectoryName = @"TestSolution";
        private const string TestSolutionName = @"TestSolution.sln";
        protected ProjectTestExtension? TestProject;
        protected const string ProjectName = @"BicepTestProject";

        private static string? SolutionPath;
        public static string? SolutionRootPath { get; private set; }

        protected override void DoHostTestInitialize()
        {
            base.DoHostTestInitialize();

            VsHost = VisualStudio;

            Solution.Open(SolutionPath);
            Solution.WaitForFullyLoaded();

            TestProject = Solution[ProjectName];
        }

        public SolutionService Solution
        {
            get
            {
                return VisualStudio.ObjectModel.Solution;
            }
        }

        protected override VisualStudioHostConfiguration GetVisualStudioHostConfiguration()
        {
            VisualStudioHostConfiguration configuration = base.GetVisualStudioHostConfiguration();

            // starts the experimental instance and deploys the visual studio client vsix
            configuration.CommandLineArguments += " /RootSuffix Exp";

            return configuration;
        }

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            var deploymentDirectory = context.DeploymentDirectory;
            SolutionRootPath = Path.Combine(deploymentDirectory, RootDirectoryName);
            SolutionPath = Path.Combine(SolutionRootPath, TestSolutionName);
        }
    }
}
