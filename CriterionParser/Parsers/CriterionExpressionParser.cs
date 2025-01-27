using CriterionParser.Enums;
using CriterionParser.Models;
using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace CriterionParser.Factories;

public sealed partial class CriterionExpressionParser
{
    private const string ExpressionLabel = "expression";
    private const string OperatorLabel = "operator";

    private const string IdentifierLabel = "identifier";
    private const string ComparatorLabel = "comparator";
    private const string ValueLabel = "value";

    private const char OpenedParenthesis = '(';
    private const char ClosedParenthesis = ')';

    private static readonly IReadOnlyDictionary<string, Comparator> StrToComparators = new Dictionary<string, Comparator>()
    {
        ["=="] = Comparator.Equal,
        ["=~"] = Comparator.Like,
        ["!="] = Comparator.Unequal,
        [">"] = Comparator.GreaterThan,
        ["<"] = Comparator.LessThan,
        ["$"] = Comparator.Contains,
        ["$~"] = Comparator.ContainsLike,
        ["->"] = Comparator.StartsWith,
        ["->~"] = Comparator.StartsWithLike,
        ["<-"] = Comparator.EndsWith,
        ["<-~"] = Comparator.EndsWithLike,
        [":"] = Comparator.Match
    }.ToFrozenDictionary();

    private static readonly IReadOnlyDictionary<string, Operator> StrToOperators = new Dictionary<string, Operator>
    {
        ["&"] = Operator.And,
        ["|"] = Operator.Or,
    }.ToFrozenDictionary();

    [GeneratedRegex(@$"(?<{ExpressionLabel}>\(.+?\)|[^&|]+?)(?<{OperatorLabel}>[&|]|$)", RegexOptions.Multiline)]
    private static partial Regex SplitExpressionRegex();

    [GeneratedRegex(@$"(?<{IdentifierLabel}>[A-z0-9]+?)(?<{ComparatorLabel}>==|=~|!=|>|<|\$|\$~|->~|->|<-|<-~|:)(?<{ValueLabel}>.+)", RegexOptions.Multiline)]
    private static partial Regex ExpressionExtractorRegex();
    
    private readonly ICriteriaFactory _criteriaFactory;
    
    public CriterionExpressionParser(ICriteriaFactory criteriaFactory) =>
        _criteriaFactory = criteriaFactory;

    /// <summary>
    /// Parse a string into a <see cref="CriterionExpression{TModel}"/> which can be evaluated, based on created <see cref="Criteria{TModel}"/>.
    /// </summary>
    /// <param name="expression">The string representation of the expression.</param>
    /// <typeparam name="TModel">The type of the model to be evaluated</typeparam>
    /// <returns>The created <see cref="CriterionExpression{TModel}"/>.</returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public CriterionExpression<TModel> Parse<TModel>(string expression)
    {
        var criteria = new List<Criteria<TModel>>();
        var stack = new Stack<(List<Criteria<TModel>> Criteria, string Expression)>();

        stack.Push((criteria, expression));

        while (stack.Count > 0)
        {
            var (currentCriteria, currentExpression) = stack.Pop();
            var matches = SplitExpressionRegex().Matches(currentExpression);

            for (var i = 0; i < matches.Count; i++)
            {
                if (!matches[i].Groups.ContainsKey(ExpressionLabel)
                    || !matches[i].Groups.ContainsKey(OperatorLabel))
                    throw new KeyNotFoundException();

                if (string.IsNullOrWhiteSpace(matches[i].Groups[ExpressionLabel].Value)
                    || (string.IsNullOrWhiteSpace(matches[i].Groups[OperatorLabel].Value) && i != matches.Count - 1))
                    throw new Exception($"Malformed criterion expression '{currentExpression}'.");

                var op = Operator.And;

                if (!string.IsNullOrWhiteSpace(matches[i].Groups[OperatorLabel].Value) &&
                            !StrToOperators.TryGetValue(matches[i].Groups[OperatorLabel].Value, out op))
                    throw new Exception($"Unknown operator '{matches[i].Groups[OperatorLabel].Value}'.");

                if (matches[i].Groups[ExpressionLabel].Value[0] is OpenedParenthesis)
                {
                    if (matches[i].Groups[ExpressionLabel].Value[^1] is not ClosedParenthesis)
                        throw new Exception($"Malformated criterion expression '{matches[i].Groups[ExpressionLabel].Value}', must have a closed parenthesis.");

                    var nestedCriteria = new List<Criteria<TModel>>();
                    currentCriteria.Add(new CriterionExpression<TModel>(nestedCriteria, op));
                    stack.Push((nestedCriteria, matches[i].Groups[ExpressionLabel].Value[1..^1]));
                }
                else
                {
                    var splittedExpression = ExpressionExtractorRegex().Match(matches[i].Groups[ExpressionLabel].Value);

                    if (!splittedExpression.Groups.ContainsKey(IdentifierLabel)
                        || !splittedExpression.Groups.ContainsKey(ComparatorLabel)
                        || !splittedExpression.Groups.ContainsKey(ValueLabel))
                        throw new Exception($"Missing some informations while parsing expression '{matches[i].Groups[ExpressionLabel].Value}'.");
                    
                    if (!StrToComparators.TryGetValue(splittedExpression.Groups[ComparatorLabel].Value, out var comparator))
                        throw new Exception($"Unknown comparator '{splittedExpression.Groups[ComparatorLabel].Value}'.");
                    
                    var criteriaInstance = _criteriaFactory.Create(splittedExpression.Groups[IdentifierLabel].Value,
                        op, comparator, splittedExpression.Groups[ValueLabel].Value);

                    if (criteriaInstance is not Criteria<TModel> typedCriteria)
                        throw new Exception($"Wrong generic type '{typeof(TModel).Name}' for criteria '{criteriaInstance}'.");
                    
                    currentCriteria.Add(typedCriteria);
                }
            }
        }

        return new CriterionExpression<TModel>(criteria);
    }
}