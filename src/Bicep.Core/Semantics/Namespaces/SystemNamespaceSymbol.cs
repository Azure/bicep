// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces
{
    public class SystemNamespaceSymbol : NamespaceSymbol
    {
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
            FunctionOverload.CreateWithVarArgs("coalesce", LanguageConstants.Any, 1, LanguageConstants.Any),

            // TODO: Requires number type
            //FunctionInfo.CreateFixed("float",LanguageConstants.Number,LanguageConstants.Any),

            FunctionOverload.CreateFixed("bool", LanguageConstants.Bool, LanguageConstants.Any),

            FunctionOverload.CreateFixed("json", LanguageConstants.Any, LanguageConstants.String),
            
            new FunctionOverload("dateTimeAdd", LanguageConstants.String, 2, 3, Enumerable.Repeat(LanguageConstants.String, 3), null),

            // newGuid and utcNow are only allowed in parameter default values
            new FunctionOverload("utcNow", LanguageConstants.String, 0, 1, Enumerable.Repeat(LanguageConstants.String, 1), null, FunctionFlags.ParamDefaultsOnly),
            new FunctionOverload("newGuid", LanguageConstants.String, 0, 0, Enumerable.Empty<TypeSymbol>(), null, FunctionFlags.ParamDefaultsOnly),
        }.ToImmutableArray();

        // TODO: Add copyIndex here when we support loops.
        private static readonly ImmutableArray<BannedFunction> BannedFunctions = new[]
        {
            /*
             * The true(), false(), and null() functions are not included in this list because
             * we parse true, false and null as keywords in the lexer, so they can't be used as functions anyway.
             */

            new BannedFunction("variables", builder => builder.VariablesFunctionNotSupported()),
            new BannedFunction("parameters", builder => builder.ParametersFunctionNotSupported()),
            new BannedFunction("if", builder => builder.IfFunctionNotSupported()),
            new BannedFunction("createArray", builder => builder.CreateArrayFunctionNotSupported()),
            new BannedFunction("createObject", builder => builder.CreateObjectFunctionNotSupported()),

            BannedFunction.CreateForOperator("add", "+"),
            BannedFunction.CreateForOperator("sub", "-"),
            BannedFunction.CreateForOperator("mul", "*"),
            BannedFunction.CreateForOperator("div", "/"),
            BannedFunction.CreateForOperator("mod", "%"),
            BannedFunction.CreateForOperator("less", "<"),
            BannedFunction.CreateForOperator("lessOrEquals", "<="),
            BannedFunction.CreateForOperator("greater", ">"),
            BannedFunction.CreateForOperator("greaterOrEquals", ">="),
            BannedFunction.CreateForOperator("equals", "=="),
            BannedFunction.CreateForOperator("not", "!"),
            BannedFunction.CreateForOperator("and", "&&"),
            BannedFunction.CreateForOperator("or", "||")
        }.ToImmutableArray();

        public SystemNamespaceSymbol() : base("sys", SystemOverloads, BannedFunctions)
        {
        }
    }
}

