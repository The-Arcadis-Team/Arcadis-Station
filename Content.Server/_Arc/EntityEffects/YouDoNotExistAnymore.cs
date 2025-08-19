using System.Linq;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Language;
using Content.Server.Speech.Components;
using Content.Shared.EntityEffects;
using Content.Shared.Language;
using Content.Shared.Language.Systems;
using Content.Shared.Mind.Components;
using Robust.Shared.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Language.Components; //Delta-V - Banning humanoids from becoming ghost roles.
using Content.Shared.Language.Events;
using Robust.Shared.Audio;
using Robust.Server.Audio;
using Content.Shared.Coordinates;
using Robust.Shared.Audio.Systems;
using Robust.Server.GameObjects;

namespace Content.Server._Arc.EntityEffects.Effects;

public sealed partial class NoMoreExisting : EntityEffect
{

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-make-sentient", ("chance", Probability));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var entityManager = args.EntityManager;

        var uid = args.TargetEntity;

        var audioSystem = entityManager.System<AudioSystem>();

        var _transform = entityManager.System<TransformSystem>();

        SoundSpecifier noMoreSound = new SoundPathSpecifier("/Audio/_ARC/no_more.ogg");

        var coordinates = _transform.GetMoverCoordinates(uid);

        audioSystem.PlayPvs(noMoreSound, coordinates, AudioParams.Default);

        entityManager.QueueDeleteEntity(uid);
    }
}
