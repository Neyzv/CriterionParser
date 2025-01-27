using CriterionParser.Factories;
using CriterionParser.Sandbox.Models.Actors;

var admin = new Character
{
    LifePoints = 30_000,
    Gold = 9999,
    Name = "[Admin]"
};
var player = new Character
{
    LifePoints = 300,
    Gold = 254,
    Name = "SamplePlayer"
};

var criterionExpressionParser = new CriterionExpressionParser(new CriteriaFactory());

var expression = criterionExpressionParser.Parse<Character>("GO>5&GO==254");

Console.WriteLine(expression.Eval(admin));
Console.WriteLine(expression.Eval(player));