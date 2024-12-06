using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Constants.SourceGenerator.Tests;

public static class Utils
{
    public static Compilation CreateCompilation(string source, params Assembly[] appendAssemblies)
    {
        var references = new List<MetadataReference>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Concat(appendAssemblies).ToArray();

        foreach (var assembly in assemblies)
        {
            if (!assembly.IsDynamic)
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        return CSharpCompilation.Create(
            "compilation",
            new[] {CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview))},
            references,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
            // .WithWarningLevel(0)
        );
    }

    public static GeneratorDriver CreateDriver(Compilation compilation, params ISourceGenerator[] generators)
    {
        return CSharpGeneratorDriver.Create(
            ImmutableArray.Create(generators),
            ImmutableArray<AdditionalText>.Empty,
            (CSharpParseOptions) compilation.SyntaxTrees.First().Options,
            null
        );
    }

    public static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics,
        params ISourceGenerator[] generators)
    {
        CreateDriver(compilation, generators)
            .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
        return updatedCompilation;
    }
}