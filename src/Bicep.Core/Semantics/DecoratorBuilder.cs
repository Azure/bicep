// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics;

public class DecoratorBuilder
{
    private readonly FunctionOverloadBuilder functionOverloadBuilder;
    private TypeSymbol attachableType;
    private DecoratorValidator? validator;
    private DecoratorEvaluator? evaluator;

    public DecoratorBuilder(string name)
    {
        this.functionOverloadBuilder = new FunctionOverloadBuilder(name);
        this.attachableType = LanguageConstants.Any;
    }

    public DecoratorBuilder WithDescription(string description)
    {
        this.functionOverloadBuilder.WithDescription(description);

        return this;
    }

    public DecoratorBuilder WithParameter(
        string name,
        TypeSymbol type,
        string description,
        FunctionParameterFlags flags,
        FunctionArgumentTypeCalculator? calculator = null)
    {
        this.functionOverloadBuilder.WithParameter(name, type, description, flags, calculator);

        return this;
    }

    public DecoratorBuilder WithVariableParameter(string namePrefix, TypeSymbol type, int minimumCount, string description, FunctionParameterFlags flags = FunctionParameterFlags.None)
    {
        this.functionOverloadBuilder.WithVariableParameter(namePrefix, type, minimumCount, description, flags);

        return this;
    }

    public DecoratorBuilder WithFlags(FunctionFlags flags)
    {
        if (!Enum.IsDefined(typeof(FunctionFlags), flags))
        {
            // VisitMissingDeclarationSyntax in the TypeAssignmentVisitor uses the flags to determine the error message in cases of dangling decorators
            throw new ArgumentException($"The specified flags value is not explicitly defined in the {nameof(FunctionFlags)} enumeration. Define the combination and update usages to ensure the combination is handled correctly.");
        }

        this.functionOverloadBuilder.WithFlags(flags);

        return this;
    }

    public DecoratorBuilder WithAttachableType(TypeSymbol attachableType)
    {
        this.attachableType = attachableType;

        return this;
    }

    public DecoratorBuilder WithValidator(DecoratorValidator validator)
    {
        this.validator = validator;

        return this;
    }

    public DecoratorBuilder WithEvaluator(DecoratorEvaluator evaluator)
    {
        this.evaluator = evaluator;

        return this;
    }

    public Decorator Build() => new(functionOverloadBuilder.Build(), attachableType, validator, evaluator);
}
