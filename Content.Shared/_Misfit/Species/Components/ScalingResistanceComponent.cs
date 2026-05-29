using Content.Shared._Misfit.Species.Systems;
using Content.Shared.Damage;

namespace Content.Shared._Misfit.Species.Components;

/// <summary>
///
/// </summary>
[RegisterComponent, Access(typeof(ScalingResistanceSystem))]
public sealed partial class ScalingResistanceComponent : Component
{
    [DataField(required: true)]
    public DamageModifierSet StartingDamageModifiers = default!;

    [DataField(required: true)]
    public DamageModifierSet EndingDamageModifiers = default!;

    [DataField] // At what total damage should the entity be matching the ending set
    public float DamageForMaximumResistance = 50f;

}
