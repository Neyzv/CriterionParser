using System.Globalization;
using System.Numerics;
using CriterionParser.Enums;
using System.Text.RegularExpressions;

namespace CriterionParser.Models;

public class BaseCriteria<TModel>
    : Criteria<TModel>
{
    private Regex? _regex;

    public Comparator Comparator { get; }

    public string Value { get; }

    protected BaseCriteria(Operator @operator,
        Comparator comparator,
        string value)
        : base(@operator)
    {
        Comparator = comparator;
        Value = value;
    }

    protected bool Compare<T>(T obj)
            where T : INumber<T>
    {
        var comparison = obj.CompareTo(T.Parse(Value, CultureInfo.InvariantCulture));

        return Comparator switch
        {
            Comparator.Equal => comparison is 0,
            Comparator.Unequal => comparison is not 0,
            Comparator.LessThan => comparison < 0,
            Comparator.GreaterThan => comparison > 0,
            _ => throw new NotSupportedException($"Unsupported comparator '{Comparator}' for this type of elements.")
        };
    }

    protected bool Compare(string str) =>
        Comparator switch
        {
            Comparator.Equal => str == Value,
            Comparator.Like => str.Equals(Value, StringComparison.InvariantCultureIgnoreCase),
            Comparator.Unequal => str != Value,
            Comparator.Contains => str.Contains(Value),
            Comparator.ContainsLike => str.Contains(Value, StringComparison.InvariantCultureIgnoreCase),
            Comparator.StartsWith => str.StartsWith(Value),
            Comparator.StartsWithLike => str.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase),
            Comparator.EndsWith => str.EndsWith(Value),
            Comparator.EndsWithLike => str.EndsWith(Value, StringComparison.InvariantCultureIgnoreCase),
            Comparator.Match => (_regex ??= new Regex(Value, RegexOptions.Multiline)).Match(str).Success,
            _ => throw new NotSupportedException($"Unsupported comparator '{Comparator}' for this type of elements.")
        };

    public override bool Eval(TModel model) =>
        true;
}
