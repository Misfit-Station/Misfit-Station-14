using Content.Shared.Damage.Systems;
using Content.Shared._Misfit.Species.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Shared._Misfit.Species.Systems;

public sealed partial class EtherealScalingDamageSystem : EntitySystem
{
    [Dependency] private EtherealPowerSystem _ethereal = default!;

    private static readonly float EtherealMaxCharge = 1920f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EtherealScalingDamageComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(Entity<EtherealScalingDamageComponent> ent, ref DamageModifyEvent args)
    {
        var charge = _ethereal.GetOrganPower(ent.Owner);
        var chargeDifference = EtherealMaxCharge - ent.Comp.ChargeForEndingModifier;

        var scalar = Math.Clamp((charge - ent.Comp.ChargeForEndingModifier) / chargeDifference, 0, 1);

        DamageSpecifier newDamage = new();
        newDamage.DamageDict.EnsureCapacity(args.Damage.DamageDict.Count);

        foreach (var (key, value) in args.Damage.DamageDict)
        {
            if (value == 0)
                continue;

            if (value < 0)
            {
                newDamage.DamageDict[key] = value;
                continue;
            }

            float newValue = value.Float();

            if (ent.Comp.EndingDamageModifier.Coefficients.TryGetValue(key, out var coefficient) && ent.Comp.StartingDamageModifier.Coefficients.TryGetValue(key, out var startCoefficient))
            {
                var difference = coefficient - startCoefficient;
                newValue *= startCoefficient + difference * (1 - scalar);
            }

            if (newValue != 0)
                newDamage.DamageDict[key] = FixedPoint2.New(newValue);
        }

        args.Damage = newDamage;
    }
}
