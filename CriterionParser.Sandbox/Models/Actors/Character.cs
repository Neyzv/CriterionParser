namespace CriterionParser.Sandbox.Models.Actors;

internal sealed record Character
{
    public required int LifePoints { get; set; }

    public required string Name { get; set; }

    public required ushort Gold { get; set; }
}
