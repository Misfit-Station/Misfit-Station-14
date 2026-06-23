using Content.Shared.Damage.Systems;
using Content.Shared._Misfit.Species.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Shared._Misfit.Species.Systems;

public sealed partial class EtherealScalingResistanceSystem : EntitySystem
{
    [Dependency] private EtherealPowerSystem _ethereal = default!;

    private static readonly float EtherealMaxCharge = 1920f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EtherealScalingResistanceComponent, DamageModifyEvent>(OnDamageModify);
    }

    private void OnDamageModify(Entity<EtherealScalingResistanceComponent> ent, ref DamageModifyEvent args)
    {
        var charge = _ethereal.GetOrganPower(ent.Owner);
        var chargeDifference = EtherealMaxCharge - ent.Comp.ChargeForEndingResistance;

        var scalar = Math.Clamp((charge - ent.Comp.ChargeForEndingResistance) / chargeDifference, 0, 1);

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

            if (ent.Comp.EndingDamageResistance.Coefficients.TryGetValue(key, out var coefficient) && ent.Comp.StartingDamageResistance.Coefficients.TryGetValue(key, out var startCoefficient))
            {
                var difference = coefficient - startCoefficient;
                newValue *= startCoefficient + difference * scalar;
            }

            if (newValue != 0)
                newDamage.DamageDict[key] = FixedPoint2.New(newValue);
        }

        args.Damage = newDamage;
    }
}
