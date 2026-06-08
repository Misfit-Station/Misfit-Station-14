using Content.Shared._Misfit.Species.Components;
using Content.Shared.Administration.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Misfit.Species.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class EtherealCrystalSystem : EntitySystem
{
    [Dependency] private DamageableSystem _damageSystem = default!;
    [Dependency] private RejuvenateSystem _rejuvenateSystem = default!;
    [Dependency] private IGameTiming _gameTiming = default!;
    [Dependency] private EntityManager _entityManager = default!;
    [Dependency] private SharedEntityStorageSystem _storageSystem = default!;

    private static readonly EntProtoId EtherealCrystalProto = "MobEtherealCrystal";


    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EtherealShouldCrystalComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnMobStateChanged(EntityUid uid, EtherealShouldCrystalComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead)
        {
            component.IngameTimeToCrystallize = _gameTiming.CurTime + TimeSpan.FromSeconds(component.TimeToCrystallize);
        }
        else if (args.OldMobState == MobState.Dead && component.IngameTimeToCrystallize.HasValue) // Case in which they are revived
        {
            component.IngameTimeToCrystallize = null;
            component.AlreadyCrystallized = false;
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var shouldCrystalEnum = EntityQueryEnumerator<EtherealShouldCrystalComponent>();

        List<EntityUid> shouldPolymorphUids = [];

        while (shouldCrystalEnum.MoveNext(out var uid, out var component))
        {
            if (!component.IngameTimeToCrystallize.HasValue || component.IngameTimeToCrystallize >= _gameTiming.CurTime)
                continue;

            shouldPolymorphUids.Add(uid);
        }

        var crystalEnum = EntityQueryEnumerator<EtherealCrystalComponent>();

        List<(EntityUid, DamageSpecifier)> shouldReviveUids = [];

        while (crystalEnum.MoveNext(out var uid, out var component))
        {
            if (!component.IngameTimeToRevive.HasValue || component.IngameTimeToRevive >= _gameTiming.CurTime)
                continue;

            shouldReviveUids.Add((uid, component.DamageOnRevive));
        }

        foreach (var uid in shouldPolymorphUids)
        {
            HandleCrystallization(uid);
        }

        foreach (var (uid, damageSpecifier) in shouldReviveUids)
        {
            HandleRevival(uid, damageSpecifier);
        }
    }

    private void HandleCrystallization(EntityUid uid)
    {
        if (!TryComp<EtherealShouldCrystalComponent>(uid, out var shouldCrystal))
            return;

        if (!_entityManager.TryGetComponent<TransformComponent>(uid, out var xform))
            return;

        shouldCrystal.IngameTimeToCrystallize = null;
        shouldCrystal.AlreadyCrystallized = true;

        var crystalUid = _entityManager.SpawnAtPosition(EtherealCrystalProto, xform.Coordinates);

        if (TryComp<EtherealColorComponent>(uid, out var etherealColor))
            CopyComp(uid, crystalUid, etherealColor);

        _storageSystem.Insert(uid, crystalUid);

        if (!TryComp<EtherealCrystalComponent>(crystalUid, out var crystalComp))
            return;

        crystalComp.IngameTimeToRevive = _gameTiming.CurTime + TimeSpan.FromSeconds(crystalComp.TimeToRevive);
    }

    private void HandleRevival(EntityUid uid, DamageSpecifier damageSpecifier)
    {
        if(!TryComp<EntityStorageComponent>(uid, out var entityStorage))
            return;

        if(!entityStorage.Contents.ContainedEntities.TryFirstOrNull(out var etherealUid))
            return;
        _storageSystem.EmptyContents(uid);

        _rejuvenateSystem.PerformRejuvenate((EntityUid)etherealUid);

        _damageSystem.TryChangeDamage((EntityUid)etherealUid, damageSpecifier, true);
        QueueDel(uid);
    }
}
