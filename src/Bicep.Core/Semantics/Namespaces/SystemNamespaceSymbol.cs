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
            new FunctionOverloadBuilder("any")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            FunctionOverload.CreateWithVarArgs("concat", LanguageConstants.Array, 1, LanguageConstants.Array),
            FunctionOverload.CreateWithVarArgs("concat", LanguageConstants.String, 1, UnionType.Create(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool)),
            FunctionOverload.CreatePartialFixed("format", LanguageConstants.String, new[]
            {
                LanguageConstants.String
            }, LanguageConstants.Any),

            new FunctionOverloadBuilder("base64")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverload("padLeft", LanguageConstants.String, 2, 3, new[]
            {
                UnionType.Create(LanguageConstants.String, LanguageConstants.Int), LanguageConstants.Int, LanguageConstants.String
            }, null),

            new FunctionOverloadBuilder("replace")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("toLower")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("toUpper")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("length")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(UnionType.Create(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array))
                .Build(),
            
            new FunctionOverloadBuilder("split")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.String, UnionType.Create(LanguageConstants.String, LanguageConstants.Array))
                .Build(),

            new FunctionOverloadBuilder("string")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("int")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(UnionType.Create(LanguageConstants.String, LanguageConstants.Int))
                .Build(),

            FunctionOverload.CreateWithVarArgs("uniqueString", LanguageConstants.String, 1, LanguageConstants.String),

            FunctionOverload.CreateWithVarArgs("guid", LanguageConstants.String, 1, LanguageConstants.String),

            new FunctionOverloadBuilder("trim")
                .WithReturnType( LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("uri")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverload("substring", LanguageConstants.String, 2, 3, new[] {LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Int}, null),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.Int)
                .Build(),

            new FunctionOverloadBuilder("empty")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(UnionType.Create(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String))
                .Build(),
            
            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Object, LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Array, LanguageConstants.Any)
                .Build(),
            
            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),
            
            FunctionOverload.CreateWithVarArgs("intersection", LanguageConstants.Object, 2, LanguageConstants.Object),
            
            FunctionOverload.CreateWithVarArgs("intersection", LanguageConstants.Array, 2, LanguageConstants.Array),
            
            FunctionOverload.CreateWithVarArgs("union", LanguageConstants.Object, 2, LanguageConstants.Object),
            
            FunctionOverload.CreateWithVarArgs("union", LanguageConstants.Array, 2, LanguageConstants.Array),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),
            
            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("indexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),

            new FunctionOverloadBuilder("lastIndexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("startsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("endsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.String, LanguageConstants.String)
                .Build(),
            
            // TODO: Needs to support number type as well
            FunctionOverload.CreateWithVarArgs("min", LanguageConstants.Int, 1, LanguageConstants.Int),

            new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),
            
            // TODO: Needs to support number type as well
            FunctionOverload.CreateWithVarArgs("max", LanguageConstants.Int, 1, LanguageConstants.Int),

            new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithFixedParameters(LanguageConstants.Array)
                .Build(),

            new FunctionOverloadBuilder("range")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Int, LanguageConstants.Int)
                .Build(),
            
            new FunctionOverloadBuilder("base64ToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("base64ToJson")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("uriComponentToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("uriComponent")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("dataUriToString")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),
            
            new FunctionOverloadBuilder("dataUri")
                .WithReturnType(LanguageConstants.String)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),
            
            new FunctionOverloadBuilder("array")
                .WithReturnType(LanguageConstants.Array)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),
            
            FunctionOverload.CreateWithVarArgs("coalesce", LanguageConstants.Any, 1, LanguageConstants.Any),

            // TODO: Requires number type
            //new FunctionOverloadBuilder("float")
            //    .WithReturnType(LanguageConstants.Number)
            //    .WithFixedParameters(LanguageConstants.Any)
            //    .Build(),

            new FunctionOverloadBuilder("bool")
                .WithReturnType(LanguageConstants.Bool)
                .WithFixedParameters(LanguageConstants.Any)
                .Build(),

            new FunctionOverloadBuilder("json")
                .WithReturnType(LanguageConstants.Any)
                .WithFixedParameters(LanguageConstants.String)
                .Build(),

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

