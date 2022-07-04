// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using k8s;

namespace Bicep.Decompiler
{
    public class K8sYamlConverter
    {
        private INamingResolver nameResolver;
        private readonly Workspace workspace;
        private readonly IFileResolver fileResolver;
        private readonly Uri bicepFileUri;
        private readonly List<object> k8sResources;
        private readonly Dictionary<ModuleDeclarationSyntax, Uri> templateUrisByModule;

        private K8sYamlConverter(Workspace workspace, IFileResolver fileResolver, Uri bicepFileUri, List<object> k8sResources, Dictionary<ModuleDeclarationSyntax, Uri> templateUrisByModule)
        {
            this.workspace = workspace;
            this.fileResolver = fileResolver;
            this.bicepFileUri = bicepFileUri;
            this.k8sResources = k8sResources;
            this.nameResolver = new UniqueNamingResolver();
            this.templateUrisByModule = templateUrisByModule;
        }

        public static (ProgramSyntax programSyntax, IReadOnlyDictionary<ModuleDeclarationSyntax, Uri> templateUrisByModule) DecompileTemplate(
            Workspace workspace,
            IFileResolver fileResolver,
            Uri bicepFileUri,
            string content)
        {
            List<object> k8sResources = KubernetesYaml.LoadAllFromString(content);

            var instance = new K8sYamlConverter(
                workspace,
                fileResolver,
                bicepFileUri,
                k8sResources,
                new());

            return (instance.Parse(), instance.templateUrisByModule);
        }

        private ProgramSyntax Parse()
        {
            var statements = new List<SyntaxBase>();

            foreach (var k8sRes in k8sResources)
            {

            }

            return new ProgramSyntax(
                statements.SelectMany(x => new[] { x, SyntaxFactory.NewlineToken }),
                SyntaxFactory.CreateToken(TokenType.EndOfFile, ""),
                Enumerable.Empty<IDiagnostic>()
            );
        }
    }
}
