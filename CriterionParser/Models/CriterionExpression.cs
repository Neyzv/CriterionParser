using CriterionParser.Enums;

namespace CriterionParser.Models;

public sealed class CriterionExpression<TModel>
    : Criteria<TModel>
{
    private readonly IReadOnlyList<Criteria<TModel>> _criterion;

    internal CriterionExpression(IReadOnlyList<Criteria<TModel>> criterias, Operator op = Operator.And)
        : base(op) =>
        _criterion = criterias;

    public override bool Eval(TModel model)
    {
        var res = true;
        var op = Operator.And;

        for (int i = 0; i < _criterion.Count && op is Operator.Or == !res; i++)
        {
            res = _criterion[i].Eval(model);
            op = _criterion[i].Operator;
        }

        return res;
    }
}
