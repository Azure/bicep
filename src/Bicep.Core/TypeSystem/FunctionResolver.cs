using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public static class FunctionResolver
    {
        private static readonly ImmutableArray<FunctionInfo> SystemFunctions = new[]
        {
            FunctionInfo.CreateFixed("any", LanguageConstants.Any, LanguageConstants.Any),

            FunctionInfo.CreateWithVarArgs("concat", LanguageConstants.Array, 1, LanguageConstants.Array),
            FunctionInfo.CreateWithVarArgs("concat", LanguageConstants.String, 1, UnionType.Create(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool)),
            FunctionInfo.CreatePartialFixed("format", LanguageConstants.String, new[]
            {
                LanguageConstants.String
            }, LanguageConstants.Any),
            FunctionInfo.CreateFixed("base64", LanguageConstants.String, LanguageConstants.String),
            new FunctionInfo("padLeft", LanguageConstants.String, 2, 3, new[]
            {
                UnionType.Create(LanguageConstants.String, LanguageConstants.Int), LanguageConstants.Int, LanguageConstants.String
            }, null),
            FunctionInfo.CreateFixed("replace", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("toLower", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("toUpper", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("length", LanguageConstants.Int, UnionType.Create(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array)),
            FunctionInfo.CreateFixed("split", LanguageConstants.Array, LanguageConstants.String, UnionType.Create(LanguageConstants.String, LanguageConstants.Array)),
            FunctionInfo.CreateFixed("add", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("sub", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("mul", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("div", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("mod", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("string", LanguageConstants.String, LanguageConstants.Any),
            FunctionInfo.CreateFixed("int", LanguageConstants.Int, UnionType.Create(LanguageConstants.String, LanguageConstants.Int)),
            FunctionInfo.CreateWithVarArgs("uniqueString", LanguageConstants.String, 1, LanguageConstants.String),
            FunctionInfo.CreateWithVarArgs("guid", LanguageConstants.String, 1, LanguageConstants.String),
            FunctionInfo.CreateFixed("trim", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("uri", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String),
            new FunctionInfo("substring", LanguageConstants.String, 2, 3, new[] {LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null),
            FunctionInfo.CreateFixed("take", LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Int),
            FunctionInfo.CreateFixed("take", LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int),
            FunctionInfo.CreateFixed("skip", LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Int),
            FunctionInfo.CreateFixed("skip", LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int),
            FunctionInfo.CreateFixed("empty", LanguageConstants.Bool, UnionType.Create(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String)),
            FunctionInfo.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.Object, LanguageConstants.String),
            FunctionInfo.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.Array, LanguageConstants.Any),
            FunctionInfo.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateWithVarArgs("intersection", LanguageConstants.Object, 2, LanguageConstants.Object),
            FunctionInfo.CreateWithVarArgs("intersection", LanguageConstants.Array, 2, LanguageConstants.Array),
            FunctionInfo.CreateWithVarArgs("union", LanguageConstants.Object, 2, LanguageConstants.Object),
            FunctionInfo.CreateWithVarArgs("union", LanguageConstants.Array, 2, LanguageConstants.Array),
            FunctionInfo.CreateFixed("first", LanguageConstants.Any, LanguageConstants.Array),
            FunctionInfo.CreateFixed("first", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("last", LanguageConstants.Any, LanguageConstants.Array),
            FunctionInfo.CreateFixed("last", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("indexOf", LanguageConstants.Int, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("lastIndexOf", LanguageConstants.Int, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("startsWith", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("endsWith", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),

            // TODO: Needs to support number type as well
            FunctionInfo.CreateWithVarArgs("min", LanguageConstants.Int, 1, LanguageConstants.Int),
            FunctionInfo.CreateFixed("min", LanguageConstants.Int, LanguageConstants.Array),

            // TODO: Needs to support number type as well
            FunctionInfo.CreateWithVarArgs("max", LanguageConstants.Int, 1, LanguageConstants.Int),
            FunctionInfo.CreateFixed("max", LanguageConstants.Int, LanguageConstants.Array),

            FunctionInfo.CreateFixed("range", LanguageConstants.Array, LanguageConstants.Int, LanguageConstants.Int),
            FunctionInfo.CreateFixed("base64ToString", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("base64ToJson", LanguageConstants.Any, LanguageConstants.String),
            FunctionInfo.CreateFixed("uriComponentToString", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("uriComponent", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("dataUriToString", LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("dataUri", LanguageConstants.String, LanguageConstants.Any),
            FunctionInfo.CreateFixed("array", LanguageConstants.Array, LanguageConstants.Any),
            FunctionInfo.CreateWithVarArgs("createArray", LanguageConstants.Array, 1, LanguageConstants.Any),
            FunctionInfo.CreateWithVarArgs("coalesce", LanguageConstants.Any, 1, LanguageConstants.Any),

            // TODO: Requires number type
            //FunctionInfo.CreateFixed("float",LanguageConstants.Number,LanguageConstants.Any),

            FunctionInfo.CreateFixed("bool", LanguageConstants.Bool, LanguageConstants.Any),

            // TODO: Needs number type
            FunctionInfo.CreateFixed("less", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("less", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionInfo.CreateFixed("lessOrEquals", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("lessOrEquals", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionInfo.CreateFixed("greater", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("greater", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionInfo.CreateFixed("greaterOrEquals", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionInfo.CreateFixed("greaterOrEquals", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            FunctionInfo.CreateFixed("equals", LanguageConstants.Bool, LanguageConstants.Any, LanguageConstants.Any),
            FunctionInfo.CreateFixed("json", LanguageConstants.Any, LanguageConstants.String),
            FunctionInfo.CreateFixed("not", LanguageConstants.Bool, LanguageConstants.Bool),
            FunctionInfo.CreateWithVarArgs("and", LanguageConstants.Bool, 2, LanguageConstants.Bool),
            FunctionInfo.CreateWithVarArgs("or", LanguageConstants.Bool, 2, LanguageConstants.Bool),

            // TODO: Return type should be a type tranformation
            FunctionInfo.CreateFixed("if", LanguageConstants.Any, LanguageConstants.Bool, LanguageConstants.Any, LanguageConstants.Any),

            new FunctionInfo("dateTimeAdd", LanguageConstants.String, 2, 3, Enumerable.Repeat(LanguageConstants.String, 3), null),
            
            //TODO: newGuid and utcNow are only allowed in parameter default values
            //FunctionInfo.CreateFixed("newGuid", LanguageConstants.String),
            //new FunctionInfo("utcNow", LanguageConstants.String, 0, 1, LanguageConstants.String.AsEnumerable(), null),

            //TODO: reference function (has to be inlined)
            //TODO: list* function (has to be inlined)

        }.ToImmutableArray();

        private static readonly ImmutableArray<FunctionInfo> AzFunctions = new[]
        {
            // TODO: Only valid at subscription scope
            // TODO: Add schema for return type
            FunctionInfo.CreateFixed("subscription", LanguageConstants.Object),

            // TODO: Only valid at RG scope
            // TODO: Add schema for return type
            FunctionInfo.CreateFixed("resourceGroup", LanguageConstants.Object),

            // TODO: Add schema for return type
            FunctionInfo.CreateFixed("deployment", LanguageConstants.Object),

            // TODO: Add schema for return type
            FunctionInfo.CreateFixed("environment", LanguageConstants.Object),

            // TODO: This is based on docs. Verify
            FunctionInfo.CreateWithVarArgs("resourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionInfo.CreateWithVarArgs("subscriptionResourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionInfo.CreateWithVarArgs("tenantResourceId", LanguageConstants.String, 2, LanguageConstants.String),
            FunctionInfo.CreateWithVarArgs("extensionResourceId", LanguageConstants.String, 3, LanguageConstants.String),

            // TODO: Not sure about return type
            new FunctionInfo("providers", LanguageConstants.Array, 1, 2, Enumerable.Repeat(LanguageConstants.String, 2), null),

            // TODO: return type is string[]
            new FunctionInfo("pickZones", LanguageConstants.Array, 3, 5, new[] {LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null)
        }.ToImmutableArray();

        private static readonly ILookup<string, FunctionInfo> AllFunctions = SystemFunctions.Concat(AzFunctions).ToLookup(fi => fi.Name, LanguageConstants.IdentifierComparer);
        
        // TODO: Banned functions: variables, parameters, copyIndex (add test to prevent anyone from adding them in the future)

        public static IEnumerable<FunctionInfo> GetMatches(string name, IList<TypeSymbol> argumentTypes)
        {
            // lookup name matches by result type
            var candidateLookup = AllFunctions[name].ToLookup(fi => fi.Match(argumentTypes));

            if (candidateLookup.Contains(FunctionMatchResult.Match))
            {
                // for full match, just return the first one
                return candidateLookup[FunctionMatchResult.Match].Take(1);
            }

            if (candidateLookup.Contains(FunctionMatchResult.PotentialMatch))
            {
                // for partial matches, return all of them
                return candidateLookup[FunctionMatchResult.PotentialMatch];
            }

            // mismatches are not helpful
            return Enumerable.Empty<FunctionInfo>();
        }
    }
}
