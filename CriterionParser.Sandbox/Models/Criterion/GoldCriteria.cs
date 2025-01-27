using CriterionParser.Attributes;
using CriterionParser.Enums;
using CriterionParser.Models;
using CriterionParser.Sandbox.Models.Actors;

namespace CriterionParser.Sandbox.Models.Criterion;

[CriteriaIdentifier("GO")]
internal sealed class GoldCriteria
    : BaseCriteria<Character>
{
    public GoldCriteria(Operator @operator, Comparator comparator, string value)
        : base(@operator, comparator, value) { }

    public override bool Eval(Character model) =>
        Compare(model.Gold);
}
