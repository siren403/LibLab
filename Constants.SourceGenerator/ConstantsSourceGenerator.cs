using Microsoft.CodeAnalysis;

namespace ConstantsGenerator;

[Generator]
public class ConstantsSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(_ =>
        {
            _.AddSource("CustomAttribute.g.cs", @"
using System;
    
[AttributeUsage(AttributeTargets.Class)]
public class CustomAttribute : Attribute
{
    public CustomAttribute()
    {
        
    }
}
            ");
        });
    }

    public void Execute(GeneratorExecutionContext context)
    {
    }
}