// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel.Namespaces
{
    public class SystemNamespaceSymbol : NamespaceSymbol
    {
        // TODO: Banned functions: variables, parameters, copyIndex (add test to prevent anyone from adding them in the future)
        private static readonly ImmutableArray<FunctionOverload> SystemOverloads = new[]
        {
            FunctionOverload.CreateFixed("any", LanguageConstants.Any, LanguageConstants.Any),

            FunctionOverload.CreateWithVarArgs("concat", LanguageConstants.Array, 1, LanguageConstants.Array),
            FunctionOverload.CreateWithVarArgs("concat", LanguageConstants.String, 1, UnionType.Create(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool)),
            FunctionOverload.CreatePartialFixed("format", LanguageConstants.String, new[]
            {
                LanguageConstants.String
            }, LanguageConstants.Any),
            FunctionOverload.CreateFixed("base64", LanguageConstants.String, LanguageConstants.String),
            new FunctionOverload("padLeft", LanguageConstants.String, 2, 3, new[]
            {
                UnionType.Create(LanguageConstants.String, LanguageConstants.Int), LanguageConstants.Int, LanguageConstants.String
            }, null),
            FunctionOverload.CreateFixed("replace", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("toLower", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("toUpper", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("length", LanguageConstants.Int, UnionType.Create(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array)),
            FunctionOverload.CreateFixed("split", LanguageConstants.Array, LanguageConstants.String, UnionType.Create(LanguageConstants.String, LanguageConstants.Array)),
            FunctionOverload.CreateFixed("add", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("sub", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("mul", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("div", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("mod", LanguageConstants.Int, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("string", LanguageConstants.String, LanguageConstants.Any),
            FunctionOverload.CreateFixed("int", LanguageConstants.Int, UnionType.Create(LanguageConstants.String, LanguageConstants.Int)),
            FunctionOverload.CreateWithVarArgs("uniqueString", LanguageConstants.String, 1, LanguageConstants.String),
            FunctionOverload.CreateWithVarArgs("guid", LanguageConstants.String, 1, LanguageConstants.String),
            FunctionOverload.CreateFixed("trim", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("uri", LanguageConstants.String, LanguageConstants.String, LanguageConstants.String),
            new FunctionOverload("substring", LanguageConstants.String, 2, 3, new[] {LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null),
            FunctionOverload.CreateFixed("take", LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Int),
            FunctionOverload.CreateFixed("take", LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int),
            FunctionOverload.CreateFixed("skip", LanguageConstants.Array, LanguageConstants.Array, LanguageConstants.Int),
            FunctionOverload.CreateFixed("skip", LanguageConstants.String, LanguageConstants.String, LanguageConstants.Int),
            FunctionOverload.CreateFixed("empty", LanguageConstants.Bool, UnionType.Create(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String)),
            FunctionOverload.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.Object, LanguageConstants.String),
            FunctionOverload.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.Array, LanguageConstants.Any),
            FunctionOverload.CreateFixed("contains", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateWithVarArgs("intersection", LanguageConstants.Object, 2, LanguageConstants.Object),
            FunctionOverload.CreateWithVarArgs("intersection", LanguageConstants.Array, 2, LanguageConstants.Array),
            FunctionOverload.CreateWithVarArgs("union", LanguageConstants.Object, 2, LanguageConstants.Object),
            FunctionOverload.CreateWithVarArgs("union", LanguageConstants.Array, 2, LanguageConstants.Array),
            FunctionOverload.CreateFixed("first", LanguageConstants.Any, LanguageConstants.Array),
            FunctionOverload.CreateFixed("first", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("last", LanguageConstants.Any, LanguageConstants.Array),
            FunctionOverload.CreateFixed("last", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("indexOf", LanguageConstants.Int, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("lastIndexOf", LanguageConstants.Int, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("startsWith", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("endsWith", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),

            // TODO: Needs to support number type as well
            FunctionOverload.CreateWithVarArgs("min", LanguageConstants.Int, 1, LanguageConstants.Int),
            FunctionOverload.CreateFixed("min", LanguageConstants.Int, LanguageConstants.Array),

            // TODO: Needs to support number type as well
            FunctionOverload.CreateWithVarArgs("max", LanguageConstants.Int, 1, LanguageConstants.Int),
            FunctionOverload.CreateFixed("max", LanguageConstants.Int, LanguageConstants.Array),

            FunctionOverload.CreateFixed("range", LanguageConstants.Array, LanguageConstants.Int, LanguageConstants.Int),
            FunctionOverload.CreateFixed("base64ToString", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("base64ToJson", LanguageConstants.Any, LanguageConstants.String),
            FunctionOverload.CreateFixed("uriComponentToString", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("uriComponent", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("dataUriToString", LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("dataUri", LanguageConstants.String, LanguageConstants.Any),
            FunctionOverload.CreateFixed("array", LanguageConstants.Array, LanguageConstants.Any),
            FunctionOverload.CreateWithVarArgs("createArray", LanguageConstants.Array, 1, LanguageConstants.Any),
            FunctionOverload.CreateWithVarArgs("coalesce", LanguageConstants.Any, 1, LanguageConstants.Any),

            // TODO: Requires number type
            //FunctionInfo.CreateFixed("float",LanguageConstants.Number,LanguageConstants.Any),

            FunctionOverload.CreateFixed("bool", LanguageConstants.Bool, LanguageConstants.Any),

            // TODO: Needs number type
            FunctionOverload.CreateFixed("less", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("less", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionOverload.CreateFixed("lessOrEquals", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("lessOrEquals", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionOverload.CreateFixed("greater", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("greater", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            // TODO: Needs number type
            FunctionOverload.CreateFixed("greaterOrEquals", LanguageConstants.Bool, LanguageConstants.String, LanguageConstants.String),
            FunctionOverload.CreateFixed("greaterOrEquals", LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Int),

            FunctionOverload.CreateFixed("equals", LanguageConstants.Bool, LanguageConstants.Any, LanguageConstants.Any),
            FunctionOverload.CreateFixed("json", LanguageConstants.Any, LanguageConstants.String),
            FunctionOverload.CreateFixed("not", LanguageConstants.Bool, LanguageConstants.Bool),
            FunctionOverload.CreateWithVarArgs("and", LanguageConstants.Bool, 2, LanguageConstants.Bool),
            FunctionOverload.CreateWithVarArgs("or", LanguageConstants.Bool, 2, LanguageConstants.Bool),

            // TODO: Return type should be a type tranformation
            FunctionOverload.CreateFixed("if", LanguageConstants.Any, LanguageConstants.Bool, LanguageConstants.Any, LanguageConstants.Any),

            new FunctionOverload("dateTimeAdd", LanguageConstants.String, 2, 3, Enumerable.Repeat(LanguageConstants.String, 3), null),

            // newGuid and utcNow are only allowed in parameter default values
            new FunctionOverload("utcNow", LanguageConstants.String, 0, 1, Enumerable.Repeat(LanguageConstants.String, 1), null, FunctionFlags.ParamDefaultsOnly),
            new FunctionOverload("newGuid", LanguageConstants.String, 0, 0, Enumerable.Empty<TypeSymbol>(), null, FunctionFlags.ParamDefaultsOnly),
        }.ToImmutableArray();

        public SystemNamespaceSymbol() : base("sys", SystemOverloads)
        {
        }
    }
}

