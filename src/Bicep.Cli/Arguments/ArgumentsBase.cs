// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Features;

namespace Bicep.Cli.Arguments
{
    public abstract class ArgumentsBase : IFeatureProviderSource
    {
        private delegate int ArgHandler(ArgumentsBase instance, string[] args, int position);
        private static readonly IReadOnlyDictionary<string, ArgHandler> FeatureArgHandlers = new Dictionary<string, ArgHandler>
        {
            {
                "--cache-root-directory",
                (i, a, p) => HandleParameterWithSingleArg(i, a, p, instance => instance.CacheRootDirectory, (instance, result) => instance.CacheRootDirectory = result)
            },
            {
                "--enable-registry",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.RegistryEnabled, (instance, result) => instance.RegistryEnabled = result)
            },
            {
                "--enable-symbolic-name-codegen",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.SymbolicNameCodegenEnabled, (instance, result) => instance.SymbolicNameCodegenEnabled = result)
            },
            {
                "--enable-imports",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.ImportsEnabled, (instance, result) => instance.ImportsEnabled = result)
            },
            {
                "--enable-resource-typed-params-and-outputs",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.ResourceTypedParamsAndOutputsEnabled, (instance, result) => instance.ResourceTypedParamsAndOutputsEnabled = result)
            },
            {
                "--enable-source-mapping",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.SourceMappingEnabled, (instance, result) => instance.SourceMappingEnabled = result)
            },
            {
                "--enable-params-files",
                (i, a, p) => HandleSwitchWithOptionalBooleanArg(i, a, p, instance => instance.ParamsFilesEnabled, (instance, result) => instance.ParamsFilesEnabled = result)
            },
            {
                "--assembly-version-override",
                (i, a, p) => HandleParameterWithSingleArg(i, a, p, instance => instance.AssemblyVersion, (instance, result) => instance.AssemblyVersion = result)
            }
        };


        public string CommandName { get; }

        sbyte IFeatureProviderSource.Priority => -1;

        public string? AssemblyVersion { get; private set; }

        public string? CacheRootDirectory { get; private set; }

        public bool? RegistryEnabled { get; private set; }

        public bool? SymbolicNameCodegenEnabled { get; private set; }

        public bool? ImportsEnabled { get; private set; }

        public bool? ResourceTypedParamsAndOutputsEnabled { get; private set; }

        public bool? SourceMappingEnabled { get; private set; }

        public bool? ParamsFilesEnabled { get; private set; }

        protected ArgumentsBase(string commandName)
        {
            CommandName = commandName;
        }

        protected static bool IsFeatureArg(string arg) => FeatureArgHandlers.ContainsKey(arg);
        protected int HandleFeatureArg(string[] args, int position) => FeatureArgHandlers[args[position]].Invoke(this, args, position);

        private static int HandleParameterWithSingleArg(ArgumentsBase instance, string[] args, int position, Func<ArgumentsBase, string?> prevSettingGetter, Action<ArgumentsBase, string> paramSetter)
        {
            if (args.Length == position + 1)
            {
                throw new CommandLineException($"The {args[position]} parameter expects an argument");
            }
            if (prevSettingGetter.Invoke(instance) is not null)
            {
                throw new CommandLineException($"The {args[position]} parameter cannot be specified twice");
            }
            paramSetter.Invoke(instance, args[position + 1]);
            return 1;
        }

        private static int HandleSwitchWithOptionalBooleanArg(ArgumentsBase instance, string[] args, int position, Func<ArgumentsBase, bool?> prevSettingGetter, Action<ArgumentsBase, bool> flagSetter)
        {
            if (prevSettingGetter.Invoke(instance) is not null)
            {
                throw new CommandLineException($"The {args[position]} parameter cannot be specified twice");
            }

            if (args.Length > position + 1 && args[position + 1] is string maybeBoolArg && bool.TryParse(maybeBoolArg, out var result))
            {
                flagSetter(instance, result);
                return 1;
            }

            flagSetter(instance, true);
            return 0;
        }
    }
}
