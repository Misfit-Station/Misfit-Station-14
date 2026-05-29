using Content.Shared.Damage.Systems;
using Content.Shared._Misfit.Species.Components;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;

namespace Content.Shared._Misfit.Species.Systems;

public partial class ScalingResistanceSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScalingResistanceComponent, DamageModifyEvent>(OnDamageModify);

    }
    private void OnDamageModify(EntityUid uid, ScalingResistanceComponent component, DamageModifyEvent args)
    {
        var scalar = Math.Clamp((_damageable.GetTotalDamage(uid) / component.DamageForMaximumResistance).Float(), 0, 1);


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

            if (component.EndingDamageModifiers.Coefficients.TryGetValue(key, out var coefficient) && component.StartingDamageModifiers.Coefficients.TryGetValue(key, out var startCoefficient))
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
