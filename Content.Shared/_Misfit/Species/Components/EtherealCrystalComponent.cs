using Content.Shared.Damage;

namespace Content.Shared._Misfit.Species.Components;
[RegisterComponent]
public sealed partial class EtherealCrystalComponent : Component
{
    [DataField]
    public DamageSpecifier DamageOnRevive = default!;

    [DataField]
    public int TimeToRevive = 10;

    [DataField]
    public TimeSpan? IngameTimeToRevive;
}

/// <summary>
/// This tells the system that they should crystallize after a minute
/// </summary>
[RegisterComponent]
public sealed partial class EtherealShouldCrystalComponent : Component
{
    [DataField]
    public int TimeToCrystallize = 10;

    [DataField]
    public TimeSpan? IngameTimeToCrystallize;

    [DataField]
    public bool AlreadyCrystallized;
}
