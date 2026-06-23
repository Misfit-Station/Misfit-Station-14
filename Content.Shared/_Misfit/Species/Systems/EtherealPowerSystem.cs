using Content.Shared.Body;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using JetBrains.Annotations;

namespace Content.Shared._Misfit.Species.Systems;

/// <summary>
/// This handles getting charge from ethereals, as well as handling the explosion logic for the EtherealPowerComponent
/// </summary>
public sealed partial class EtherealPowerSystem : EntitySystem
{
    [Dependency] private SharedBatterySystem _battery = default!;
    [Dependency] private BodySystem _body = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<BatteryComponent, BodyRelayedEvent<OrganGetPowerEvent>>(OnOrganGetPower);
    }

    [PublicAPI]
    public float GetOrganPower(Entity<BodyComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp))
            return 0;

        var ev = new OrganGetPowerEvent();
        _body.RelayEvent((ent.Owner, ent.Comp), ref ev);

        return ev.Charge;
    }

    private void OnOrganGetPower(Entity<BatteryComponent> ent, ref BodyRelayedEvent<OrganGetPowerEvent> args)
    {
        args.Args = args.Args with { Charge = args.Args.Charge + _battery.GetCharge(ent.AsNullable()) };
    }
}

[ByRefEvent]
public record struct OrganGetPowerEvent(float Charge = 0);
