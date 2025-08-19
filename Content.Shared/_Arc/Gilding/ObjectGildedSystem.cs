using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Content.Shared.Tools.Systems;
using Content.Shared.Interaction;
using Robust.Shared.Serialization;
using Content.Shared.DoAfter;
using Robust.Shared.Physics.Components;
using Content.Shared.Coordinates;
using Content.Shared.Stacks;
using Robust.Shared.Network;
using Content.Shared.Mind;

namespace Content.Shared._Arc.Admemery
{
    public sealed class ObjectGildedSystem : EntitySystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

        [Dependency] private readonly SharedToolSystem _toolSystem = default!;

        [Dependency] private readonly ILogManager _logManager = default!;

        [Dependency] private readonly EntityManager _entityManager = default!;

        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

        [Dependency] private readonly INetManager _netMan = default!;

        [Dependency] private readonly SharedStackSystem _stackSystem = default!;

        [Dependency] private readonly SharedAudioSystem _audioSystem = default!;

        [Dependency] private readonly SharedMindSystem _mind = default!;

        private ISawmill _sawmill = default!;

        public override void Initialize()
        {
            base.Initialize();

            _sawmill = _logManager.GetSawmill("gilded");

            SubscribeLocalEvent<ObjectGildedComponent, ComponentStartup>(OnComponentStartup);
            SubscribeLocalEvent<ObjectGildedComponent, ComponentRemove>(OnComponentShutdown);
            SubscribeLocalEvent<ObjectGildedComponent, InteractUsingEvent>(OnInteract);
            SubscribeLocalEvent<ObjectGildedComponent, GildedObjectMeltedEvent>(PostMelt);


        }
        private void OnComponentStartup(EntityUid uid, ObjectGildedComponent component, ComponentStartup args)
        {

            _sawmill.Info("Component starting");

            AddGoldenShader(uid, component);

            // Play the sound effect on the component
            _audioSystem.PlayPvs(component.GildedSound, uid, AudioParams.Default);
        }

        private void AddGoldenShader(EntityUid uid, ObjectGildedComponent component)
        {
            EnsureComp<AppearanceComponent>(uid);
            EnsureComp<ObjectGildedComponent>(uid);

            _appearanceSystem.SetData(uid, GildedVisuals.Gilded, true);
        }

        private void OnComponentShutdown(EntityUid uid, ObjectGildedComponent component, ComponentRemove args)
        {

            _sawmill.Info("Component shutting down");

            if (!Terminating(uid))
                _appearanceSystem.SetData(uid, GildedVisuals.Gilded, false);
        }

        private void OnInteract(EntityUid uid, ObjectGildedComponent component, InteractUsingEvent args)
        {
            if (args.Handled || !_toolSystem.HasQuality(args.Used, "Welding"))
                return;


            float delay = 3f;

            args.Handled = _toolSystem.UseTool
                (args.Used,
                args.User,
                args.Target,
                delay,
                "Welding",
                new GildedObjectMeltedEvent
                {
                    Delay = delay,
                });
        }

        private void PostMelt(EntityUid uid, ObjectGildedComponent component, GildedObjectMeltedEvent args)
        {
            if (Terminating(uid))
                return;

            if (!TryComp<PhysicsComponent>(uid, out var physicsComponent))
                return;

            if (_netMan.IsServer)
            {
                var goldToSpawn = physicsComponent.Mass; // We can reasonably assume each bar is 1kg

                while (goldToSpawn > 0)
                {
                    var stack = goldToSpawn > 30 ? 30 : goldToSpawn;
                    goldToSpawn -= stack;

                    var gold = _entityManager.SpawnEntity("IngotGold1", _transformSystem.GetMoverCoordinates(uid));
                    _stackSystem.SetCount(gold, (int) stack);

                }

                if (_mind.TryGetMind(uid, out var mindId, out var mind))
                {
                    SoundSpecifier noMoreSound = new SoundPathSpecifier("/Audio/_ARC/no_more.ogg");

                    var coordinates = _transformSystem.GetMoverCoordinates(uid);

                    _audioSystem.PlayPvs(noMoreSound, coordinates, AudioParams.Default);
                }

                _entityManager.QueueDeleteEntity(uid);
            }
        }
    }
}
