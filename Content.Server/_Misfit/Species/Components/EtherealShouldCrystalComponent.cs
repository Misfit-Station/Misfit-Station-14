using Content.Shared.Damage;

namespace Content.Shared._Misfit.Species.Components;

/// <summary>
/// This tells the system that they should crystallize after a minute
/// </summary>
[RegisterComponent]
public sealed partial class EtherealShouldCrystalComponent : Component
{
    [DataField]
    public int TimeToCrystallize = 60;

    [DataField]
    public TimeSpan? IngameTimeToCrystallize;

    [DataField]
    public bool AlreadyCrystallized;
}

[RegisterComponent]
public sealed partial class EtherealCrystalComponent : Component
{
    [DataField]
    public DamageSpecifier DamageOnRevive = default!;

    [DataField]
    public int TimeToRevive = 120;

    [DataField]
    public TimeSpan? IngameTimeToRevive;
}
