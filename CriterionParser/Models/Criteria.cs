using CriterionParser.Enums;

namespace CriterionParser.Models;

public abstract class Criteria
{
    public Operator Operator { get; }

    internal Criteria(Operator @operator) =>
        Operator = @operator;
}

public abstract class Criteria<TModel>
    : Criteria
{
    protected Criteria(Operator @operator)
        : base(@operator) { }

    /// <summary>
    /// Eval the <see cref="Criteria{TModel}"/> for the provided instance of <see cref="TModel"/>.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public abstract bool Eval(TModel model);
}
