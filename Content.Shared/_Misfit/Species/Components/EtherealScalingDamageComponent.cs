using Content.Shared._Misfit.Species.Systems;
using Content.Shared.Damage;

namespace Content.Shared._Misfit.Species.Components;

/// <summary>
/// This scales the amount of damage taken via a LERP between the StartingDamageModifier and the EndingDamageModifier based off of the Charge of the creature's organs
/// </summary>
[RegisterComponent, Access(typeof(EtherealScalingDamageSystem))]
public sealed partial class EtherealScalingDamageComponent : Component
{
    /// <summary>
    /// This is the modifier it uses at full charge
    /// </summary>
    [DataField(required: true)]
    public DamageModifierSet StartingDamageModifier;

    /// <summary>
    /// This is the modifier it uses at the ending modifier value
    /// </summary>
    [DataField(required: true)]
    public DamageModifierSet EndingDamageModifier;

    [DataField] // At what total damage should the entity be matching the ending set
    public float ChargeForEndingModifier = 0f; // 1920 is total
}
