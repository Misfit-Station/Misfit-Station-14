using System.Linq;
using Content.Shared._Misfit.Species.Components;
using Content.Shared.Body;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Misfit.Species.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class EtherealColorSystem : EntitySystem
{
    [Dependency] private SharedPointLightSystem _pointLight = default!;
    [Dependency] private SharedVisualBodySystem _visualBody = default!;
    [Dependency] private DamageableSystem _damageable = default!;

    // TODO: Make all of these a part of the component PLEASE
    private static readonly FixedPoint2 TotalHealth = 200;
    private static readonly ProtoId<OrganCategoryPrototype> Head = "Head";
    private static readonly HumanoidVisualLayers[] LayersToForce =
        [HumanoidVisualLayers.Hair, HumanoidVisualLayers.FacialHair];

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EtherealColorComponent, StartingGearEquippedEvent>(OnComponentStartup);
        SubscribeLocalEvent<EtherealColorComponent, DamageChangedEvent>(OnDamageChanged);
    }

    private void OnComponentStartup(EntityUid uid, EtherealColorComponent component, ref StartingGearEquippedEvent args)
    {
        if (!_pointLight.TryGetLight(uid, out var lightComp))
            return;

        if (!_visualBody.TryGatherMarkingsData(uid, null, out var profiles, out _, out var _))
            return;

        if (!profiles.TryFirstOrNull(out var profile))
            return;

        var skinColor = profile.Value.Value.SkinColor; // Every "organ" should have the same skin color

        _pointLight.SetColor(uid, skinColor, lightComp);

        component.InitialColor = skinColor;
    }

    private void OnDamageChanged(EntityUid uid, EtherealColorComponent component, DamageChangedEvent args)
    {
        // Note from Falcon - GetTotalDamage is deprecated, this will likely need to be refactored more in the future
        var scalar = Math.Clamp(((TotalHealth - _damageable.GetTotalDamage((uid, args.Damageable))) / TotalHealth).Float(), 0, 1);
        Color newColor = new(Color.White.RGBA - (Color.White.RGBA - component.InitialColor.RGBA) * scalar);

        if (!_visualBody.TryGatherMarkingsData(uid, null, out var profiles, out _, out var markings))
            return;

        var etherealProfiles = profiles.ToDictionary(pair => pair.Key,
            pair => pair.Value with { SkinColor = newColor });
        _visualBody.ApplyProfiles(uid, etherealProfiles);
        _pointLight.SetColor(uid, newColor);

        if (!markings.TryGetValue(Head, out var headMarkings))
            return;

        foreach (var layer in LayersToForce)
        {
            if (!headMarkings.TryGetValue(layer, out var hairMarkings))
                continue;

            var newMarkings = hairMarkings.Select(marking => marking.WithColor(newColor)).ToList();

            headMarkings[layer] = newMarkings;
        }

        _visualBody.ApplyMarkings(uid, markings);
    }
}
