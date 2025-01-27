using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using CriterionParser.SourceGenerator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CriterionParser.SourceGenerator;

public sealed partial class CriteriaFactrorySourceGenerator
{
    private const string BaseCriteriaName = "BaseCriteria<";
    private const string CriteriaIdentifierName = "CriteriaIdentifier";
    private const string CriteriaIdentifierAttributeName = "CriteriaIdentifierAttribute";
    
    private static bool Predicate(SyntaxNode node, CancellationToken ct)
    {
        if (node is not ClassDeclarationSyntax classSyntax)
            return false;

        if (classSyntax.BaseList is null)
            return false;
        
        if(!classSyntax
               .AttributeLists
               .SelectMany(x => x.Attributes)
               .Any(static attribute => attribute.Name.ToString() == CriteriaIdentifierName))
            return false;

        return classSyntax
            .BaseList
            .Types
            .Select(static x => x.Type)
            .Any(static x => x.ToString().Contains(BaseCriteriaName));
    }

    private static CriteriaInformation Transform(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) is not INamedTypeSymbol symbol)
            throw new Exception("Can not get declared symbol.");

        var identifierAttributes = symbol
            .GetAttributes()
            .Where(x => x.AttributeClass?.Name.ToString() == CriteriaIdentifierAttributeName)
            .Select(x=> (string)x.ConstructorArguments.First().Value!)
            .ToImmutableArray();
        
        if (identifierAttributes.IsEmpty)
            throw new Exception("Can not find criteria identifier attribute.");

        return new CriteriaInformation(symbol.ToDisplayString(), identifierAttributes);
    }
}