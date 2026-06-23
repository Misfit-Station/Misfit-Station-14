using Content.Shared._Misfit.Species.Systems;
using Content.Shared.Damage;

namespace Content.Shared._Misfit.Species.Components;

/// <summary>
///
/// </summary>
[RegisterComponent, Access(typeof(EtherealScalingResistanceSystem))]
public sealed partial class EtherealScalingResistanceComponent : Component
{
    /// <summary>
    /// This is the modifier it uses at full charge
    /// </summary>
    [DataField(required: true)]
    public DamageModifierSet StartingDamageResistance;

    /// <summary>
    /// This is the modifier it uses at the ending resistance value
    /// </summary>
    [DataField(required: true)]
    public DamageModifierSet EndingDamageResistance;

    [DataField] // At what total damage should the entity be matching the ending set
    public float ChargeForEndingResistance = 0f; // 1920 is total
}
