using Content.Shared.Body;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Body;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Shared._Misfit.EntityEffects.Effects.Power;

/// <summary>
/// Modifies power by a given amount multiplied by scale. This can increase or decrease power in organs as well.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ChangePowerEntityEffectSystem : EntityEffectSystem<BodyComponent, ChangePower>
{
    [Dependency] private SharedBatterySystem _battery = default!;
    [Dependency] private BodySystem _body = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BodyComponent, OrganChangePowerEvent>(_body.RelayEvent);
        SubscribeLocalEvent<BatteryComponent, BodyRelayedEvent<OrganChangePowerEvent>>(OnOrganEvent);
    }

    protected override void Effect(Entity<BodyComponent> entity, ref EntityEffectEvent<ChangePower> args)
    {
        var ev = new OrganChangePowerEvent(args.Effect.Amount * args.Scale);
        RaiseLocalEvent(entity, ref ev);

        if (!TryComp<BatteryComponent>(entity, out var batteryComponent))
            return;

        _battery.ChangeCharge((entity, batteryComponent), ev.Amount);
    }

    private void OnOrganEvent(Entity<BatteryComponent> ent, ref BodyRelayedEvent<OrganChangePowerEvent> args)
    {
        _battery.ChangeCharge(ent.AsNullable(), args.Args.Amount);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class ChangePower : EntityEffectBase<ChangePower>
{
    /// <summary>
    /// Amount of power we're applying or removing if negative.
    /// </summary>
    [DataField]
    public float Amount = 1f;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("entity-effect-guidebook-modify-bleed-amount", ("chance", Probability), ("deltasign", MathF.Sign(Amount)));
}

[ByRefEvent]
public record struct OrganChangePowerEvent(float Amount);
