using ConstantsGenerator;

namespace Constants.SourceGenerator.Tests;

using static Utils;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var source = """
                     namespace Tests 
                     {
                         class Program
                         { 
                             public static void Main(string[] args) 
                             {
                             } 
                         }
                         
                         [Custom]
                         public class Foo 
                         {
                         }
                     }
                     """;
        var newCompilation = RunGenerators(
            CreateCompilation(source),
            out var diagnostics,
            new ConstantsSourceGenerator()
        );

        Assert.IsEmpty(diagnostics);
        Assert.IsEmpty(newCompilation.GetDiagnostics());
    }
}