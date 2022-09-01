// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Bicep.VSLanguageServerClient.TestServices;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    [DeploymentItem(RootDirectoryName, RootDirectoryName)]
    public class VisualStudioBicepHostTest : VisualStudioHostTest
    {
        private const string ProjectName = @"BicepTestProject";
        private const string RootDirectoryName = @"TestSolution";
        private const string TestSolutionName = @"TestSolution.sln";

        private static string? SolutionPath;

        protected ProjectTestExtension? TestProject;

        protected override void DoHostTestInitialize()
        {
            base.DoHostTestInitialize();

            VsHostUtility.VsHost = VisualStudio;

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

        public void WaitForBicepLanguageServiceActivation(IVisualStudioTextEditorTestExtension editor)
        {
            editor.Focus();

            var languageServerActivationService = VsHostUtility.VsHost!.Get<LanguageServerActivationService>();
            languageServerActivationService.WaitForLanguageServerActivation();
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
            var SolutionRootPath = Path.Combine(deploymentDirectory, RootDirectoryName);
            SolutionPath = Path.Combine(SolutionRootPath, TestSolutionName);
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            try
            {
                if (VsHostUtility.VsHost != null)
                {
                    Process visualStudioProcess = VsHostUtility.VsHost.HostProcess;

                    if (VsHostUtility.VsHost.ObjectModel.Solution.IsOpen)
                    {
                        VsHostUtility.VsHost.ObjectModel.Solution.Close();
                    }

                    PostMessage(VsHostUtility.VsHost.MainWindowHandle, 0x10, IntPtr.Zero, IntPtr.Zero); // WM_CLOSE
                    visualStudioProcess.WaitForExit(5000);

                    if (!visualStudioProcess.HasExited)
                    {
                        visualStudioProcess.Kill();
                    }
                }
            }
            catch (Exception) { }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
