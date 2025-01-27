using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CriterionParser.SourceGenerator.Models;

public sealed record CriteriaInformation(string ClassName, ImmutableArray<string> Identifiers);