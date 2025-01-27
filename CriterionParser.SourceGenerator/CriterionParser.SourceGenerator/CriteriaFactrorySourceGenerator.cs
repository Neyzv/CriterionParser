using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace CriterionParser.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed partial class CriteriaFactrorySourceGenerator
    : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider
            .CreateSyntaxProvider(Predicate, Transform)
            .Collect();
        
        context.RegisterSourceOutput(provider, static (spc, source) => Generate(spc, source));
    }
}