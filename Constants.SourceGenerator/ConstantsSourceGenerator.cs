using System;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ConstantsGenerator
{
    [Generator]
    public class ConstantsSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Console.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));

            var source = $$"""
                           using System;
                           namespace ExampleSourceGenerated
                           {
                               public static class ExampleSourceGenerated
                               {
                                   public static string Print()
                                   {
                                       Console.WriteLine("Hello from source generator");
                                       return "{{DateTime.Now.ToString(CultureInfo.InvariantCulture)}}";
                                   }
                               }
                           }
                           """;

            context.AddSource("exampleSourceGenerator", SourceText.From(source, Encoding.UTF8));
        }
    }
}