// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Utils
{
    internal class ModuleDefinition
    {
        public string ModuleName { get; }
        public ResourceScope ModulePropertyScopeType { get; }
        public IReadOnlyCollection<StringSyntax?>? ModulePropertyScopeValue { get; }
        public StringSyntax ModulePropertyNameValue { get; }

        public ModuleDefinition(string moduleName, ResourceScope modulePropertyScopeType, ImmutableArray<StringSyntax?>? modulePropertyScopeValue, StringSyntax modulePropertyNameValue)
        {
            ModuleName = moduleName;
            ModulePropertyScopeType = modulePropertyScopeType;
            ModulePropertyScopeValue = modulePropertyScopeValue;
            ModulePropertyNameValue = modulePropertyNameValue;
        }

        public static readonly IEqualityComparer<ModuleDefinition> EqualityComparer = new ModuleComparer();

        private class ModuleComparer : IEqualityComparer<ModuleDefinition>
        {

            public bool Equals(ModuleDefinition x, ModuleDefinition y)
            {
                if (x.ModulePropertyScopeType != y.ModulePropertyScopeType)
                {
                    return false;
                }

                var xpnv = x.ModulePropertyNameValue.TryGetLiteralValue();
                var ypnv = y.ModulePropertyNameValue.TryGetLiteralValue();

                if (xpnv is null || ypnv is null || !string.Equals(xpnv, ypnv, StringComparison.InvariantCultureIgnoreCase))
                {
                    //no point in checking scope at least one of them is interpolated or their literal names differ
                    return false;
                }

                if ((x.ModulePropertyScopeValue is null || x.ModulePropertyScopeValue.Count == 0) && (y.ModulePropertyScopeValue is null || y.ModulePropertyScopeValue.Count == 0))
                {
                    // this case indicates that modules are being deployed to scope without name specified (e.g. subscription(), resourceGroup(), etc.)
                    // and as we checked before that names are equal, we need to return true.
                    return true;
                }


                //null values indicates that module used is parent scope
                //as we checked case, when we have both modules in parent scope, we can safely return false when null is found
                //this might still lead to situation, when we define scope which in real deployment is same as parent scope, but this cannot be forseen
                //(it'll be a runtime error, while we check only for compilation errors)
                return x.ModulePropertyScopeValue is not null && y.ModulePropertyScopeValue is not null
                    && Enumerable.SequenceEqual(x.ModulePropertyScopeValue, y.ModulePropertyScopeValue, StringSyntaxComparerInstance);

            }

            public int GetHashCode(ModuleDefinition obj)
            {
                var hc = new HashCode();
                hc.Add(obj.ModulePropertyScopeType);
                foreach (var x in obj.ModulePropertyScopeValue ?? Enumerable.Empty<StringSyntax?>())
                {
                    hc.Add(x, StringSyntaxComparerInstance);
                }
                hc.Add(obj.ModulePropertyNameValue?.TryGetLiteralValue(), StringComparer.InvariantCultureIgnoreCase);
                return hc.ToHashCode();                
            }

            private static readonly StringSyntaxComparer StringSyntaxComparerInstance = new();

            private class StringSyntaxComparer : IEqualityComparer<StringSyntax?>
            {
                public bool Equals(StringSyntax? x, StringSyntax? y)
                {
                    var xv = x?.TryGetLiteralValue();
                    var yv = y?.TryGetLiteralValue();
                    //null values indicates value is interpolated or was not a string (i.e. reference).
                    //since at this point we check only for literal matching, we do comparison only if both are not nulls (plain strings)
                    return xv is not null && yv is not null && string.Equals(xv, yv, StringComparison.InvariantCultureIgnoreCase);

                }

                public int GetHashCode(StringSyntax? obj)
                {
                    return StringComparer.InvariantCultureIgnoreCase.GetHashCode(obj?.TryGetLiteralValue() ?? string.Empty);
                }
            }
        }
    }
}
