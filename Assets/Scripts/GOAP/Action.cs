using System;

public class GoapAction<Model>
{
    public string name;

    public Func<Model, bool> condition;
    public Func<Model, Model> effect;
    public float cost;

    public GoapAction(string name, Func<Model, bool> condition, Func<Model, Model> effect, float cost)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
        this.effect = effect ?? throw new ArgumentNullException(nameof(effect));
        this.cost = cost;
    }
}
