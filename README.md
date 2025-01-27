# CriterionParser
A boolean expression parser based on a input string.
## How to use
### Implement a model
Here we will implement a Character class which will represent a player in a video game.
```cs
internal sealed record Character(string Name, int Gold);
```
### Create a criteria
We will implement a criteria which will check the amount of gold of our `Character`.
```cs
[CriteriaIdentifier("GO"), CriteriaIdentifier("Go")] // A criteria can have multiple identifier.
internal sealed class GoldCriteria
    : BaseCriteria<Character>
{
    public GoldCriteria(Operator @operator, Comparator comparator, string value)
        : base(@operator, comparator, value) { }

    public override bool Eval(Character model) =>
        Compare(model.Gold);
}
```
And the Factory will be source generated and no reflection will be used, the factory is named `CriteriaFactory`.
### Use it
You'll need to create an instance of the `CriterionExpressionParser` and passing into the constructor an instance of `CriteriaFactory`.
```cs
var criterionExpressionParser = new CriterionExpressionParser(new CriteriaFactory());
```
And we can now parse and evaluate our criteria
```cs
var criterionExpressionParser = new CriterionExpressionParser(new CriteriaFactory());
var player = new Character
{
    Name = "SamplePlayer",
    Gold = 255
};
var expression = criterionExpressionParser.Parse<Character>("GO>200");
Console.WriteLine(expression.Eval(player)); // will print true
```

